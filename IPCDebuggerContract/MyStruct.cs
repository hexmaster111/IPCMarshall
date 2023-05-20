using IPCMarshall;

namespace IPCDebuggerContract;

public struct MyStruct
{
    public ValueString StringOne;
    public MyStruct(string one)
    {
        if (one == null) throw new ArgumentNullException(nameof(one));
        if (one.Length > ValueString.MaxLen) throw new ArgumentException("String too long", nameof(one));
        StringOne = new ValueString(one);
    }
}