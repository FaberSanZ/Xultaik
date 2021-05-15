// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	IMGLoader.cs
=============================================================================*/




using System;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using Vortice.Vulkan;

namespace Vultaik
{

    internal unsafe class IMGLoader : IDisposable
    {
        private Image<Rgba32> _image;


        public IMGLoader(byte[] data)
        {
            _image = SixLabors.ImageSharp.Image.Load<Rgba32>(data);
            Span<Rgba32> pixels = _image.GetPixelSpan();

            for (int i = 0; i < pixels.Length; i++)
            {
                ref Rgba32 pixel = ref pixels[i];
                byte a = pixel.A;

                if (a is 0)
                {
                    pixel.PackedValue = 0;
                }
                else
                {
                    pixel.R = (byte)((pixel.R * a) >> 8);
                    pixel.G = (byte)((pixel.G * a) >> 8);
                    pixel.B = (byte)((pixel.B * a) >> 8);
                }
            }

            ImageDescription _data = new ImageDescription()
            {
                Width = _image.Width,
                Height = _image.Height,
                Format = VkFormat.R8G8B8A8UNorm,
                Size = _image.Width * _image.Height * 4,
                Depth = 1,
                IsCubeMap = false,
                MipLevels = 1, // (int)Math.Floor(Math.Log(Math.Max(_image.Width, _image.Height))) + 1, // TODO: MipMaps 
                Data = MemoryMarshal.AsBytes(pixels).ToArray(),
            };


            TextureData = _data;

        }


        public IMGLoader(string filename)
        {
            _image = SixLabors.ImageSharp.Image.Load<Rgba32>(filename);
            Span<Rgba32> pixels = _image.GetPixelSpan();

            for (int i = 0; i < pixels.Length; i++)
            {
                ref Rgba32 pixel = ref pixels[i];
                byte a = pixel.A;

                if (a is 0)
                {
                    pixel.PackedValue = 0;
                }
                else
                {
                    pixel.R = (byte)((pixel.R * a) >> 8);
                    pixel.G = (byte)((pixel.G * a) >> 8);
                    pixel.B = (byte)((pixel.B * a) >> 8);
                }
            }

            ImageDescription data = new ImageDescription()
            {
                Width = _image.Width,
                Height = _image.Height,
                Format = VkFormat.R8G8B8A8UNorm,
                Size = _image.Width * _image.Height * 4,
                Depth = 1,
                IsCubeMap = false,
                MipLevels = 1, //(int)Math.Floor(Math.Log(Math.Max(_image.Width, _image.Height))) + 1, // TODO: MipMaps 
                Data = MemoryMarshal.AsBytes(pixels).ToArray(),
                //ArraySize = 1,
                
            };


            TextureData = data;

        }


        public ImageDescription TextureData { get; private set; }

        public int Width => _image.Width;

        public int Height => _image.Height;

        public int MipMaps => (int)Math.Floor(Math.Log(Math.Max(_image.Width, _image.Height))) + 1; // TODO: MipMaps 

        public int Size => _image.Width * _image.Height * 4;

        public byte[] Data => GetAllTextureData();

        public bool IsCubeMap => false;


        public static ImageDescription LoadFromData(byte[] data)
        {
            return new IMGLoader(data).TextureData;
        }

        public static ImageDescription LoadFromFile(string filename)
        {
            return new IMGLoader(filename).TextureData;
        }

        public void Dispose()
        {
            _image.Dispose();
        }

        private byte[] GetAllTextureData()
        {
            Span<Rgba32> pixels = _image.GetPixelSpan();

            return MemoryMarshal.AsBytes(pixels).ToArray();
        }
    }
}
