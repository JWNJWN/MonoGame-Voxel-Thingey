#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

texture texColor;
texture texNormal;
texture texDepth;

sampler gColor = sampler_state
{
	Texture = (texColor);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = NEARSET;
};
sampler gNormal = sampler_state
{
	Texture = (texNormal);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
};
sampler gDepth = sampler_state
{
	Texture = (texDepth);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
};

matrix VPI;
matrix VI;

float3 CameraPosition;

float3 LightDirection;
float4 LightColor;
float LightIntensity;


struct Input
{
	float3 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct Output
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

Output MainVS(Input input)
{
	Output output;

	output.Position = float4(input.Position, 1);
	output.UV = input.UV;

	return output;
}

float4 Phong(float3 Position, float3 Normal, float SpecularIntensity, float SpecularPower)
{
	float3 Reflection = normalize(reflect(LightDirection, Normal));

	float Eye = normalize(CameraPosition - Position.xyz);
	float NL = dot(Normal, -LightDirection);

	
	float3 Ambient = 0.3;
	float3 Diffuse = NL * LightColor;
	float Specular = SpecularIntensity * pow(saturate(dot(Reflection, Eye)), SpecularPower);

	return LightIntensity * float4(Diffuse.rgb + Ambient.rgb, Specular);
}

float4 MainPS(Output input) : COLOR0
{
	float3 eNorm = tex2D(gNormal, input.UV);

	float Depth = tex2D(gDepth, input.UV);

	float4 Position = 1;
	Position.x = input.UV.x * 2 - 1;
	Position.y = -(input.UV.y * 2 - 1);
	Position.z = Depth;

	Position = mul(Position, VPI);

	return Phong(Position.xyz, eNorm.rgb, 1, 255);
}

technique Main
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};