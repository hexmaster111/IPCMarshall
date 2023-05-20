using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace IPCMarshall;

public class IPCMarshall<T> where T : struct
{
    private static readonly int Size = Marshal.SizeOf<T>();
    protected readonly MemoryMappedFile _memoryMappedFile;
    protected readonly Semaphore _semaphore;
    protected readonly TimeSpan _timeout;

    internal IPCMarshall(string structMapName, TimeSpan timeout)
    {
        string name = $"{structMapName}.{nameof(IPCMarshall<T>)}.mmf.ipc.bin";
        _memoryMappedFile = MemoryMappedFile.CreateOrOpen(name, Size);
        _timeout = timeout;
        _semaphore = new Semaphore(1, 1, $"{structMapName}.{nameof(IPCMarshall<T>)}.mmf.ipc.semaphore", out var isNew);
    }
}

public class IPCMemServer<T> : IPCMarshall<T> where T : struct
{
    public bool Write(ref T @struct)
    {
        if (!_semaphore.WaitOne(_timeout)) return false; // failed to get the semaphore in the given timeout
        using var accessor = _memoryMappedFile.CreateViewAccessor();
        accessor.Write(0, ref @struct); //Getting an exception here, Using a string in the struct? not ok., By val only.
        _semaphore.Release();
        return true;
    }

    public IPCMemServer(string structMapName, TimeSpan timeout) : base(structMapName, timeout)
    {
    }
}

public class IPCMemClient<T> : IPCMarshall<T> where T : struct
{
    public bool Read(out T @struct)
    {
        if (!_semaphore.WaitOne(_timeout))
        {
            @struct = default;
            return false;
        }

        using var accessor = _memoryMappedFile.CreateViewAccessor();
        accessor.Read(0, out @struct);
        _semaphore.Release();
        return true;
    }

    public IPCMemClient(string structMapName, TimeSpan timeout) : base(structMapName, timeout)
    {
    }
}