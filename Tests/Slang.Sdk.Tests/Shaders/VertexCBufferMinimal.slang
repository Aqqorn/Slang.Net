cbuffer PerFrame : register(b0)
{
    float4x4 ModelViewProjection;
};

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
    output.position = mul(ModelViewProjection, float4(input.position, 1.0));
    return output;
}
