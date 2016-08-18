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
sampler gDepth = sampler_state
{
	Texture = (texDepth);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

matrix VPI;
matrix VI;

float3 CameraPosition;

float3 LightDirection;
float4 LightColor;
float LightIntensity;

float2 GBufferTextureSize;

struct Input
{
	float3 Position : POSITION;
	float2 UV : TEXCOORD;
};

struct Output
{
	float4 Position : POSITION;
	float2 UV : TEXCOORD;
};

Output MainVS(Input input)
{
	Output output;

	output.Position = float4(input.Position, 1);
	output.UV = input.UV - float2(1/GBufferTextureSize.xy);

	return output;
}

float4 manualSample(sampler Sampler, float2 UV, float2 textureSize)
{
	float2 texelPos = textureSize * UV;
	float2 lerps = frac(texelPos);
	float texelSize = 1/textureSize;

	float4 sourceVals[4];
	sourceVals[0] = tex2D(Sampler, UV);
	sourceVals[1] = tex2D(Sampler, UV + float2(texelSize, 0));
	sourceVals[2] = tex2D(Sampler, UV + float2(0, texelSize)); 
	sourceVals[3] = tex2D(Sampler, UV + float2(texelSize, texelSize));

	float4 interpolated = lerp(lerp(sourceVals[0], sourceVals[1], lerps.x),
							   lerp(sourceVals[2], sourceVals[3], lerps.x), lerps.y);
	
	return interpolated;
}

float4 Phong(float3 Position, float3 Normal, float SpecularIntensity, float SpecularPower)
{
	float3 Reflection = normalize(reflect(LightDirection, Normal));

	float Eye = normalize(CameraPosition - Position.xyz);
	float NL = dot(Normal, -LightDirection);

	
	float3 Ambient = LightColor*0.3;
	float3 Diffuse = NL * LightColor;
	float Specular = SpecularIntensity * pow(saturate(dot(Reflection, Eye)), SpecularPower);

	return LightIntensity * float4(Diffuse.rgb + Ambient.rgb, Specular);
}

float4 MainPS(Output input) : COLOR
{
	float3 eNorm = tex2D(gNormal, input.UV);

	//float3 Normal = mul(tex2D(gNormal, input.UV).rgb, VI);

	//float SpecularIntensity = tex2D(gColor, input.UV).w;

	float Depth = manualSample(gDepth, input.UV, GBufferTextureSize).x;

	float4 Position = 1;
	Position.x = input.UV.x * 2 - 1;
	Position.y = -(input.UV.x * 2 - 1);
	Position.z = Depth;

	Position = mul(Position, VPI);

	Position /= Position.w;

	return Phong(Position.xyz, eNorm.rgb, 0.1, 255);
}

technique Main
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};