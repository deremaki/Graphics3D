#define VS_SHADERMODEL vs_4_0_level_9_3
#define PS_SHADERMODEL ps_4_0_level_9_3

float4x4 World;
float4x4 View;
float4x4 Projection;

float4 MaterialColor;

float3 LightDirection;
float3 LightPositions[2];
float3 LightDirections[2];
float4 LightColors[2];
float3 CameraPosition;

texture ModelTexture;

//Diffuse
float Kd = 1;
//Specular
float Shininess = 50;
float Ks = 1;
//Ambient
float Ka = 0.06;
//reflectors
float P = 5;


sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct TexturedShaderInput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL;
	float2 TextureCoordinate : TEXCOORD0;
};

struct TexturedShaderOutput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL;
	float4 Color : TEXCOORD0;
    float3 WorldPosition : POSITION1;
	float2 TextureCoordinate : TEXCOORD1;
};

TexturedShaderOutput TexturedVS(in TexturedShaderInput input)
{
	TexturedShaderOutput output = (TexturedShaderOutput)0;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);

	output.Normal = normalize(mul(input.Normal, (float3x3)World));
	output.Position = mul(viewPosition, Projection);
    output.WorldPosition = worldPosition.xyz;
	output.Color = MaterialColor;

	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 Diffuse(float3 N, float3 L, float distanceIntensity)
{
	float lightIntensity = saturate(dot(N, L));
	return lightIntensity;
}

float4 SpecPhong(float3 N, float3 L, float3 V, float distanceIntensity)
{
	float3 R = normalize(2 * saturate(dot(L, N)) * N - L);
	float4 dotProduct = saturate(dot(R, V));
	float4 specular = Ks * pow(dotProduct, 50);
	return specular;
}

float4 TexturedPS(TexturedShaderOutput input) : COLOR
{

	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    textureColor.a = 1;

	float3 N = normalize(input.Normal);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
    float distanceIntensity = 1;
	float4 resultIntensity = Ka;
	float3 L = normalize(LightDirection);
	float4 whiteColor = float4(1, 1, 1, 1); 
	float4 diffuse = Diffuse(N, L, distanceIntensity) * input.Color;
	float4 specular = SpecPhong(N, L, V, distanceIntensity);
	resultIntensity += whiteColor * (diffuse + specular);

	//reflectors
	for (int i = 0;i < 2;i++)
	{
		L = normalize(LightPositions[i].xyz - input.WorldPosition.xyz);
		diffuse = Diffuse(N, L, distanceIntensity) * input.Color;
		specular = SpecPhong(N, L, V, distanceIntensity);
		//Reflector
		float4 att = 0;
		float cos = dot(-normalize(LightDirections[i].xyz), L);
		att = pow(saturate(cos), P);

		resultIntensity += LightColors[i] * att * (diffuse + specular);

	}
	

	return resultIntensity;
}

technique TexturedColored
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL TexturedVS();
		PixelShader = compile PS_SHADERMODEL TexturedPS();
	}
};