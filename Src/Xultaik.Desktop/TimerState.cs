// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using Xultaik.Desktop;
using System.Data;
using System.Diagnostics;

namespace Xultaik.Desktop;

// source https://github.com/Microsoft/DirectXTK/wiki/StepTimer
// Do not remove conversions or use "Convert"
public class TimerState
{
    /// <summary>
    /// Integer format represents time using 10.000.000 ticks per second.
    /// </summary>
    public const ulong TicksPerSecond = 10000000;

    /// <summary>
    /// Frequency cap for Update/RenderFrame events.
    /// </summary>
    public const double MaxFrequency = 500.0;

    private long _qpcFrequency;

    private long _qpcLastTime;

    private ulong _qpcMaxDelta;

    private ulong _leftOverTicks;

    private uint _framesThisSecond;

    private ulong _qpcSecondCounter;

    private ulong _targetElapsedTicks;

    private double _updateFrequecy;


    /// <summary>
    /// Get elapsed time since the previous Update call.
    /// </summary>
    public ulong ElapsedTicks { get; private set; }

    /// <summary>
    /// Get elapsed time since the previous Update call.
    /// </summary>
    public double ElapsedSeconds => TicksToSeconds(ElapsedTicks);

    /// <summary>
    /// Get total time since the start of the program.
    /// </summary>
    public ulong TotalTicks { get; private set; }

    /// <summary>
    /// Get total time since the start of the program.
    /// </summary>
    public double TotalSeconds => TicksToSeconds(TotalTicks);

    /// <summary>
    /// Get total number of updates since start of the program.
    /// </summary>
    public ulong FrameCount { get; private set; }

    /// <summary>
    /// Get the current framerate.
    /// </summary>
    public uint FramesPerSecond { get; private set; }

    /// <summary>
    /// Gets a value indicating whether or not the time is in fixed mode.
    /// </summary>
    public bool IsFixedTimeStep { get; private set; }


    /// <summary>
    /// Interrupt elapsed time.
    /// </summary>
    public bool Stop { get; set; }


    public double UpdateFrequency
    {
        get => _updateFrequecy;
        set
        {
            double frequency = value;
            if (frequency <= 1.0)
            {
                IsFixedTimeStep = false;
                _updateFrequecy = 0.0;
            }
            else if (frequency <= MaxFrequency)
            {
                IsFixedTimeStep = true;
                _targetElapsedTicks = SecondsToTicks(1.0 / frequency);
                _updateFrequecy = frequency;
            }
            else
            {
                Debug.Print("Target render frequency clamped to {0}Hz.", MaxFrequency);
                IsFixedTimeStep = true;
                _targetElapsedTicks = SecondsToTicks(1.0 / MaxFrequency);
                _updateFrequecy = MaxFrequency;
            }
        }
    }

    public TimerState()
    {
        ElapsedTicks = 0;
        TotalTicks = 0;
        _leftOverTicks = 0;
        FrameCount = 0;
        FramesPerSecond = 0;
        _framesThisSecond = 0;
        _qpcSecondCounter = 0;
        IsFixedTimeStep = false;
        _targetElapsedTicks = TicksPerSecond / 60;

        _qpcFrequency = Stopwatch.Frequency;
        _qpcLastTime = Stopwatch.GetTimestamp();

        // Initialize max delta to 1/10 of a second.
        _qpcMaxDelta = (ulong)(_qpcFrequency / 10);
    }

    // After an intentional timing discontinuity (for instance a blocking IO operation)
    // call this to avoid having the fixed timestep logic attempt a set of catch-up
    // Update calls.
    public void Reset()
    {
        _qpcLastTime = Stopwatch.GetTimestamp();

        _leftOverTicks = 0;
        FramesPerSecond = 0;
        _framesThisSecond = 0;
        _qpcSecondCounter = 0;
    }

    // Update timer state, calling the specified Update function the appropriate number of times.
    public void NewFrame()
    {
        if (Stop)
        {
            return;
        }

        // Query the current time.
        long currentTime = Stopwatch.GetTimestamp();
        ulong timeDelta = (ulong)(currentTime - _qpcLastTime);

        _qpcLastTime = currentTime;
        _qpcSecondCounter += timeDelta;

        // Clamp excessively large time deltas (e.g. after paused in the debugger).
        if (timeDelta > _qpcMaxDelta)
        {
            timeDelta = _qpcMaxDelta;
        }

        // Convert QPC units into a canonical tick format. This cannot overflow due to the previous clamp.
        timeDelta *= TicksPerSecond;
        timeDelta /= (ulong)_qpcFrequency;

        ulong lastFrameCount = FrameCount;

        if (IsFixedTimeStep)
        {
            // Fixed timestep update logic
            // If the app is running very close to the target elapsed time (within 1/4 of a millisecond) just clamp
            // the clock to exactly match the target value. This prevents tiny and irrelevant errors
            // from accumulating over time. Without this clamping, a game that requested a 60 fps
            // fixed update, running with vsync enabled on a 59.94 NTSC display, would eventually
            // accumulate enough tiny errors that it would drop a frame. It is better to just round
            // small deviations down to zero to leave things running smoothly.

            if (Math.Abs((long)(timeDelta - _targetElapsedTicks)) < (long)(TicksPerSecond / 4000))
            {
                timeDelta = _targetElapsedTicks;
            }

            _leftOverTicks += timeDelta;

            while (_leftOverTicks >= _targetElapsedTicks)
            {
                ElapsedTicks = _targetElapsedTicks;
                TotalTicks += _targetElapsedTicks;
                FrameCount++;

                _leftOverTicks -= _targetElapsedTicks;
            }
        }
        else
        {
            // Variable timestep update logic.
            ElapsedTicks = timeDelta;
            TotalTicks += timeDelta;
            FrameCount++;

            _leftOverTicks = 0;
        }

        // Track the current framerate.
        if (FrameCount != lastFrameCount)
        {
            _framesThisSecond++;
        }

        if (_qpcSecondCounter >= (ulong)_qpcFrequency)
        {
            FramesPerSecond = _framesThisSecond;
            _framesThisSecond = 0;
            _qpcSecondCounter %= (ulong)_qpcFrequency;
        }
    }

    private static double TicksToSeconds(ulong ticks)
    {
        return (double)ticks / TicksPerSecond;
    }

    private static ulong SecondsToTicks(double seconds)
    {
        return (uint)(seconds * TicksPerSecond);
    }

    public static implicit operator FrameEventArgs(TimerState state)
    {
        return new FrameEventArgs(
                state.ElapsedTicks,
                state.ElapsedSeconds,
                state.TotalTicks,
                state.TotalSeconds,
                state.FrameCount,
                state.FramesPerSecond);
    }
}
