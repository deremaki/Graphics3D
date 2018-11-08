#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0

float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

int phongblinn = 0;
int textures = 0;

float4x4 WorldInverseTranspose;

float3 DiffuseLightDirection = float3(10, 10, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float Shininess = 100;
float4 SpecularColor = float4(1, 1, 1, 1);
float SpecularIntensity = 1;
float3 ViewVector;

texture ModelTexture;
sampler2D textureSampler = sampler_state {
	Texture = (ModelTexture);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};


struct VertexShaderInput
{
    float4 Position : SV_POSITION0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION0;
	float3 WorldPosition : NORMAL0;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinate : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition.xyz;

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	
	output.Normal = normal.xyz;
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 light = normalize(DiffuseLightDirection);
	float3 normal = normalize(input.Normal);	
	float3 v = normalize(float3((ViewVector - input.WorldPosition).xyz));

	float lightIntensity = dot(normal, light);
	float4 diffColor = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	float dotProduct;
	float4 specular;

	if (phongblinn == 0) //phong lighting
	{
		float3 r = normalize(2 * dot(light, normal) * normal - light);
		dotProduct = saturate(dot(r, v));
		specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, Shininess), 0) * length(diffColor);
	}
	else //blinn lighting
	{
		float3 h = normalize((light + v) / abs(light + v));		
		dotProduct = saturate(dot(normal, h));
		specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, 2* Shininess), 0) * length(diffColor);
	}

	if (textures == 0)
	{
		float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
		textureColor.a = 1;
		return saturate(textureColor * diffColor + AmbientColor * AmbientIntensity + specular);
	}
	else
		return saturate(diffColor + AmbientColor * AmbientIntensity + specular);
}

technique Ambient
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
