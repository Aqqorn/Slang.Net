struct VSInput
{
    float3 position : POSITION;
};

struct VSOutput
{
    float4 position : SV_Position;
};

[shader("vertex")]
VSOutput VSMain(VSInput input)
{
    VSOutput output;
    output.position = float4(input.position, 1.0);
    return output;
}

[shader("fragment")]
float4 PSMain(VSOutput input) : SV_Target
{
    return float4(0.2, 0.4, 0.8, 1.0);
}
