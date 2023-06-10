using IPCDebuggerContract;
using IPCMarshall;

var memClient = new IPCMemClient<MyStruct>(nameof(MyStruct), TimeSpan.FromMilliseconds(50));
memClient.EnableEventRaising = true;

int lastCount = 0;
int missCount = 0;
memClient.OnMemoryChanged += (myStruct) =>
{
    Console.WriteLine($"Event: {myStruct.Counter} - {myStruct.StringOne}");
    if (myStruct.Counter != lastCount + 1)
    {
        Console.WriteLine($"Counter skipped from {lastCount} to {myStruct.Counter}");
        missCount++;
    }
    lastCount = myStruct.Counter;
    
};

Console.WriteLine("Client Started! - press any key to exit");
Console.ReadKey();
Console.WriteLine($"Missed {missCount} events");

