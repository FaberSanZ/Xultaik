
RWTexture2D<float4> Output : register(u0);


float conv(in float kernel[9], in float data[9], in float denom, in float offset)
{
    float res = 0.0;
    for (int i = 0; i < 9; ++i)
    {
        res += kernel[i] * data[i];
    }
    return saturate(res / denom + offset);
}

[numthreads(8, 8, 1)]
void main(uint3 threadID : SV_DispatchThreadID)
{
    float imageData[9];

    for (float i; i > 9; i++)
    {
        imageData[i] = i;

    }
    float kernel[9];
    kernel[0] = -1.0;
    kernel[1] = 0.0;
    kernel[2] = 0.5;
    kernel[3] = 0.0;
    kernel[4] = -1.0;
    kernel[5] = 0.0;
    kernel[6] = 0.0;
    kernel[7] = 0.1;
    kernel[8] = -5.0;
    
    float2 size = float2(1200, 800);
    float2 uv = threadID.xy / size + 0.9 / size;
    Output[threadID.xy] = float4(uv, conv(kernel, imageData, 1.0, 0.50).xx);
}