using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows
{
    public class Window
    {
        public Form Form { get; set; } 

        public Timer Timer { get; } = new Timer();

        public string Title { get; set; } 

        public int Width { get; set; }
        public int Height { get; set; }

        public bool AppPaused { get; set; }
        public bool Running { get; set; }

        public bool Minimized { get; set; }
        public bool Maximized { get; set; }

        private FormWindowState lastWindowState = FormWindowState.Normal;

        public delegate void Render();

        public delegate void Update();



        public Window() : this ("First Window", 800, 600) { }

        public Window(string Title, int Width, int Height)
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


                if (Form.WindowState != lastWindowState)
                {
                    lastWindowState = Form.WindowState;

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

        public void Run(Action Render)
        {
            Timer.Reset();
            while (Running)
            {
                Application.DoEvents();
                Timer.Tick();
                if (AppPaused != true)
                {
                    Render();
                }
                else
                {
                    Thread.Sleep(300);
                }
            }
        }

    }
}
