#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix View;
matrix Projection;

//GBUFFER

struct VertexShaderInput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION;
	float3 Color : COLOR0;
	float3 Normal : COLOR1;
	float3 Depth : COLOR2;
};

struct PixelShaderOutput
{
	float4 Color : COLOR0;
	float4 Normal : COLOR1;
	float4 Depth : COLOR2;
	float4 Specular : COLOR3;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	matrix WVP = mul(mul(World, View), Projection);

	output.Position = mul(input.Position, WVP);
	output.Depth = output.Position.w/32;
	output.Normal = input.Normal;
	output.Color = input.Color.xyz;

	return output;
}

PixelShaderOutput MainPS(VertexShaderOutput input)
{
	PixelShaderOutput output;
	
	output.Color = float4(input.Color, 1);
	output.Normal = float4(input.Normal, 1);
	output.Depth = float4(input.Depth, 1);
	output.Specular = float4(1,1,1,1);

	return output;
}

//LIGHTING
texture texColor;
texture texNormal;
texture texDepth;

sampler gDepth = sampler_state
{
	Texture = (texDepth);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
sampler gNormal = sampler_state
{
	Texture = (texNormal);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
sampler gColor = sampler_state
{
	Texture = (texColor);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

const int Lights = 32;
float3 lightPosition;
float3 lightColor;

float3 viewPosition;

struct LightInput
{
	float4 Position : POSITION;
	float2 UV : TEXCOORD;
};

struct LightOutput
{
	float4 Position : POSITION;
	float2 UV : TEXCOORD;
};

LightOutput LightVS(LightInput input)
{
	LightOutput output = (LightOutput)0;
	
	output.Position = input.Position;
	output.UV = input.UV;

	return output;
}

float4 LightPS(LightOutput input) : COLOR
{
	float3 fragPosition = tex2D(gDepth, input.UV).rgb;
	float3 normal = tex2D(gNormal, input.UV).rgb;
	float3 color = tex2D(gColor, input.UV).rgb;

	float3 lighting = color*0.1;
	float3 viewDirection = normalize(viewPosition - fragPosition);

	float3 lightDirection = normalize(lightPosition - fragPosition);
	float3 diffuse = max(dot(normal, lightDirection), 0) * color * lightColor;
	lighting += diffuse;

	return float4(lighting, 1);
}

technique Deferred
{
	pass GBuffer
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
	pass Lighting
	{
		VertexShader = compile VS_SHADERMODEL LightVS();
		PixelShader = compile PS_SHADERMODEL LightPS();
	}
};