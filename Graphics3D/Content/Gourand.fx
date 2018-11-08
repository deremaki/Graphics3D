#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0

float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

int phongblinn;
int textures;

float4x4 WorldInverseTranspose;

float3 DiffuseLightDirection = float3(10, 10, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float Shininess = 200;
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
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD1;
	float4 color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	float3 WorldPosition = worldPosition.xyz;

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float3 normal3 = normal.xyz;

	float3 light = normalize(DiffuseLightDirection);
	float3 n = normalize(normal3);
	float3 v = normalize(float3((ViewVector - WorldPosition).xyz));

	float lightIntensity = dot(n, light);
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
		specular = SpecularIntensity * SpecularColor * max(pow(dotProduct, 2 * Shininess), 0) * length(diffColor);
	}
	float4 color = diffColor + AmbientColor * AmbientIntensity + specular;
	
	output.color = color;
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	if (textures == 0)
	{
		float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
		textureColor.a = 1;
		return saturate(textureColor * input.color);
	}
	return saturate(input.color);

}

technique Gourand
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}
