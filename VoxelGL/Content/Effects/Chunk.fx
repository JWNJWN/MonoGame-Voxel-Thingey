float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;
 
float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float TextureTotalSize;
float TextureVoxelSize;

texture ModelTexture;
sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};
 
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
	float2 TextureOffset : Normal;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 wPosition : Color0;
    float4 Color : COLOR1;
    float2 TextureCoordinate : TEXCOORD0;
	float2 TextureOffset : TEXCOORD1;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.wPosition = worldPosition;
	output.Color = input.Color;

	float2 textureCoord = input.TextureCoordinate;

    output.TextureCoordinate = textureCoord;
	output.TextureOffset = input.TextureOffset;
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 light = normalize(DiffuseLightDirection);
    float3 normal = normalize(mul(cross(ddy(input.wPosition.xyz), ddx(input.wPosition.xyz)), WorldInverseTranspose));
	
	//Diffuse
	float lightIntensity = mul(normal, light);
	
	float uvMult = TextureVoxelSize/TextureTotalSize;
	float2 uv = (input.TextureOffset * uvMult) + fmod(input.TextureCoordinate, 1.0) * uvMult;

    float4 textureColor = tex2D(textureSampler, uv);
    textureColor.a = 1;
 
	input.Color = DiffuseColor * DiffuseIntensity * lightIntensity;

    return saturate((textureColor * input.Color) + AmbientColor * AmbientIntensity);
}

technique Textured
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}