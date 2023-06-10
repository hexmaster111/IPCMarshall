using IPCMarshall;
using IPCMarshall.ValueTypes.ValueAction;

namespace ValueActions;

public class ValActionPublisher<TVt> : IPCMemServer<InvocationDataStruct<TVt>> where TVt : struct
{
    private InvocationDataStruct<TVt> _lastData = InvocationDataStruct<TVt>.Start();

    public void Invoke(TVt value)
    {
        _lastData = _lastData.Next(value);
        Write(ref _lastData);
    }

    public ValActionPublisher(string name) : base(name, TimeSpan.FromMilliseconds(5)) => Write(ref _lastData);
}

public class ValActionPublisher : ValActionPublisher<EmptyStruct>
{
    public ValActionPublisher(string name) : base(name)
    {
    }
}