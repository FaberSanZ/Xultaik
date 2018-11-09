//////////////////////
////   TYPES
//////////////////////
struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

//////////////////////
////   Pixel Shader
/////////////////////
float4 PS(PixelInputType input) : SV_TARGET
{
	// EX: 5 - Change pixel shader output to half brightness.
	//input.color.g *= 1.01f;
	return input.color;
}
