// shaders.hlsl
struct VS2PS
{
    float4 position : SV_Position;
    float4 color : COLOR;
};

VS2PS VS(float3 position : POSITION, float4 color : COLOR)
{
    VS2PS vs2ps;
    vs2ps.position = float4(position, 1.0);
    vs2ps.color = color;
    return vs2ps;
}

float4 PS(VS2PS vs2ps) : SV_Target
{
    return vs2ps.color;
}