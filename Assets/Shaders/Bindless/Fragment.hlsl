

SamplerState samplerColor : register(s1);
Texture2D textureColor[] : register(t2);


struct PushConsts
{
    int i;
};
[[vk::push_constant]] PushConsts index;

struct VSOutput
{
	[[vk::location(0)]] float2 UV : TEXCOORD0;
};

float4 main(VSOutput input) : SV_TARGET
{
    float4 color_0 = textureColor[NonUniformResourceIndex(index.i)].Sample(samplerColor, input.UV);
    float4 color_1 = textureColor[NonUniformResourceIndex(0)].Sample(samplerColor, input.UV);
    
    //color.r = (color_0.r * color_1.r);
    //color.g = (color_0.g * color_1.g);
    //color.b = (color_0.b * color_1.b);
    
    return color_0;
}