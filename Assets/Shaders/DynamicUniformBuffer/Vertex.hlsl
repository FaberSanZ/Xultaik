struct VSInput
{
	[[vk::location(0)]] float3 Pos : POSITION0;
	[[vk::location(1)]] float3 Color : COLOR0;
};

struct View
{
	float4x4 View;
	float4x4 Projection;
};


struct Model
{
	float4x4 Model;
};

ConstantBuffer<View> view : register(b0);
ConstantBuffer<Model> model_dynamic : register(b1);

struct VSOutput
{
	float4 Pos : SV_POSITION;
	[[vk::location(0)]] float3 Color : COLOR0;
};

VSOutput main(VSInput input)
{
	VSOutput output = (VSOutput)0;

	output.Color = input.Color;
	output.Pos = mul(view.Projection, mul(view.View, mul(model_dynamic.Model, float4(input.Pos.xyz, 1.0))));

	return output;

}