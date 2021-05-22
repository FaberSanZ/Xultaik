using System;
using System.Diagnostics;

namespace Vultaik.Toolkit
{
    public class TimerTick
    {
        private long startRawTime;
        private long lastRawTime;
        private int pauseCount;
        private long pauseStartTime;
        private long timePaused;

        public TimerTick()
        {
            Reset();
        }


        public TimeSpan TotalTime { get; private set; }

        public TimeSpan ElapsedAdjustedTime { get; private set; }

        public TimeSpan ElapsedTime { get; private set; }

        public bool IsPaused => pauseCount > 0;


        public void Reset()
        {
            TotalTime = TimeSpan.Zero;
            startRawTime = Stopwatch.GetTimestamp();
            lastRawTime = startRawTime;
        }

        public void Resume()
        {
            pauseCount--;
            if (pauseCount <= 0)
            {
                timePaused += Stopwatch.GetTimestamp() - pauseStartTime;
                pauseStartTime = 0L;
            }
        }

        public void Tick()
        {
            // Don't tick when this instance is paused.
            if (IsPaused)
            {
                return;
            }

            long rawTime = Stopwatch.GetTimestamp();
            TotalTime = ConvertRawToTimestamp(rawTime - startRawTime);
            ElapsedTime = ConvertRawToTimestamp(rawTime - lastRawTime);
            ElapsedAdjustedTime = ConvertRawToTimestamp(rawTime - (lastRawTime + timePaused));

            if (ElapsedAdjustedTime < TimeSpan.Zero)
            {
                ElapsedAdjustedTime = TimeSpan.Zero;
            }

            timePaused = 0;
            lastRawTime = rawTime;
        }


        public void Pause()
        {
            pauseCount++;
            if (pauseCount == 1)
            {
                pauseStartTime = Stopwatch.GetTimestamp();
            }
        }


        private static TimeSpan ConvertRawToTimestamp(long delta) => TimeSpan.FromTicks((delta * 10000000) / Stopwatch.Frequency);


    }
}