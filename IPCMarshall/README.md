# IPCMarshall

A simple IPC library for typesafe communication between processes.

## Features

- [x] Typesafe
- [x] Simple API (With events!)
- [x] Shared memory
- [x] Server/Client model

## Theory

This allows for a simple single server many client model. The server will create a shared memory region and the clients
will connect to it. The server will then be able to write to the memory region and the clients will be able to read from
it.

## Limitations
- Strings must be passed By value, todo this, I have created a ValueString struct that can be used instead of a string.
- The struct must be a blittable type, this means that it must be able to be copied to memory without any special
  marshalling. This means that it cannot contain any reference types, or any types that contain reference types.

## Usage

### Server

```csharp

using IPCDebuggerContract;
using IPCMarshall;

var memManager = new IPCMemServer<MyStruct>(nameof(MyStruct), TimeSpan.FromMilliseconds(50));

Console.WriteLine("Hub Starting");

int i = 0;

while (true)
{
    Thread.Sleep(750);
    var a = new MyStruct($"!**{i++}**!");
    var written = memManager.Write(ref a);
    Console.WriteLine($"Written: {written} - {a.StringOne}");
}

```

### Client

```csharp
using IPCDebuggerContract;
using IPCMarshall;

var memClient = new IPCMemClient<MyStruct>(nameof(MyStruct), TimeSpan.FromMilliseconds(50));
memClient.EnableEventRaising = true;

memClient.OnMemoryChanged += (myStruct) => Console.WriteLine($"Event: {myStruct.StringOne}");

Console.WriteLine("Client Started! - press any key to exit");
Console.ReadKey();
```

### Contract
```csharp
using IPCMarshall;

namespace IPCDebuggerContract;

public struct MyStruct
{
    public ValueString StringOne;
    public MyStruct(string one)
    {
        StringOne = new ValueString(one);
    }
}

```
