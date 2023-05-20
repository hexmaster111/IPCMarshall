using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace IPCMarshall;

public class IPCMarshall<T> where T : struct
{
    /// <summary>
    ///     The memory layout will match this Mem size, [Struct][UInt16] where the UInt16 is a bit field.
    /// </summary>
    private static readonly int MemSize = Marshal.SizeOf<T>() + sizeof(UInt16);

    protected static readonly int BitFieldOffset = Marshal.SizeOf<T>();

    protected readonly MemoryMappedFile MemoryMappedFile;
    protected readonly Semaphore MemMapSemaphore;
    protected readonly TimeSpan AccessTimeOut;
    private readonly string _structMapName;
    private readonly string _semaphoreName;

    internal IPCMarshall(string structMapName, TimeSpan accessTimeOut)
    {
        _structMapName = $"{structMapName}.{nameof(IPCMarshall<T>)}.mmf.ipc.bin";
        _semaphoreName = $"{structMapName}.{nameof(IPCMarshall<T>)}.mmf.ipc.semaphore";
        MemoryMappedFile = MemoryMappedFile.CreateOrOpen(_structMapName, MemSize);
        AccessTimeOut = accessTimeOut;
        MemMapSemaphore = new Semaphore(1, 1, _semaphoreName, out var isNew);
    }
}