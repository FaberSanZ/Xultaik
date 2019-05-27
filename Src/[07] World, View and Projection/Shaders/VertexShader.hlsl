cbuffer MatrixBuffer : register(b0)
{
	matrix W;
	matrix V;
	matrix P;
};


struct VS_INPUT
{
	float4 pos : POSITION;
	float4 color : COLOR;
};

struct VS_OUTPUT
{
	float4 pos : SV_POSITION;
	float4 color : COLOR;
};

VS_OUTPUT VS(VS_INPUT input)
{
	VS_OUTPUT output;

	// Convert the position vector to homogeneous coordinates for matrix calculations.
	input.pos.w = W;

	// Calculate the position of the vertex against the world, view, and projection matrices.
	output.pos = mul(input.pos, W);
	output.pos = mul(output.pos, V);
	output.pos = mul(output.pos, P);

	// Store the input color for the pixel shader to use.
	output.color = input.color;

	return output;
}
