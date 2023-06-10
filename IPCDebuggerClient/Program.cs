using IPCDebuggerContract;
using IPCMarshall;
using IPCMarshall.ValueTypes.IPAction;

var memClient = new IPCMemClient<MySharedDataStruct>($"x{nameof(MySharedDataStruct)}x", TimeSpan.FromMilliseconds(50))
{
    EnableEventRaising = true
};


var lastCount = 0;
var missCount = 0;
memClient.MemoryChanged += (myStruct) =>
{
    Console.WriteLine($"Event: {myStruct.Counter} - {myStruct.StringOne}");
    if (myStruct.Counter != lastCount + 1)
    {
        Console.WriteLine($"Counter skipped from {lastCount} to {myStruct.Counter}");
        missCount++;
    }

    lastCount = myStruct.Counter;
};

var publisher = new ValActionPublisher<MySharedActionDataStruct>(nameof(MySharedActionDataStruct));
var i = 0;
var callback = new TimerCallback((_) => { publisher.Invoke(new MySharedActionDataStruct("Action!", i++)); });
var tmr = new Timer(callback);

tmr.Change(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));

Console.WriteLine("Client Started! - press any key to exit");
Console.ReadKey();
Console.WriteLine($"Missed {missCount} events");