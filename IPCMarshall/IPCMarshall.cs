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

    internal IPCMarshall(string structMapName, TimeSpan accessTimeOut)
    {
        var mapName = $"{structMapName}.{nameof(IPCMarshall<T>)}.mmf.ipc.bin";
        var semaphoreName = $"{structMapName}.{nameof(IPCMarshall<T>)}.mmf.ipc.semaphore";
        MemoryMappedFile = MemoryMappedFile.CreateOrOpen(mapName, MemSize);
        AccessTimeOut = accessTimeOut;
        MemMapSemaphore = new Semaphore(1, 1, semaphoreName, out var isNew);
    }
}