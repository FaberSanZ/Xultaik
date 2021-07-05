
Texture2D textureColor[] : register(t1);
Texture2D textureColor2[] : register(t3);

SamplerState samplerColor : register(s2);


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
    float4 color = textureColor[NonUniformResourceIndex(index.i)].Sample(samplerColor, input.UV);
    float4 color2 = textureColor2[NonUniformResourceIndex(0)].Sample(samplerColor, input.UV);


    return color2;
}