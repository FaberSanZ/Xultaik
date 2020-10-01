using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Zeckoxe.Games
{
    public class GameBase
    {
        private readonly ServiceProvider serviceProvider;
        private readonly object tickLock = new object();

        private bool isExiting;
        private readonly Stopwatch stopwatch = new Stopwatch();

        protected GameBase()
        {

        }

        public GameTime Time { get; } = new GameTime();

        public bool IsRunning { get; private set; }

        public void Run()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("This game is already running.");
            }

            IsRunning = true;

            Initialize();
            LoadContentAsync();

            stopwatch.Start();
            Time.Update();

            BeginRun();
        }

        public void Exit()
        {
            if (IsRunning)
            {
                isExiting = true;

                lock (tickLock)
                {
                    CheckEndRun();
                }
            }
        }

        public void Tick()
        {
            lock (tickLock)
            {
                if (isExiting)
                {
                    CheckEndRun();
                    return;
                }

                try
                {
                    Time.Update();

                    Update(Time);

                    BeginDraw();
                    Draw(Time);
                }
                finally
                {
                    EndDraw();

                    CheckEndRun();
                }
            }
        }

        public virtual void Initialize()
        {
        }

        public virtual Task LoadContentAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void BeginRun()
        {
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void BeginDraw()
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }


        public virtual void EndDraw()
        {

        }

        public virtual void EndRun()
        {
        }

        public virtual void ConfigureServices()
        {

        }

        private void CheckEndRun()
        {
            if (isExiting && IsRunning)
            {
                EndRun();

                stopwatch.Stop();

                IsRunning = false;
                isExiting = false;
            }
        }
    }
}
