// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. Faber.reach@gmail.com

/*===================================================================================
	GameTime.cs
====================================================================================*/



using System;

namespace Zeckoxe.Core
{
    public class GameTime
    {

        private readonly TimerTick watch = new TimerTick();


        public GameTime()
        {
            watch.Reset();
        }

        public TimeSpan ElapsedTime => watch.ElapsedAdjustedTime;

        public TimeSpan TotalTime => watch.TotalTime;

        public float ElapsedSeconds => (float)ElapsedTime.TotalSeconds;

        public float TotalSeconds => (float)TotalTime.TotalSeconds;

        public float ElapsedMilliseconds => (float)ElapsedTime.TotalMilliseconds;

        public float TotalMilliseconds => (float)TotalTime.TotalMilliseconds;

        public long Ticks => TotalTime.Ticks;




        public void Start() => watch.Reset();


        public void Reset() => watch.Reset();


        public void Pause() => watch.Pause();


        public void Resume() => watch.Resume();


        public void Update() => watch.Tick();

    }
}
