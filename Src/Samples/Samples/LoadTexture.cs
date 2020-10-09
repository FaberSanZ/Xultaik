using System;
using System.Collections.Generic;
using System.Text;
using Zeckoxe.Engine;
using Zeckoxe.Graphics;

namespace Samples.Samples
{
    public class LoadTexture : Game, IDisposable
    {
        internal int TextureWidth = 256; //Texture Data
        internal int TextureHeight = 256; //Texture Data
        internal int TexturePixelSize = 4;  // The number of bytes used to represent a pixel in the texture. RGBA

        public LoadTexture() : base()
        {
            Parameters.Settings.Validation = ValidationType.Console | ValidationType.Debug;
            Window.Title += " - (LoadTexture) ";
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void BeginDraw()
        {
            base.BeginDraw();

            Context.CommandBuffer.BeginFramebuffer(Framebuffer, 0.5f, 0.5f, 0.5f);
        }


        internal byte[] GenerateTextureData()
        {
            byte r = 255;
            byte g = 255;
            byte b = 255;
            byte a = 255;

            int color = default;
            color |= r << 24;
            color |= g << 16;
            color |= b << 8;
            color |= a;

            uint _value = (uint)color; // RBGA

            byte Cr = (byte)((_value >> 24) & 0xFF);
            byte Cg = (byte)((_value >> 16) & 0xFF);
            byte Cb = (byte)((_value >> 8) & 0xFF);
            byte Ca = (byte)(_value & 0xFF);


            int rowPitch = TextureWidth * TexturePixelSize;
            int cellPitch = rowPitch >> 3;       // The width of a cell in the checkboard texture.
            int cellHeight = TextureWidth >> 3;  // The height of a cell in the checkerboard texture.
            int textureSize = rowPitch * TextureHeight;
            byte[] data = new byte[textureSize];

            for (int n = 0; n < textureSize; n += TexturePixelSize)
            {
                int x = n % rowPitch;
                int y = n / rowPitch;
                int i = x / cellPitch;
                int j = y / cellHeight;

                if (i % 2 == j % 2)
                {
                    data[n + 0] = Cr; // R
                    data[n + 1] = Cg; // G
                    data[n + 2] = Cb; // B
                    data[n + 3] = Ca; // A
                }
                else
                {
                    data[n + 0] = 0xff; // R
                    data[n + 1] = 0xff; // G
                    data[n + 2] = 0xff; // B
                    data[n + 3] = 0xff; // A
                }
            }

            return data;
        }


        public void Dispose()
        {
        }
    }
}
