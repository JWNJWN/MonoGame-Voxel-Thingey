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

float Gamma;

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
    float4 TextureCoordinate : TEXCOORD0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 wPosition : Color0;
    float4 Color : COLOR1;
	float4 TextureInfo : TEXCOORD0;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	
	float4 pos = input.Position.xyzw;
	
    float4 worldPosition = mul(pos, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.wPosition = worldPosition;
	output.Color = input.Color;

	output.TextureInfo = input.TextureCoordinate;
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 light = normalize(DiffuseLightDirection);
    float3 normal = normalize(cross(ddy(input.wPosition.xyz), ddx(input.wPosition.xyz)));
	
	//Diffuse
	float lightIntensity = dot(normal, light);
	
	float uvMult = TextureVoxelSize/TextureTotalSize;
	float2 uv = (input.TextureInfo.zw * uvMult) + fmod(input.TextureInfo.xy, 1) * uvMult;

    float4 textureColor = tex2D(textureSampler, uv);
    textureColor.a = 1;
 
	float4 lightColor = pow(DiffuseColor * DiffuseIntensity * lightIntensity, 1/Gamma);

    return saturate((textureColor * lightColor) + AmbientColor * AmbientIntensity);
}

technique Textured
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}