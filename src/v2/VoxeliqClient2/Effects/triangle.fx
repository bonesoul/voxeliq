float4 VShader(float4 position : POSITION) : SV_POSITION
{
	return position;
}

float4 PShader(float4 position : SV_POSITION) : SV_Target
{
	return float4(1.0f, 1.0f, 0.0f, 1.0f);
}