float4 PS() : SV_TARGET
{
	return float4(1.0f, 0.2f, 0.0f, 1.0f);
}

float4 VS(float4 inPos : POSITION) : SV_POSITION
{
	return inPos;
}