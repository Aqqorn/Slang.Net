StructuredBuffer<float4> InputData : register(t0);
RWStructuredBuffer<float4> OutputData : register(u0);

[shader("compute")]
[numthreads(1, 1, 1)]
void CS(uint3 dispatchThreadID : SV_DispatchThreadID)
{
    uint i = dispatchThreadID.x;
    OutputData[i] = InputData[i] * 2.0;
}
