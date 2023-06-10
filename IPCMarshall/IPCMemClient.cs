using System.Timers;
using Timer = System.Timers.Timer;

namespace IPCMarshall;

public class IPCMemClient<T> : IPCMarshall<T> where T : struct
{
    private Timer? _checkTimer;
    private bool _enableEventRaising;
    private TimeSpan _eventRaisingCheckInterval = TimeSpan.FromMilliseconds(1);
    private UInt16 _lastMemMapNoticeBitField = 0;


    /// <summary>
    ///     To enable this, set <see cref="EnableEventRaising" /> to true.
    ///     Rapid changes may be missed, but the last change will be raised.
    /// </summary>
    public event Action<T>? MemoryChanged;

    public bool EnableEventRaising
    {
        get => _enableEventRaising;
        set
        {
            if (_enableEventRaising == value) return; // No change.
            _enableEventRaising = value;
            if (!value) return;
            _checkTimer ??= new Timer();
            _checkTimer.Interval = _eventRaisingCheckInterval.TotalMilliseconds;
            _checkTimer.Elapsed += CheckAndAlertChange;
            _checkTimer.Start();
        }
    }


    private void CheckAndAlertChange(object? sender, ElapsedEventArgs args)
    {
        if (!Read(out var @struct, out var indicator)) return;
        if (indicator == _lastMemMapNoticeBitField) return;
        _lastMemMapNoticeBitField = indicator;
        MemoryChanged?.Invoke(@struct);
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


    /// <summary>
    ///     Reads from the memory pool
    /// </summary>
    /// <param name="struct">the T that has been written by the server</param>
    /// <param name="writeIndicator">write indicator, if success, should never be 0, the server shifts the bit every write</param>
    /// <returns>True on success, false if lock could not be taken, -OR- if the server has never written to the mem file</returns>
    public bool Read(out T @struct, out UInt16 writeIndicator)
    {
        if (!MemMapSemaphore.WaitOne(AccessTimeOut))
        {
            @struct = default;
            writeIndicator = 0;
            return false;
        }

        using var accessor = MemoryMappedFile.CreateViewAccessor();
        accessor.Read(0, out @struct);
        accessor.Read(BitFieldOffset, out writeIndicator);
        if (writeIndicator == 0)
        {
            MemMapSemaphore.Release();
            return false;
        }
        MemMapSemaphore.Release();
        return true;
    }
    
    public IPCMemClient(string structMapName, TimeSpan accessTimeOut) : base(structMapName, accessTimeOut)
    {
    }
}