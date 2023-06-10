namespace IPCMarshall.ValueTypes.ValueAction;

public class ValActionSubscriber<TVt> : IPCMemClient<InvocationDataStruct<TVt>> where TVt : struct
{
    public event Action<TVt>? Invoked;

    public ValActionSubscriber(string name) : base(name, TimeSpan.FromMilliseconds(5))
    {
        MemoryChanged += OnMemoryChanged;
        EnableEventRaising = true;
    }

    private void OnMemoryChanged(InvocationDataStruct<TVt> obj) => Invoked?.Invoke(obj.ValueTransmitted);
}

public class ValActionSubscriber : ValActionSubscriber<EmptyStruct>
{
    public ValActionSubscriber(string name) : base(name)
    {
    }
}