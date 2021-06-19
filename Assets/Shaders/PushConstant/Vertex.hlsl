struct VSInput
{
	[[vk::location(0)]] float3 Pos : POSITION0;
	[[vk::location(1)]] float3 Color : COLOR0;
};

struct VSOutput
{
	float4 Pos : SV_POSITION;
	[[vk::location(0)]] float3 Color : COLOR0;
};


struct View
{
	float4x4 View;
	float4x4 Projection;
};


struct PushConsts
{
	float4x4 Model;
};



[[vk::push_constant]] PushConsts primitive;
ConstantBuffer<View> view : register(b0);


VSOutput main(VSInput input)
{
	VSOutput output = (VSOutput)0;

	output.Color = input.Color;
	output.Pos = mul(view.Projection, mul(view.View, mul(primitive.Model, float4(input.Pos.xyz, 1.0))));

	return output;

}