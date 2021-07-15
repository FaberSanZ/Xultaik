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
	float4 ambientColor;
	float4 diffuseColor;
	float3 lightDirection;
	float padding;
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


	// Set the default output color to the ambient light value for all pixels.
	color = light.ambientColor;

	// Invert the light direction for calculations.
	lightDir = -light.lightDirection;

	// Calculate the amount of the light on this pixel.
	lightIntensity = saturate(dot(input.Normal, lightDir));

	if (lightIntensity > 0.0f)
	{
		// Determine the final diffuse color based on the diffuse color and the amount of light intensity.
		color += (light.diffuseColor * lightIntensity);
	}

	// Saturate the final light color.
	color = saturate(color);

	// Multiply the texture pixel and the final diffuse color to get the final pixel color result.
	// EX 2: for seeing only the lighting effect.
	color = color * texture_color;


	return color;



}