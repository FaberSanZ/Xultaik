using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _08__Transformations
{
    public class Windows
    {
        public Form Form { get; set; }    // Windows

        public Timer Timer { get; } = new Timer();

        public string Title { get; set; } // Titulo de la ventana

        public int Width { get; set; }
        public int Height { get; set; }

        public bool AppPaused { get; set; }
        public bool Running { get; set; }

        public bool Minimized { get; set; }
        public bool Maximized { get; set; }

        private int FrameCount { get; set; }
        private float TimeElapsed { get; set; }

        private FormWindowState _lastWindowState = FormWindowState.Normal;

        public delegate void Render();
        public delegate void Update();



        public Windows() : this ("First Window", 720, 510) { }

        public Windows(string Title, int Width, int Height)
        {
            this.Title = Title;
            this.Width = Width;
            this.Height = Height;
        }

        public void Initialize()
        {
            Form = new Form();
            Form.Text = Title;
            Form.ClientSize = new System.Drawing.Size(Width, Height);
            Form.StartPosition = FormStartPosition.CenterScreen;

            Form.ResizeBegin += (sender, e) =>
            {
                AppPaused = true;
                Timer.Stop();
            };

            Form.ResizeEnd += (sender, e) =>
            {
                AppPaused = false;
                Timer.Start();
            };

            Form.Activated += (sender, e) =>
            {
                AppPaused = false;
                Timer.Start();

            };

            Form.Deactivate += (sender, e) =>
            {
                AppPaused = true;
                Timer.Stop();

            };

            Form.HandleDestroyed += (sender, e) => Running = false;

            Form.Resize += (sender, e) =>
            {

                Width = Form.ClientSize.Width;
                Height = Form.ClientSize.Height;


                if (Form.WindowState != _lastWindowState)
                {
                    _lastWindowState = Form.WindowState;

                    if (Form.WindowState == FormWindowState.Maximized)
                    {
                        AppPaused = false;
                        Minimized = false;
                        Maximized = true;

                    }
                    else if (Form.WindowState == FormWindowState.Minimized)
                    {
                        AppPaused = true;
                        Minimized = true;
                        Maximized = false;
                    }

                    else if (Form.WindowState == FormWindowState.Normal)
                    {
                        if (Minimized) // Restoring from minimized state?
                        {
                            AppPaused = false;
                            Minimized = false;
                        }
                        else if (Maximized) // Restoring from maximized state?
                        {
                            AppPaused = false;
                            Maximized = false;
                        }
                    }
                };
            };

            Form.Show();

            Form.Update();

            Running = true;
        }

        public void Run(Render render, Update update)
        {
            Timer.Reset();
            while (Running)
            {
                Application.DoEvents();
                Timer.Tick();
                if (AppPaused != true)
                {
                    update();
                    render();
                }
                else
                {
                    Thread.Sleep(300);
                }
            }
        }

    }
}
