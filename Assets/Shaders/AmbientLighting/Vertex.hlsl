

struct VSInput
{
	[[vk::location(0)]] float3 Pos : POSITION0;
	[[vk::location(1)]] float3 Normal : NORMAL0;
	[[vk::location(2)]] float2 UV : TEXCOORD0;
};


struct UBO
{
	float4x4 Model;
	float4x4 View;
	float4x4 Projection;
};

ConstantBuffer<UBO> ubo : register(b0);



struct PushConsts 
{
	float4x4 model;
};
[[vk::push_constant]] PushConsts primitive;


struct VSOutput
{
	float4 Pos : SV_POSITION;
	[[vk::location(0)]] float3 Normal : NORMAL0;
	[[vk::location(1)]] float2 UV : TEXCOORD0;
};

VSOutput main(VSInput input)
{
	VSOutput output = (VSOutput)0;

	float4x4 mod = mul(ubo.Model, primitive.model);

	// Calculate the position of the vertex against the world, view, and projection matrices.
    output.Pos = mul(ubo.Projection, mul(ubo.View, mul(mod, float4(input.Pos.xyz, 1.0))));

	// Store the texture coordinates for the pixel shader.
	output.UV = input.UV;

	// Calculate the normal vector against the world matrix only.
	output.Normal = mul(input.Normal, (float3x3)mod);

	// Normalize the normal vector.
	output.Normal = normalize(output.Normal);

	return output;

}