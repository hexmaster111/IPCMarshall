// See https://aka.ms/new-console-template for more information

using IPCDebuggerContract;
using IPCMarshall;

var memManager = new IPCMemServer<MyStruct>(nameof(MyStruct), TimeSpan.FromMilliseconds(50));

Console.WriteLine("Hub Starting");

int i = 0;

while (true)
{
    Thread.Sleep(25);
    var a = new MyStruct($"Ima string!", i++);
    var written = memManager.Write(ref a);
    Console.WriteLine($"Written: {written} - {a.Counter} - {a.StringOne}");
}


