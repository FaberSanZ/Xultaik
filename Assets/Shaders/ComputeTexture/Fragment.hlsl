struct VSOutput
{
	[[vk::location(0)]] float2 UV : TEXCOORD0;
};




Texture2D Texture : register(t1);
SamplerState Sampler : register(s2);




float4 main(VSOutput input) : SV_TARGET
{

	float4 color= Texture.Sample(Sampler, input.UV);


	return color;
}