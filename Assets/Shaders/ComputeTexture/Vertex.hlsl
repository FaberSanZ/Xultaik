

struct VSInput
{
    [[vk::location(0)]] float3 Pos : SV_POSITION;
	[[vk::location(1)]] float2 UV : TEXCOORD0;
};



struct VSOutput
{
    float4 Pos : SV_POSITION;
	[[vk::location(0)]] float2 UV : TEXCOORD0;
};

VSOutput main(uint id : SV_VertexID)
{
	VSOutput output = (VSOutput)0;
    VSInput input = (VSInput) 0;
	
	
    input.UV = float2((id << 1) & 2, id & 2);
    input.Pos = float4(input.UV * float2(2, -2) + float2(-1, 1), 0, 1);
    
    output.UV = float2((id << 1) & 2, id & 2);
    output.Pos = float4(input.UV * float2(2, -2) + float2(-1, 1), 0, 1);

	return output;

}