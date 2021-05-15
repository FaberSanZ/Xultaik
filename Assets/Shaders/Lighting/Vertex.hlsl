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

cbuffer ubo : register(b0) { UBO ubo; }



struct PushConsts {
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

	//output.UV = input.UV;

	//return output;





	// Change the position vector to be 4 units for proper matrix calculations.
	//input.Pos.w = 1.0f;

	// Calculate the position of the vertex against the world, view, and projection matrices.
    output.Pos = mul(ubo.Projection, mul(ubo.View, mul(mul(ubo.Model, primitive.model), float4(input.Pos.xyz, 1.0))));


	// Store the texture coordinates for the pixel shader.
	output.UV = input.UV;

	// Calculate the normal vector against the world matrix only.
	output.Normal = mul(input.Normal, (float3x3)mul(ubo.Model, primitive.model));

	// Normalize the normal vector.
	output.Normal = normalize(output.Normal);

	return output;

}