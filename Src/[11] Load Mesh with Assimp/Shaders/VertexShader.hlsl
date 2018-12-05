cbuffer MatrixBuffer
{
	matrix WVP;
};


struct VertexInputType
{
	float4 Position : POSITION;
	float2 Tex : TEXCOORD0;
};

struct PixelInputType
{
	float4 Position : SV_POSITION;
	float2 Tex : TEXCOORD0;
};


PixelInputType VS(VertexInputType input)
{
	PixelInputType output;

	// Change the position vector to be 4 units for proper matrix calculations.
	input.Position.w = 1.0f;

	// Calculate the position of the vertex against the world, view, and projection matrices.
	output.Position = mul(input.Position, WVP);

	// Store the texture coordinates for the pixel shader to use.
	output.Tex = input.Tex;

	return output;
}
