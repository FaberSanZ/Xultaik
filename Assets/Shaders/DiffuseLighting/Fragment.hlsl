
Texture2D shaderTexture : register(t1);

SamplerState samplerColor : register(s2);


struct LightBuffer
{
	float4 diffuseColor;
	float3 lightDirection;
	float padding;
};


cbuffer light : register(b3) { LightBuffer light; }



struct VSOutput
{
	[[vk::location(0)]] float3 Normal : NORMAL0;
	[[vk::location(1)]] float2 UV : TEXCOORD0;
};

float4 main(VSOutput input) : SV_TARGET
{

	float4 textureColor;
	float3 lightDir;
	float lightIntensity;
	float4 color;


	// Sample the pixel color from the texture using the sampler at this texture coordinate location.
	textureColor = shaderTexture.Sample(samplerColor, input.UV);

	// Invert the light direction for calculations.
	lightDir = -light.lightDirection;

	// Calculate the amount of light on this pixel.
	lightIntensity = saturate(dot(input.Normal, lightDir));

	// Determine the final amount of diffuse color based on the diffuse color combined with the light intensity.
	color = saturate(light.diffuseColor * lightIntensity);

	// Multiply the texture pixel and the final diffuse color to get the final pixel color result.
	color *= textureColor;

	return color;



}