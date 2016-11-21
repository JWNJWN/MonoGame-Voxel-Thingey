float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;
 
float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

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
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 wPosition : Color0;
    float4 Color : COLOR1;
    float2 TextureCoordinate : TEXCOORD0;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.wPosition = worldPosition;
 
	output.Color = input.Color;

    output.TextureCoordinate = input.TextureCoordinate;
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 light = normalize(DiffuseLightDirection);
    float3 normal = normalize(mul(cross(ddy(input.wPosition.xyz), ddx(input.wPosition.xyz)), WorldInverseTranspose));
	
	//Diffuse
	float lightIntensity = mul(normal, light);

	float2 uv = frac(input.TextureCoordinate);

    float4 textureColor = tex2D(textureSampler, uv);
    textureColor.a = 1;
 
    return saturate(textureColor * (input.Color + (DiffuseColor * DiffuseIntensity * lightIntensity)) + AmbientColor * AmbientIntensity);
}

float4 PS(VertexShaderOutput input) : COLOR0
{

    float3 light = normalize(DiffuseLightDirection);
    float3 normal = normalize(cross(ddx(input.wPosition.xyz), ddy(input.wPosition.xyz)));

	//Diffuse
	float lightIntensity = mul(normal, light);


	float2 uv = float2(dot(normal.zxy, input.wPosition.xyz), dot(normal.yzx, input.wPosition.xyz));

	//uv+=0.5;

    float4 textureColor = tex2D(textureSampler, uv);
    textureColor.a = 1;
 
    return float4(uv, 0, 0);
}

technique Textured
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
    pass Pass2
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PS();
    }
}