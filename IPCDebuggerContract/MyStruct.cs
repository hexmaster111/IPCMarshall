using IPCMarshall;
using IPCMarshall.ValueTypes;

namespace IPCDebuggerContract;

public struct MyStruct
{
    public ValueString StringOne;
    public int Counter;
    public MyStruct(string one, int counter)
    {
        StringOne = new ValueString(one);
        Counter = counter;
    }
}

