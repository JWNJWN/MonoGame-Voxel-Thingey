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
	output.Depth = output.Position.w/20;
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

technique Deferred
{
	pass GBuffer
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};