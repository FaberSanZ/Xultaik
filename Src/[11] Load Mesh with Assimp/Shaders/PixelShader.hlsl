Texture2D shaderTexture;
SamplerState SampleType;


struct PixelInputType
{
	float4 Position : SV_POSITION;
	float2 Tex : TEXCOORD0;
};


float4 PS(PixelInputType input) : SV_TARGET
{
	float4 textureColor;

	// Sample the pixel color from the texture using the sampler at this texture coordinate location.
	textureColor = shaderTexture.Sample(SampleType, input.Tex);

	return textureColor;
}
