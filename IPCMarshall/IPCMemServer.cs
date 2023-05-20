using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Timers;
using Timer = System.Timers.Timer;

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

/// <summary>
///     An IPC (inner process communication) server that writes to a memory mapped file. Safe for multiple processes.
/// </summary>
/// <typeparam name="T">The DTO structure to send to the <see cref="IPCMemClient{T}"/></typeparam>
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

    private Timer? _checkTimer;
    private bool _enableEventRaising;
    private T? _lastReadStructFromCheckTimer;
    private TimeSpan _eventRaisingCheckInterval = TimeSpan.FromSeconds(1);
    
    
    /// <summary>
    ///     To enable this, set <see cref="EnableEventRaising" /> to true.
    ///     Note: This event is raised on a different thread.
    /// </summary>
    public event Action<T>? OnMemoryChanged;
    
    public bool EnableEventRaising
    {
        get => _enableEventRaising;
        set
        {
            _enableEventRaising = value;
            if (!value) return;
            _checkTimer ??= new Timer();
            _checkTimer.Interval = _eventRaisingCheckInterval.TotalMilliseconds;
            _checkTimer.Elapsed += CheckAndAlertChange;
        }
    }
    
    
    private void CheckAndAlertChange(object? sender, ElapsedEventArgs args)
    {
        if (!Read(out var @struct)) return;
        if (_lastReadStructFromCheckTimer == null)
        {
            _lastReadStructFromCheckTimer = @struct;
            return;
        }

        if (_lastReadStructFromCheckTimer.Equals(@struct)) return;
        _lastReadStructFromCheckTimer = @struct;
        OnMemoryChanged?.Invoke(@struct);
    }


    public TimeSpan EventRaisingCheckInterval
    {
        get => _eventRaisingCheckInterval;
        set
        {
            _eventRaisingCheckInterval = value;
            if (_checkTimer == null) return; // Timer interval will be set when EnableEventRaising is set to true.
            _checkTimer.Interval = _eventRaisingCheckInterval.TotalMilliseconds;
        }
    }


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