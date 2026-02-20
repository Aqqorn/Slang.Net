struct VSIn
{
    float3 position : POSITION;
};

struct VSOut
{
    float4 position : SV_Position;
};

[shader("vertex")]
VSOut VS(VSIn input)
{
    VSOut output;
    output.position = float4(input.position.xy * 0.5, input.position.z, 1.0);
    return output;
}
