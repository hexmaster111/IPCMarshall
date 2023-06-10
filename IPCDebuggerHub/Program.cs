using IPCDebuggerContract;
using IPCMarshall;
using IPCMarshall.ValueTypes.ValueAction;

var memManager = new IPCMemServer<MySharedDataStruct>($"x{nameof(MySharedDataStruct)}x", TimeSpan.FromMilliseconds(50));

Console.WriteLine("Hub Starting");
var subscriber = new ValActionSubscriber<MySharedActionDataStruct>(nameof(MySharedActionDataStruct));

subscriber.Invoked += (s) => { Console.WriteLine($"{s.StringOne} - {s.Counter}"); };

var i = 0;
var toggle = false;
while (true)
{
    Thread.Sleep(1000);
    toggle = !toggle;
    var a = new MySharedDataStruct($"Ima string!", i++);
    var written = memManager.Write(ref a);
    Console.WriteLine($"Written: {written} - {a.Counter} - {a.StringOne}");
}