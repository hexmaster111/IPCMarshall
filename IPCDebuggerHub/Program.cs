// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using IPCDebuggerContract;
using IPCMarshall;
using Microsoft.Extensions.Primitives;

var memManager = new IPCMemServer<MyStruct>(nameof(MyStruct), new TimeSpan(0, 0, 0, 0, 50));

Console.WriteLine("Hello world");

int i = 0;

while (true)
{
    Thread.Sleep(750);
    var a = new MyStruct($"!**{i++}**!");
    var written = memManager.Write(ref a);
    Console.WriteLine($"Written: {written} - {a.StringOne}");
}


