Texture2D<float4> Albedo : register(t0);
SamplerState LinearClamp : register(s0);

struct PSIn
{
    float4 position : SV_Position;
    float2 uv : TEXCOORD0;
};

[shader("fragment")]
float4 FS(PSIn input) : SV_Target
{
    return Albedo.Sample(LinearClamp, input.uv);
}
