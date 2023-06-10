namespace IPCMarshall.ValueTypes.ValueAction;

public class ValActionPublisher<TVt> : IPCMemServer<InvocationDataStruct<TVt>> where TVt : struct
{
    private InvocationDataStruct<TVt> _lastData = InvocationDataStruct<TVt>.Start();

    public void Invoke(TVt value)
    {
        _lastData = _lastData.Next(value);
        base.Write(ref _lastData);
    }

    public ValActionPublisher(string name) : base(name, TimeSpan.FromMilliseconds(5))
    {
        base.Write(ref _lastData);
    }
}