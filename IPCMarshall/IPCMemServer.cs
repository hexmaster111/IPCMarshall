namespace IPCMarshall;

/// <summary>
///     An IPC (inner process communication) server that writes to a memory mapped file. Safe for multiple processes.
/// </summary>
/// <typeparam name="T">The DTO structure to send to the <see cref="IPCMemClient{T}"/></typeparam>
public class IPCMemServer<T> : IPCMarshall<T> where T : struct
{
    public bool Write(ref T @struct)
    {
        if (!MemMapSemaphore.WaitOne(AccessTimeOut)) return false; // failed to get the semaphore in the given timeout
        using var accessor = MemoryMappedFile.CreateViewAccessor();
        accessor.Write(0, ref @struct); //Getting an exception here, Using a string in the struct? not ok., By val only.
        var bitField = accessor.ReadUInt16(BitFieldOffset);
        //bitshift left, if we hit the end, reset to 0.
        bitField = (UInt16)((bitField << 1) | (bitField > 0b1000_0000_0000_0000 ? 1 : 0));
        if (bitField == 0) bitField = 1; // 0 is reserved for "never written", correct to 1
        accessor.Write(BitFieldOffset, bitField);
        MemMapSemaphore.Release();
        return true;
    }

    public IPCMemServer(string structMapName, TimeSpan accessTimeOut) : base(structMapName, accessTimeOut)
    {
    }
}