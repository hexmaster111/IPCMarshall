using IPCMarshall;
using IPCMarshall.ValueTypes;

namespace IPCDebuggerContract;

public struct MySharedDataStruct
{
    public ValueString StringOne;
    public int Counter;

    public MySharedDataStruct(string one, int counter)
    {
        StringOne = new ValueString(one);
        Counter = counter;
    }
}

public struct MySharedActionDataStruct
{
    public ValueString StringOne;
    public int Counter;

    public MySharedActionDataStruct(string one, int counter)
    {
        StringOne = new ValueString(one);
        Counter = counter;
    }
}