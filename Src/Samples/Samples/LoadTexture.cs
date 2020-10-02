using System;
using System.Collections.Generic;
using System.Text;
using Zeckoxe.Engine;

namespace Samples.Samples
{
    public class LoadTexture : Game, IDisposable
    {
        public LoadTexture() : base()
        {
            Parameters.Settings.Validation = false;
            Window.Title += " - (LoadTexture) ";
        }
        public override void BeginDraw()
        {
            base.BeginDraw();

            Context.CommandBuffer.BeginFramebuffer(Framebuffer, 0.5f, 0.5f, 0.5f);
        }


        public void Dispose()
        {
        }
    }
}
