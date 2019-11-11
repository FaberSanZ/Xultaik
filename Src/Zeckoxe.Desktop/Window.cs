// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Window.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace Zeckoxe.Desktop
{
    public class Window : Form
    {
       
        public Window(string title, int width, int height)
        {
            //if()
            this.Text = title;
            this.Width = width;
            this.Height = height;
            
        }



        public void RenderLoop(Action render)
        {
            if (this is null)
                throw new ArgumentNullException("Windows");

            if (render is null)
                throw new ArgumentNullException("renderCallback");

            this.Show();

            using var renderLoop = new RenderLoop(this)
            {
                UseApplicationDoEvents = false,
                //AllowWindowssKeys = true
            };

            while (renderLoop.NextFrame())
                render();

        }
    }



}
