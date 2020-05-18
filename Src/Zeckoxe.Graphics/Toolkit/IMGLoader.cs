// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	IMGLoader.cs
=============================================================================*/





using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Zeckoxe.Graphics.Toolkit
{

	public unsafe class IMGLoader
	{
		public TextureData TextureData { get; set; }

		public IMGLoader(string filename)
		{
			//using FileStream fs = File.OpenRead(filename);
			 byte[] fs = File.ReadAllBytes(filename);

			TextureData = Load(fs);
		}





		internal TextureData Load(byte[] stream)
		{
			//_stream = stream;
			int req_comp = ImageNative.rgb_alpha;
			int x;
			int y;
			int comp;
			byte* result;
			fixed (byte* ptr = stream)
			{
				result = ImageNative.load_from_memory(ptr, stream.Length, &x, &y, &comp, req_comp);
			}



			int bpp = 4; // R8G8B8A8Unorm -> Red - Green - Blue - Alpha

			int rowPitch = x * bpp;
			int slicePitch = rowPitch * y;


			TextureData image = new TextureData
			{
				Width = x,
				Height = y,
				Depth = 1,
				Size = slicePitch,
				IsCubeMap = false,
				MipMaps = (int)Math.Floor(Math.Log(Math.Max(x, y), 2)) + 1,
				Format = PixelFormat.R8g8b8a8Unorm
			};

			if (result is null)
			{
				throw new Exception(ImageNative.LastError);
			}


			// Convert to array
			byte[] data = new byte[slicePitch];


			Marshal.Copy((IntPtr)result, data, 0, data.Length);
			ImageNative.free(result);

			image.Data = data;

			return image;
		}



		public static TextureData LoadFromFile(string filename)
		{
			return new IMGLoader(filename).TextureData;
		}


		//public static TextureData LoadFromFile(Stream stream)
		//{
		//	return new IMGLoader(stream).TextureData;
		//}
	}
}
