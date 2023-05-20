using IPCDebuggerContract;
using IPCMarshall;

var memClient = new IPCMemClient<MyStruct>(nameof(MyStruct), new TimeSpan(0, 0, 0, 0, 50));

Console.WriteLine("Client Started!");

while (true)
{
    Thread.Sleep(1000);
    var read = memClient.Read(out var a);
    Console.WriteLine($"Read: {read} - {a.StringOne}");
}


