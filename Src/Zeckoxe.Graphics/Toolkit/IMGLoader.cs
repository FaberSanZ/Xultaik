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

		private Stream _stream;
		private byte[] _buffer = new byte[1024];

		private readonly ImageNative.io_callbacks _callbacks;

		public TextureData TextureData { get; set; }

		public IMGLoader(string filename)
		{
			_callbacks = new ImageNative.io_callbacks
			{
				read = ReadCallback,
				skip = SkipCallback,
				eof = Eof
			};

			using FileStream fs = File.OpenRead(filename);

			TextureData = Load(fs);
		}


		public IMGLoader(Stream stream)
		{
			_callbacks = new ImageNative.io_callbacks
			{
				read = ReadCallback,
				skip = SkipCallback,
				eof = Eof
			};

			TextureData = Load(stream);
		}


		private int SkipCallback(void* user, int i)
		{
			return (int)_stream.Seek(i, SeekOrigin.Current);
		}

		private int Eof(void* user)
		{
			return _stream.CanRead ? 1 : 0;
		}

		private int ReadCallback(void* user, sbyte* data, int size)
		{
			if (size > _buffer.Length)
			{
				_buffer = new byte[size * 2];
			}

			int res = _stream.Read(_buffer, 0, size);
			Marshal.Copy(_buffer, 0, (IntPtr)data, size);
			return res;
		}




		internal TextureData Load(Stream stream)
		{
			_stream = stream;
			int req_comp = ImageNative.rgb_alpha;
			int x;
			int y;
			int comp;
			byte* result = ImageNative.load_from_callbacks(_callbacks, null, &x, &y, &comp, req_comp);

			TextureData image = new TextureData
			{
				Width = x,
				Height = y,
				Depth = 1,
				Size = 1,
				IsCubeMap = false,
				MipMaps = (int)Math.Floor(Math.Log(Math.Max(x, y), 2)) + 1,
				Format = PixelFormat.R8g8b8a8Snorm
			};

			//Console.WriteLine(comp);

			if (result is null)
			{
				throw new Exception(ImageNative.LastError);
			}



			// Convert to array
			byte[] data = new byte[x * y * 4];


			Marshal.Copy((IntPtr)result, data, 0, data.Length);
			ImageNative.free(result);

			image.Data = data;

			return image;
		}



		public static TextureData LoadFromFile(string filename)
		{
			return new IMGLoader(filename).TextureData;
		}


		public static TextureData LoadFromFile(Stream stream)
		{
			return new IMGLoader(stream).TextureData;
		}
	}
}
