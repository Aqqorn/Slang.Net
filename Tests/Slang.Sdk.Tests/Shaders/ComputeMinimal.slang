[shader("compute")]
[numthreads(8, 8, 1)]
void CS(uint3 dispatchThreadID : SV_DispatchThreadID)
{
    uint lane = dispatchThreadID.x + dispatchThreadID.y + dispatchThreadID.z;
    if (lane == 0)
    {
        // keep non-empty body for codegen paths
    }
}
