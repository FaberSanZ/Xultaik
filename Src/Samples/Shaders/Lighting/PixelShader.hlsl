//////////////////////
////   GLOBALS
//////////////////////
Texture2D shaderTexture : register(t1, space1);
SamplerState SampleType : register(t1, space1);

cbuffer LightBuffer : register(b2)
{
	float4 diffuseColor;
	float3 lightDirection;
	float padding;
};

//////////////////////
////   TYPES
//////////////////////
struct PixelInputType
{
	[[vk::location(0)]]  float4 position : SV_POSITION;
	[[vk::location(1)]]  float3 normal : NORMAL;
	[[vk::location(2)]]  float2 tex : TEXCOORD0;
};

//////////////////////
////   Pixel Shader
/////////////////////
float4 main(PixelInputType input) : SV_TARGET
{
	float4 textureColor;
	float3 lightDir;
	float lightIntensity;
	float4 color;

	// Sample the pixel color from the texture using the sampler at this texture coordinate location.
	textureColor = shaderTexture.Sample(SampleType, input.tex);

	// Invert the light direction for calculations.
	lightDir = -lightDirection;

	// Calculate the amount of the light on this pixel.
	lightIntensity = saturate(dot(input.normal, lightDir));

	// Determine the final amount of diffuse color based on the diffuse color combined with the light intensity.
	color = saturate(diffuseColor * lightIntensity);

	// Multiply the texture pixel and the final diffuse color to get the final pixel color result.
	// EX 2: Render pixels wihtout a texture.
	color = color * textureColor;

	return color;
}
