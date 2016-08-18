#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float3 LightDirection;
half4 LightColor;
half4 LightAmbient;

half Gamma;

struct VertexShaderInput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	matrix WorldViewProjection = mul(mul(World, View), Projection);

	float3 normal = normalize(input.Normal);
	float3 lightDirection = normalize(LightDirection);

	float3 NdotL = max(0, dot(normal, lightDirection));
	float3 ambient = LightAmbient * input.Color.xyz;

	float3 diffuse = pow(NdotL * LightColor * input.Color.xyz, Gamma);
	float4 c = float4(diffuse + ambient, input.Color.a);

	
	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = c;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return input.Color;
}

VertexShaderOutput NormalVS(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	matrix WorldViewProjection = mul(mul(World, View), Projection);

	float3 normal = normalize(mul(input.Normal, WorldInverseTranspose));

	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = float4(normal/2 + 0.5, 0.5);

	return output;
}

float4 NormalPS(VertexShaderOutput input) : COLOR
{
	return input.Color;
}

technique Normals 
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL NormalVS();
		PixelShader = compile PS_SHADERMODEL NormalPS();
	}
};

VertexShaderOutput DepthVS(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	matrix WorldViewProjection = mul(mul(World, View), Projection);

	float3 normal = normalize(mul(input.Normal, WorldInverseTranspose));

	output.Position = mul(input.Position, WorldViewProjection);
	output.Color =  1 - output.Position.w/32;

	return output;
}

float4 DepthPS(VertexShaderOutput input) : COLOR
{
	return float4(input.Color.xyz, 1);
}

technique Depth 
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL DepthVS();
		PixelShader = compile PS_SHADERMODEL DepthPS();
	}
};

struct SelectInput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL;
	float4 Color : COLOR0;
};

struct SelectOutput
{
	float4 Position : SV_POSITION;
	float3 vPosition : NORMAL;
	float3 Normal : NORMAL1;
};

SelectOutput SelectVS(VertexShaderInput input) 
{
	SelectOutput output = (SelectOutput)0;
	matrix WorldViewProjection = mul(mul(World, View), Projection);


	output.Position = mul(input.Position, WorldViewProjection);
	output.vPosition = input.Position;
	output.Normal = input.Normal;

	return output;
}

float4 SelectPS(SelectOutput input) : COLOR
{
	float3 voxelPosition = floor(input.vPosition) - clamp(input.Normal, 0, 1);
	

	return float4(voxelPosition.x/32, voxelPosition.y/32, voxelPosition.z/32, 1);
}

technique VoxelEffect
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
	pass P1
	{
		VertexShader = compile VS_SHADERMODEL SelectVS();
		PixelShader = compile PS_SHADERMODEL SelectPS();
	}
};