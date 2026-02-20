struct VSIn
{
    float3 position : POSITION;
};

struct VSOut
{
    float4 position : SV_Position;
};

[shader("vertex")]
VSOut VSMain(VSIn input)
{
    VSOut output;
    output.position = float4(input.position, 1.0);
    return output;
}

[shader("fragment")]
float4 PSRed() : SV_Target
{
    return float4(1.0, 0.0, 0.0, 1.0);
}

[shader("fragment")]
float4 PSGreen() : SV_Target
{
    return float4(0.0, 1.0, 0.0, 1.0);
}
