///////////////////////
////   GLOBALS
///////////////////////
float4x4 W;
float4x4 V;
float4x4 P;

//////////////////////
////   TYPES
//////////////////////
struct VertexInputType
{
    float4 position : POSITION;
    float4 color : COLOR;
};

struct PixelInputType
{
    float4 position : SV_POSITION;
    float4 color : COLOR;
};

/////////////////////////////////////
/////   Vertex Shader
/////////////////////////////////////
PixelInputType VS(VertexInputType input)
{
    PixelInputType output;

	// Change the position vector to be 4 units for proper matrix calculations.
    input.position.w = 1.0f;

	// Calculate the position of the vertex against the world, view, and projection matrices.
    output.position = mul(input.position, W);
    output.position = mul(output.position, V);
    output.position = mul(output.position, P);

	// Store the input color for the pixel shader to use.
    output.color = input.color;

    return output;
}
