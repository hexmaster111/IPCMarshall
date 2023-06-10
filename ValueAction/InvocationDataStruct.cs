namespace IPCMarshall.ValueTypes.ValueAction;

public readonly struct InvocationDataStruct<TVt> where TVt : struct
{
    public static InvocationDataStruct<TVt> Start() => new(0, default);
    public InvocationDataStruct<TVt> Next(TVt value) => new(InvocationCount + 1, value);

    private InvocationDataStruct(ulong invocationCount, TVt valueTransmitted)
    {
        InvocationCount = invocationCount;
        ValueTransmitted = valueTransmitted;
    }

    public UInt64 InvocationCount { get; }
    public TVt ValueTransmitted { get; }
}