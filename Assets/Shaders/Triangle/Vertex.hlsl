struct VSInput
{
	[[vk::location(0)]] float3 Pos : POSITION0;
	[[vk::location(1)]] float3 Color : COLOR0;
};

struct UBO
{
	float4x4 Model;
	float4x4 View;
	float4x4 Projection;
};

cbuffer ubo : register(b0) { UBO ubo; }

struct VSOutput
{
	float4 Pos : SV_POSITION;
	[[vk::location(0)]] float3 Color : COLOR0;
};

VSOutput main(VSInput input)
{
	VSOutput output = (VSOutput)0;

	output.Color = input.Color;
	output.Pos = mul(ubo.Projection, mul(ubo.View, mul(ubo.Model, float4(input.Pos.xyz, 1.0))));

	return output;

}