struct VSOutput
{
	[[vk::location(0)]] float3 Normal : NORMAL0;
	[[vk::location(1)]] float2 UV : TEXCOORD0;
};




//Texture2D<float> Texture : register(t1);
Texture2D Texture : register(t1);
SamplerState Sampler : register(s2);


struct LightBuffer
{
	float4 diffuseColor;
	float3 lightDirection;
	float is_texture;
};
ConstantBuffer<LightBuffer> light : register(b3);



float4 main(VSOutput input) : SV_TARGET
{

	float4 texture_color;
	float3 lightDir;
	float lightIntensity;
	float4 color;


	// Sample the pixel color from the texture using the sampler at this texture coordinate location.
	texture_color = Texture.Sample(Sampler, input.UV);

	// Invert the light direction for calculations.
	lightDir = -light.lightDirection;

	// Calculate the amount of light on this pixel.
	lightIntensity = saturate(dot(input.Normal, lightDir));

	// Determine the final amount of diffuse color based on the diffuse color combined with the light intensity.
	color = saturate(light.diffuseColor * lightIntensity);


	[flatten]
	if (light.is_texture == 1)
		color *= texture_color; // Multiply the texture pixel and the final diffuse color to get the final pixel color result.


	return color;



}