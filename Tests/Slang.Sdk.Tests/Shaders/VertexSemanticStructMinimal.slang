struct VSIn
{
    float3 position : POSITION;
    float4 color : COLOR0;
};

struct VSOut
{
    float4 position : SV_Position;
    float4 tint : COLOR0;
};

[shader("vertex")]
VSOut VS(VSIn input)
{
    VSOut output;
    output.position = float4(input.position, 1.0);
    output.tint = input.color;
    return output;
}
