// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.
// Based on STB: https://github.com/nothings/stb

/*=============================================================================
	ImageNative.cs
=============================================================================*/



using System;
using System.Runtime.InteropServices;

namespace Zeckoxe.Graphics
{
	internal unsafe class ImageNative
	{
		public const long DBL_EXP_MASK =		0x7ff0000000000000L;
		public const int DBL_MANT_BITS =		52;
		public const long DBL_SGN_MASK =		-1 - 0x7fffffffffffffffL;
		public const long DBL_MANT_MASK =		0x000fffffffffffffL;
		public const long DBL_EXP_CLR_MASK =	DBL_SGN_MASK | DBL_MANT_MASK;

		public const int Default =				0;
		public const int grey =					1;
		public const int grey_alpha =			2;
		public const int rgb =					3;
		public const int rgb_alpha =			4;
		public const int ORDER_RGB =			0;
		public const int ORDER_BGR =			1;
		public const int SCAN_load =			0;
		public const int SCAN_type =			1;
		public const int SCAN_header =			2;
		public const int F_none =				0;
		public const int F_sub =				1;
		public const int F_up =					2;
		public const int F_avg =				3;
		public const int F_paeth =				4;
		public const int F_avg_first =			5;
		public const int F_paeth_first =		6;
		public static float h2l_gamma_i =		1.0f / 2.2f;
		public static float h2l_scale_i =		1.0f;

		public const int ZFAST_BITS =			9;


		public static int unpremultiply_on_load = 0;
		public static int de_iphone_flag = 0;
		public static byte[] depth_scale_table = { 0, 0xff, 0x55, 0, 0x11, 0, 0, 0, 0x01 };

		public static string g_failure_reason;
		public static int vertically_flip_on_load;

		public static string LastError;


		public delegate int ReadCallback(void* user, sbyte* data, int size);

		public delegate int SkipCallback(void* user, int n);

		public delegate int EofCallback(void* user);

		public delegate void idct_block_kernel(byte* output, int out_stride, short* data);

		public delegate void YCbCr_to_RGB_kernel(byte* output, byte* y, byte* pcb, byte* pcr, int count, int step);

		public delegate byte* Resampler(byte* a, byte* b, byte* c, int d, int e);

		public delegate int QSortComparer(void* a, void* b);


		public class io_callbacks
		{
			public ReadCallback read;
			public SkipCallback skip;
			public EofCallback eof;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct IMGComp
		{
			public int id;
			public int h;
			public int v;
			public int tq;
			public int ha;
			public int hd;
			public int dc_pred;

			public int x, y, w2, h2;
			public byte* data;
			public void* raw_data;
			public void* raw_coeff;
			public byte* linebuf;
			public short* coeff;			// progressive only
			public int coeff_w, coeff_h;	// number of 8x8 coefficient blocks
		}

		public class jpeg
		{
			public Context s;
			public readonly huffman[] huff_dc = new huffman[4];
			public readonly huffman[] huff_ac = new huffman[4];
			public readonly ushort[][] dequant;

			public readonly short[][] fast_ac;

			public int img_h_max, img_v_max;
			public int img_mcu_x, img_mcu_y;
			public int img_mcu_w, img_mcu_h;

			public IMGComp[] img_comp = new IMGComp[4];

			public uint code_buffer; // jpeg entropy-coded buffer
			public int code_bits; // number of valid bits
			public byte marker; // marker seen while filling entropy buffer
			public int nomore; // flag if we saw a marker so must stop

			public int progressive;
			public int spec_start;
			public int spec_end;
			public int succ_high;
			public int succ_low;
			public int eob_run;
			public int jfif;
			public int app14_color_transform; // Adobe APP14 tag
			public int rgb;

			public int scan_n;
			public int[] order = new int[4];
			public int restart_interval, todo;

			// kernels
			public idct_block_kernel idct_block_kernel;
			public YCbCr_to_RGB_kernel YCbCr_to_RGB_kernel;
			public Resampler resample_row_hv_2_kernel;

			public jpeg()
			{
				for (int i = 0; i < 4; ++i)
				{
					huff_ac[i] = new huffman();
					huff_dc[i] = new huffman();
				}

				for (int i = 0; i < img_comp.Length; ++i)
				{
					img_comp[i] = new IMGComp();
				}

				fast_ac = new short[4][];
				for (int i = 0; i < fast_ac.Length; ++i)
				{
					fast_ac[i] = new short[1 << ZFAST_BITS];
				}

				dequant = new ushort[4][];
				for (int i = 0; i < dequant.Length; ++i)
				{
					dequant[i] = new ushort[64];
				}
			}
		};

		public class Resample
		{
			public Resampler resample;
			public byte* line0;
			public byte* line1;
			public int hs;
			public int vs;
			public int w_lores;
			public int ystep;
			public int ypos;
		}


		private static int err(string str)
		{
			LastError = str;
			return 0;
		}



		public class Context
		{
			public uint img_x;
			public uint img_y;
			public int img_n;
			public int img_out_n;
			public io_callbacks io = new io_callbacks();
			public void* io_user_data;
			public int read_from_callbacks;
			public int buflen;
			public byte* buffer_start = (byte*)malloc(128);
			public byte* img_buffer;
			public byte* img_buffer_end;
			public byte* img_buffer_original;
			public byte* img_buffer_original_end;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct result_info
		{
			public int bits_per_channel;
			public int num_channels;
			public int channel_order;
		}

		public class huffman
		{
			public byte[] fast = new byte[1 << 9];
			public ushort[] code = new ushort[256];
			public byte[] values = new byte[256];
			public byte[] size = new byte[257];
			public uint[] maxcode = new uint[18];
			public int[] delta = new int[17];
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct zhuffman
		{
			public fixed ushort fast[1 << 9];
			public fixed ushort firstcode[16];
			public fixed int maxcode[17];
			public fixed ushort firstsymbol[16];
			public fixed byte size[288];
			public fixed ushort value[288];
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct zbuf
		{
			public byte* zbuffer;
			public byte* zbuffer_end;
			public int num_bits;
			public uint code_buffer;
			public sbyte* zout;
			public sbyte* zout_start;
			public sbyte* zout_end;
			public int z_expandable;
			public zhuffman z_length;
			public zhuffman z_distance;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct pngchunk
		{
			public uint length;
			public uint type;
		}

		public class png
		{
			public Context s = new Context();
			public byte* idata;
			public byte* expanded;
			public byte* _out_;
			public int depth;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct bmp_data
		{
			public int bpp;
			public int offset;
			public int hsz;
			public uint mr;
			public uint mg;
			public uint mb;
			public uint ma;
			public uint all_a;
		}


		public static uint[] bmask => new uint[]
		{
			0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767, 65535
		};

		public static int[] jbias => new int[]
		{
			0, -1, -3, -7, -15, -31, -63, -127, -255, -511, -1023, -2047, -4095, -8191, -16383,
			-32767
		};

		public static byte[] jpeg_dezigzag => new byte[]
		{
			0, 1, 8, 16, 9, 2, 3, 10, 17, 24, 32, 25, 18, 11, 4, 5, 12, 19, 26, 33, 40,
			48, 41, 34, 27, 20, 13, 6, 7, 14, 21, 28, 35, 42, 49, 56, 57, 50, 43, 36, 29, 22, 15, 23, 30, 37, 44, 51,
			58, 59, 52,
			45, 38, 31, 39, 46, 53, 60, 61, 54, 47, 55, 62, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63,
			63
		};

		public static int[] zlength_base => new int[]
		{
			3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51, 59, 67,
			83, 99, 115, 131, 163, 195, 227, 258, 0, 0
		};

		public static int[] zlength_extra => new int[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5,
			5, 5, 5, 0, 0, 0
		};

		public static int[] zdist_base => new int[]
		{
			1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385, 513, 769,
			1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577, 0, 0
		};

		public static int[] zdist_extra => new int[]
		{
			0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11,
			11, 12, 12, 13, 13
		};

		public static byte[] length_dezigzag => new byte[] 
		{ 
			16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 
		};

		public static byte[] zdefault_length => new byte[]
		{
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8,
			8, 8, 8, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
			9, 9, 9,
			9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
			9, 9, 9,
			9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
			9, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8
		};

		public static byte[] zdefault_distance => new byte[]
		{
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 5, 5, 5, 5, 5, 5
		};

		public static byte[] png_sig => new byte[] 
		{ 
			137, 80, 78, 71, 13, 10, 26, 10 
		};

		public static byte[] first_row_filter => new byte[]
		{
			F_none, F_sub, F_none, F_avg_first,
			F_paeth_first
		};



		public static void* malloc(ulong size)
		{
			return malloc((long)size);
		}

		public static void* malloc(long size)
		{
			IntPtr ptr = Marshal.AllocHGlobal((int)size);

			return ptr.ToPointer();
		}

		public static void memcpy(void* a, void* b, ulong size)
		{
			byte* ap = (byte*)a;
			byte* bp = (byte*)b;
			for (ulong i = 0; i < size; ++i)
			{
				*ap++ = *bp++;
			}
		}

		public static void free(void* a)
		{
			IntPtr ptr = new IntPtr(a);
			Marshal.FreeHGlobal(ptr);
		}

		public static void memset(void* ptr, int value, ulong size)
		{
			byte* bptr = (byte*)ptr;
			byte bval = (byte)value;
			for (ulong i = 0; i < size; ++i)
			{
				*bptr++ = bval;
			}
		}



		public static uint _lrotl(uint x, int y)
		{
			return (x << y) | (x >> (32 - y));
		}

		public static void* realloc(void* a, ulong newSize)
		{
			if (a is null)
			{
				return malloc(newSize);
			}

			IntPtr result = Marshal.ReAllocHGlobal((IntPtr)a, new IntPtr((long)newSize));

			return result.ToPointer();
		}







		public static void start_mem(Context s, byte* buffer, int len)
		{
			s.io.read = null;
			s.read_from_callbacks = 0;
			s.img_buffer = s.img_buffer_original = buffer;
			s.img_buffer_end = s.img_buffer_original_end = buffer + len;
		}

		public static void start_callbacks(Context s, io_callbacks c, void* user)
		{
			s.io = c;
			s.io_user_data = user;
			s.buflen = 128;
			s.read_from_callbacks = 1;
			s.img_buffer_original = s.buffer_start;
			refill_buffer(s);
			s.img_buffer_original_end = s.img_buffer_end;
		}

		public static void rewind(Context s)
		{
			s.img_buffer = s.img_buffer_original;
			s.img_buffer_end = s.img_buffer_original_end;
		}

		public static int addsizes_valid(int a, int b)
		{
			if (b < 0)
			{
				return 0;
			}

			return (a <= 2147483647 - b) ? 1 : 0;
		}

		public static int mul2sizes_valid(int a, int b)
		{
			if (a < 0 || b < 0)
			{
				return 0;
			}
			if (b is 0)
			{
				return 1;
			}
			return (a <= 2147483647 / b) ? 1 : 0;
		}

		public static int Mad2SizesValid(int a, int b, int add)
		{
			return mul2sizes_valid(a, b) != 0 && addsizes_valid(a * b, add) != 0 ? 1 : 0;
		}

		public static int mad3sizes_valid(int a, int b, int c, int add)
		{
			return mul2sizes_valid(a, b) != 0 &&
				   (mul2sizes_valid(a * b, c) != 0) &&
				   (addsizes_valid(a * b * c, add) != 0) ? 1 : 0;
		}


		public static void* malloc_mad2(int a, int b, int add)
		{
			if (Mad2SizesValid(a, b, add) == 0)
			{
				return (null);
			}

			return malloc((ulong)(a * b + add));
		}

		public static void* malloc_mad3(int a, int b, int c, int add)
		{
			if (mad3sizes_valid(a, b, c, add) == 0)
			{
				return (null);
			}

			return malloc((ulong)(a * b * c + add));
		}

		public static void* load_main(Context s, int* x, int* y, int* comp, int req_comp, result_info* info, int bpc)
		{
			info->bits_per_channel = 8;
			info->channel_order = ORDER_RGB;
			info->num_channels = 0;


			if (jpeg_test(s) != 0)
			{
				return jpeg_load(s, x, y, comp, req_comp, info);
			}

			if (png_test(s) != 0)
			{
				return png_load(s, x, y, comp, req_comp, info);
			}

			if (bmp_test(s) != 0)
			{
				return bmp_load(s, x, y, comp, req_comp, info);
			}

			if (tga_test(s) != 0)
			{
				return tga_load(s, x, y, comp, req_comp, info);
			}

			byte* res = stackalloc byte[1]
			{
				0
			};

			return err("unknown image type") != 0 ? res : null;
		}

		public static byte* convert_16_to_8(ushort* orig, int w, int h, int channels)
		{
			int i;
			int img_len = w * h * channels;
			byte* reduced = stackalloc byte[img_len];

			byte* res = stackalloc byte[1]
			{
				0
			};

			if (reduced is null)
			{
				return err("outofmem") != 0 ? res : null;
			}

			for (i = 0; i < img_len; ++i)
			{
				reduced[i] = (byte)((orig[i] >> 8) & 0xFF);
			}

			free(orig);
			return reduced;
		}

		public static ushort* convert_8_to_16(byte* orig, int w, int h, int channels)
		{
			int img_len = w * h * channels;
			ushort* enlarged = stackalloc ushort[img_len * 2];
			byte* res = stackalloc byte[1]
			{
				0
			};

			if (enlarged is null)
			{
				return (ushort*)(err("outofmem") != 0 ? res : null);
			}

			for (int i = 0; i < img_len; ++i)
			{
				enlarged[i] = (ushort)((orig[i] << 8) + orig[i]);
			}

			free(orig);
			return enlarged;
		}

		public static void vertical_flip(void* image, int w, int h, int bytes_per_pixel)
		{
			int row;
			ulong bytes_per_row = (ulong)(w * bytes_per_pixel);
			byte* temp = stackalloc byte[2048];
			byte* bytes = (byte*)(image);
			for (row = 0; (row) < (h >> 1); row++)
			{
				byte* row0 = bytes + (ulong)row * bytes_per_row;
				byte* row1 = bytes + (ulong)(h - row - 1) * bytes_per_row;
				ulong bytes_left = bytes_per_row;
				while ((bytes_left) != 0)
				{
					ulong bytes_copy = ((bytes_left) < (2048)) ? bytes_left : 2048;
					memcpy(temp, row0, bytes_copy);
					memcpy(row0, row1, bytes_copy);
					memcpy(row1, temp, bytes_copy);
					row0 += bytes_copy;
					row1 += bytes_copy;
					bytes_left -= bytes_copy;
				}
			}
		}

		public static byte* load_and_postprocess_8bit(Context s, int* x, int* y, int* comp, int req_comp)
		{
			result_info ri = new result_info();
			void* result = load_main(s, x, y, comp, req_comp, &ri, 8);
			if ((result) == (null))
			{
				return (null);
			}

			if (ri.bits_per_channel != 8)
			{
				result = convert_16_to_8((ushort*)result, *x, *y, req_comp == 0 ? *comp : req_comp);
				ri.bits_per_channel = 8;
			}

			if ((vertically_flip_on_load) != 0)
			{
				int channels = (req_comp) != 0 ? req_comp : *comp;
				vertical_flip(result, *x, *y, channels);
			}

			return (byte*)(result);
		}

		public static ushort* load_and_postprocess_16bit(Context s, int* x, int* y, int* comp, int req_comp)
		{
			result_info ri = new result_info();
			void* result = load_main(s, x, y, comp, req_comp, &ri, 16);
			if ((result) == (null))
			{
				return (null);
			}

			if (ri.bits_per_channel != 16)
			{
				result = convert_8_to_16((byte*)(result), *x, *y,
					(req_comp) == (0) ? *comp : req_comp);
				ri.bits_per_channel = 16;
			}

			if ((vertically_flip_on_load) != 0)
			{
				int channels = (req_comp) != 0 ? req_comp : *comp;
				vertical_flip(result, *x, *y, channels * 2);
			}

			return (ushort*)(result);
		}

		public static ushort* load_16_from_memory(byte* buffer, int len, int* x, int* y, int* channels_in_file,
			int desired_channels)
		{
			Context s = new Context();
			start_mem(s, buffer, len);
			return load_and_postprocess_16bit(s, x, y, channels_in_file, desired_channels);
		}

		public static ushort* load_16_from_callbacks(io_callbacks clbk, void* user, int* x, int* y,
			int* channels_in_file, int desired_channels)
		{
			Context s = new Context();
			start_callbacks(s, clbk, user);
			return load_and_postprocess_16bit(s, x, y, channels_in_file, desired_channels);
		}

		public static byte* load_from_memory(byte* buffer, int len, int* x, int* y, int* comp, int req_comp)
		{
			Context s = new Context();
			start_mem(s, buffer, len);
			return load_and_postprocess_8bit(s, x, y, comp, req_comp);
		}

		public static byte* load_from_callbacks(io_callbacks clbk, void* user, int* x, int* y, int* comp, int req_comp)
		{
			Context s = new Context();
			start_callbacks(s, clbk, user);
			return load_and_postprocess_8bit(s, x, y, comp, req_comp);
		}


		public static void refill_buffer(Context s)
		{
			int n = s.io.read(s.io_user_data, (sbyte*)(s.buffer_start), s.buflen);
			if ((n) == (0))
			{
				s.read_from_callbacks = 0;
				s.img_buffer = s.buffer_start;
				s.img_buffer_end = s.buffer_start;
				s.img_buffer_end++;

				*s.img_buffer = 0;
			}
			else
			{
				s.img_buffer = s.buffer_start;
				s.img_buffer_end = s.buffer_start;
				s.img_buffer_end += n;
			}

		}

		public static byte get8(Context s)
		{
			if (s.img_buffer < s.img_buffer_end)
			{
				return *s.img_buffer++;
			}

			if (s.read_from_callbacks != 0)
			{
				refill_buffer(s);
				return *s.img_buffer++;
			}

			return 0;
		}

		public static int at_eof(Context s)
		{
			if ((s.io.read) != null)
			{
				if (s.io.eof(s.io_user_data) == 0)
				{
					return 0;
				}

				if ((s.read_from_callbacks) == (0))
				{
					return 1;
				}
			}

			return (s.img_buffer) >= (s.img_buffer_end) ? 1 : 0;
		}

		public static void skip(Context s, int n)
		{
			if ((n) < (0))
			{
				s.img_buffer = s.img_buffer_end;
				return;
			}

			if ((s.io.read) != null)
			{
				int blen = (int)(s.img_buffer_end - s.img_buffer);
				if ((blen) < (n))
				{
					s.img_buffer = s.img_buffer_end;
					s.io.skip(s.io_user_data, n - blen);
					return;
				}
			}

			s.img_buffer += n;
		}

		public static int getn(Context s, byte* buffer, int n)
		{
			if ((s.io.read) != null)
			{
				int blen = (int)(s.img_buffer_end - s.img_buffer);
				if ((blen) < (n))
				{
					int res;
					int count;
					memcpy(buffer, s.img_buffer, (ulong)(blen));
					count = s.io.read(s.io_user_data, (sbyte*)(buffer) + blen, n - blen);
					res = (count) == (n - blen) ? 1 : 0;
					s.img_buffer = s.img_buffer_end;
					return res;
				}
			}

			if (s.img_buffer + n <= s.img_buffer_end)
			{
				memcpy(buffer, s.img_buffer, (ulong)(n));
				s.img_buffer += n;
				return 1;
			}
			else
			{
				return 0;
			}
		}

		public static int get16be(Context s)
		{
			int z = get8(s);
			return (z << 8) + get8(s);
		}

		public static uint get32be(Context s)
		{
			uint z = (uint)(get16be(s));
			return (uint)((z << 16) + get16be(s));
		}

		public static int get16le(Context s)
		{
			int z = get8(s);
			return z + (get8(s) << 8);
		}

		public static uint get32le(Context s)
		{
			uint z = (uint)(get16le(s));
			return (uint)(z + (get16le(s) << 16));
		}

		public static byte compute_y(int r, int g, int b)
		{
			return (byte)(((r * 77) + (g * 150) + (29 * b)) >> 8);
		}

		public static byte* convert_format(byte* data, int img_n, int req_comp, uint x, uint y)
		{
			int i;
			int j;
			byte* good;
			if ((req_comp) == (img_n))
			{
				return data;
			}

			good = (byte*)(malloc_mad3(req_comp, (int)(x), (int)(y), 0));
			if ((good) == (null))
			{
				free(data);
				return (err("outofmem")) != 0 ? ((byte*)null) : (null);
			}

			for (j = 0; (j) < ((int)(y)); ++j)
			{
				byte* src = data + j * x * img_n;
				byte* dest = good + j * x * req_comp;
				switch (((img_n) * 8 + (req_comp)))
				{
					case ((1) * 8 + (2)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 2)
						{
							dest[0] = src[0];
							dest[1] = 255;
						}

						break;
					case ((1) * 8 + (3)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 3)
						{
							dest[0] = dest[1] = dest[2] = src[0];
						}

						break;
					case ((1) * 8 + (4)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 4)
						{
							dest[0] = dest[1] = dest[2] = src[0];
							dest[3] = 255;
						}

						break;
					case ((2) * 8 + (1)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 1)
						{
							dest[0] = src[0];
						}

						break;
					case ((2) * 8 + (3)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 3)
						{
							dest[0] = dest[1] = dest[2] = src[0];
						}

						break;
					case ((2) * 8 + (4)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 4)
						{
							dest[0] = dest[1] = dest[2] = src[0];
							dest[3] = src[1];
						}

						break;
					case ((3) * 8 + (4)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 4)
						{
							dest[0] = src[0];
							dest[1] = src[1];
							dest[2] = src[2];
							dest[3] = 255;
						}

						break;
					case ((3) * 8 + (1)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 1)
						{
							dest[0] = compute_y(src[0], src[1], src[2]);
						}

						break;
					case ((3) * 8 + (2)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 2)
						{
							dest[0] = compute_y(src[0], src[1], src[2]);
							dest[1] = 255;
						}

						break;
					case ((4) * 8 + (1)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 1)
						{
							dest[0] = compute_y(src[0], src[1], src[2]);
						}

						break;
					case ((4) * 8 + (2)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 2)
						{
							dest[0] = compute_y(src[0], src[1], src[2]);
							dest[1] = src[3];
						}

						break;
					case ((4) * 8 + (3)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 3)
						{
							dest[0] = src[0];
							dest[1] = src[1];
							dest[2] = src[2];
						}

						break;
					default:
						return (err("0")) != 0 ? ((byte*)null) : (null);
				}
			}

			free(data);
			return good;
		}

		public static ushort compute_y_16(int r, int g, int b)
		{
			return (ushort)(((r * 77) + (g * 150) + (29 * b)) >> 8);
		}

		public static ushort* convert_format16(ushort* data, int img_n, int req_comp, uint x, uint y)
		{
			int i;
			int j;
			ushort* good;
			if ((req_comp) == (img_n))
			{
				return data;
			}

			good = (ushort*)(malloc((ulong)(req_comp * x * y * 2)));
			if ((good) == (null))
			{
				free(data);
				return (ushort*)((err("outofmem")) != 0 ? ((byte*)null) : (null));
			}

			for (j = 0; (j) < ((int)(y)); ++j)
			{
				ushort* src = data + j * x * img_n;
				ushort* dest = good + j * x * req_comp;
				switch (((img_n) * 8 + (req_comp)))
				{
					case ((1) * 8 + (2)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 2)
						{
							dest[0] = src[0];
							dest[1] = 0xffff;
						}

						break;
					case ((1) * 8 + (3)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 3)
						{
							dest[0] = dest[1] = dest[2] = src[0];
						}

						break;
					case ((1) * 8 + (4)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 4)
						{
							dest[0] = dest[1] = dest[2] = src[0];
							dest[3] = 0xffff;
						}

						break;
					case ((2) * 8 + (1)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 1)
						{
							dest[0] = src[0];
						}

						break;
					case ((2) * 8 + (3)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 3)
						{
							dest[0] = dest[1] = dest[2] = src[0];
						}

						break;
					case ((2) * 8 + (4)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 4)
						{
							dest[0] = dest[1] = dest[2] = src[0];
							dest[3] = src[1];
						}

						break;
					case ((3) * 8 + (4)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 4)
						{
							dest[0] = src[0];
							dest[1] = src[1];
							dest[2] = src[2];
							dest[3] = 0xffff;
						}

						break;
					case ((3) * 8 + (1)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 1)
						{
							dest[0] = compute_y_16(src[0], src[1], src[2]);
						}

						break;
					case ((3) * 8 + (2)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 2)
						{
							dest[0] = compute_y_16(src[0], src[1], src[2]);
							dest[1] = 0xffff;
						}

						break;
					case ((4) * 8 + (1)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 1)
						{
							dest[0] = compute_y_16(src[0], src[1], src[2]);
						}

						break;
					case ((4) * 8 + (2)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 2)
						{
							dest[0] = compute_y_16(src[0], src[1], src[2]);
							dest[1] = src[3];
						}

						break;
					case ((4) * 8 + (3)):
						for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 3)
						{
							dest[0] = src[0];
							dest[1] = src[1];
							dest[2] = src[2];
						}

						break;
					default:
						return (ushort*)((err("0")) != 0 ? ((byte*)null) : (null));
				}
			}

			free(data);
			return good;
		}

		public static int build_huffman(huffman h, int* count)
		{
			int i;
			int j;
			int k = 0;
			int code;
			for (i = 0; (i) < (16); ++i)
			{
				for (j = 0; (j) < (count[i]); ++j)
				{
					h.size[k++] = ((byte)(i + 1));
				}
			}

			h.size[k] = 0;
			code = 0;
			k = 0;
			for (j = 1; j <= 16; ++j)
			{
				h.delta[j] = k - code;
				if ((h.size[k]) == (j))
				{
					while ((h.size[k]) == (j))
					{
						h.code[k++] = ((ushort)(code++));
					}

					if ((code - 1) >= (1 << j))
					{
						return err("bad code lengths");
					}
				}

				h.maxcode[j] = (uint)(code << (16 - j));
				code <<= 1;
			}

			h.maxcode[j] = 0xffffffff;
			for (i = 0; i < h.fast.Length; ++i)
			{
				h.fast[i] = 255;
			}
			for (i = 0; (i) < (k); ++i)
			{
				int s = h.size[i];
				if (s <= 9)
				{
					int c = h.code[i] << (9 - s);
					int m = 1 << (9 - s);
					for (j = 0; (j) < (m); ++j)
					{
						h.fast[c + j] = ((byte)(i));
					}
				}
			}

			return 1;
		}

		public static void build_fast_ac(short[] fast_ac, huffman h)
		{
			int i;
			for (i = 0; (i) < (1 << 9); ++i)
			{
				byte fast = h.fast[i];
				fast_ac[i] = 0;
				if ((fast) < (255))
				{
					int rs = h.values[fast];
					int run = (rs >> 4) & 15;
					int magbits = rs & 15;
					int len = h.size[fast];
					if (((magbits) != 0) && (len + magbits <= 9))
					{
						int k = ((i << len) & ((1 << 9) - 1)) >> (9 - magbits);
						int m = 1 << (magbits - 1);
						if ((k) < (m))
						{
							k += (int)((~0U << magbits) + 1);
						}

						if (((k) >= (-128)) && (k <= 127))
						{
							fast_ac[i] = ((short)((k << 8) + (run << 4) + (len + magbits)));
						}
					}
				}
			}
		}

		public static void grow_buffer_unsafe(jpeg j)
		{
			do
			{
				int b = (j.nomore) != 0 ? 0 : get8(j.s);
				if ((b) == (0xff))
				{
					int c = get8(j.s);
					while ((c) == (0xff))
					{
						c = get8(j.s);
					}

					if (c != 0)
					{
						j.marker = ((byte)(c));
						j.nomore = 1;
						return;
					}
				}

				j.code_buffer |= (uint)(b << (24 - j.code_bits));
				j.code_bits += 8;
			} while (j.code_bits <= 24);
		}

		public static int jpeg_huff_decode(jpeg j, huffman h)
		{
			uint temp;
			int c;
			int k;
			if ((j.code_bits) < (16))
			{
				grow_buffer_unsafe(j);
			}

			c = (int)((j.code_buffer >> (32 - 9)) & ((1 << 9) - 1));
			k = h.fast[c];
			if ((k) < (255))
			{
				int s = h.size[k];
				if ((s) > (j.code_bits))
				{
					return -1;
				}

				j.code_buffer <<= s;
				j.code_bits -= s;
				return h.values[k];
			}

			temp = j.code_buffer >> 16;
			for (k = 9 + 1; ; ++k)
			{
				if ((temp) < (h.maxcode[k]))
				{
					break;
				}
			}

			if ((k) == (17))
			{
				j.code_bits -= 16;
				return -1;
			}

			if ((k) > (j.code_bits))
			{
				return -1;
			}

			c = (int)(((j.code_buffer >> (32 - k)) & bmask[k]) + h.delta[k]);
			j.code_bits -= k;
			j.code_buffer <<= k;
			return h.values[c];
		}

		public static int extend_receive(jpeg j, int n)
		{
			uint k;
			int sgn;
			if ((j.code_bits) < (n))
			{
				grow_buffer_unsafe(j);
			}

			sgn = (int)j.code_buffer >> 31;
			k = _lrotl(j.code_buffer, n);
			j.code_buffer = k & ~bmask[n];
			k &= bmask[n];
			j.code_bits -= n;
			return (int)(k + (jbias[n] & ~sgn));
		}

		public static int jpeg_get_bits(jpeg j, int n)
		{
			uint k;
			if ((j.code_bits) < (n))
			{
				grow_buffer_unsafe(j);
			}

			k = _lrotl(j.code_buffer, n);
			j.code_buffer = k & ~bmask[n];
			k &= bmask[n];
			j.code_bits -= n;
			return (int)(k);
		}

		public static int jpeg_get_bit(jpeg j)
		{
			uint k;
			if (j.code_bits < 1)
			{
				grow_buffer_unsafe(j);
			}

			k = j.code_buffer;
			j.code_buffer <<= 1;
			--j.code_bits;
			return (int)(k & 0x80000000);
		}

		public static int jpeg_decode_block(jpeg j, short* data, huffman hdc, huffman hac,
			short[] fac, int b, ushort[] dequant)
		{
			int diff;
			int dc;
			int k;
			int t;
			if ((j.code_bits) < (16))
			{
				grow_buffer_unsafe(j);
			}

			t = jpeg_huff_decode(j, hdc);
			if ((t) < (0))
			{
				return err("bad huffman code");
			}

			memset(data, 0, (ulong)(64 * sizeof(short)));
			diff = (t) != 0 ? extend_receive(j, t) : 0;
			dc = j.img_comp[b].dc_pred + diff;
			j.img_comp[b].dc_pred = dc;
			data[0] = ((short)(dc * dequant[0]));
			k = 1;
			do
			{
				uint zig;
				int c;
				int r;
				int s;
				if ((j.code_bits) < (16))
				{
					grow_buffer_unsafe(j);
				}

				c = (int)((j.code_buffer >> (32 - 9)) & ((1 << 9) - 1));
				r = fac[c];
				if ((r) != 0)
				{
					k += (r >> 4) & 15;
					s = r & 15;
					j.code_buffer <<= s;
					j.code_bits -= s;
					zig = jpeg_dezigzag[k++];
					data[zig] = ((short)((r >> 8) * dequant[zig]));
				}
				else
				{
					int rs = jpeg_huff_decode(j, hac);
					if ((rs) < (0))
					{
						return err("bad huffman code");
					}

					s = rs & 15;
					r = rs >> 4;
					if ((s) == (0))
					{
						if (rs != 0xf0)
						{
							break;
						}

						k += 16;
					}
					else
					{
						k += r;
						zig = jpeg_dezigzag[k++];
						data[zig] = ((short)(extend_receive(j, s) * dequant[zig]));
					}
				}
			} while ((k) < (64));

			return 1;
		}

		public static int jpeg_decode_block_prog_dc(jpeg j, short* data, huffman hdc, int b)
		{
			int diff;
			int dc;
			int t;
			if (j.spec_end != 0)
			{
				return err("can't merge dc and ac");
			}

			if ((j.code_bits) < (16))
			{
				grow_buffer_unsafe(j);
			}

			if ((j.succ_high) == (0))
			{
				memset(data, 0, (ulong)(64 * sizeof(short)));
				t = jpeg_huff_decode(j, hdc);
				diff = (t) != 0 ? extend_receive(j, t) : 0;
				dc = j.img_comp[b].dc_pred + diff;
				j.img_comp[b].dc_pred = dc;
				data[0] = ((short)(dc << j.succ_low));
			}
			else
			{
				if ((jpeg_get_bit(j)) != 0)
				{
					data[0] += ((short)(1 << j.succ_low));
				}
			}

			return 1;
		}

		public static int jpeg_decode_block_prog_ac(jpeg j, short* data, huffman hac, short[] fac)
		{
			int k;
			if ((j.spec_start) == (0))
			{
				return err("can't merge dc and ac");
			}

			if ((j.succ_high) == (0))
			{
				int shift = j.succ_low;
				if ((j.eob_run) != 0)
				{
					--j.eob_run;
					return 1;
				}

				k = j.spec_start;
				do
				{
					uint zig;
					int c;
					int r;
					int s;
					if ((j.code_bits) < (16))
					{
						grow_buffer_unsafe(j);
					}

					c = (int)((j.code_buffer >> (32 - 9)) & ((1 << 9) - 1));
					r = fac[c];
					if ((r) != 0)
					{
						k += (r >> 4) & 15;
						s = r & 15;
						j.code_buffer <<= s;
						j.code_bits -= s;
						zig = jpeg_dezigzag[k++];
						data[zig] = ((short)((r >> 8) << shift));
					}
					else
					{
						int rs = jpeg_huff_decode(j, hac);
						if ((rs) < (0))
						{
							return err("bad huffman code");
						}

						s = rs & 15;
						r = rs >> 4;
						if ((s) == (0))
						{
							if ((r) < (15))
							{
								j.eob_run = 1 << r;
								if ((r) != 0)
								{
									j.eob_run += jpeg_get_bits(j, r);
								}

								--j.eob_run;
								break;
							}

							k += 16;
						}
						else
						{
							k += r;
							zig = jpeg_dezigzag[k++];
							data[zig] = ((short)(extend_receive(j, s) << shift));
						}
					}
				} while (k <= j.spec_end);
			}
			else
			{
				short bit = (short)(1 << j.succ_low);
				if ((j.eob_run) != 0)
				{
					--j.eob_run;
					for (k = j.spec_start; k <= j.spec_end; ++k)
					{
						short* p = &data[jpeg_dezigzag[k]];
						if (*p != 0)
						{
							if ((jpeg_get_bit(j)) != 0)
							{
								if ((*p & bit) == (0))
								{
									if ((*p) > (0))
									{
										*p += bit;
									}
									else
									{
										*p -= bit;
									}
								}
							}
						}
					}
				}
				else
				{
					k = j.spec_start;
					do
					{
						int r;
						int s;
						int rs = jpeg_huff_decode(j, hac);
						if ((rs) < (0))
						{
							return err("bad huffman code");
						}

						s = rs & 15;
						r = rs >> 4;
						if ((s) == (0))
						{
							if ((r) < (15))
							{
								j.eob_run = (1 << r) - 1;
								if ((r) != 0)
								{
									j.eob_run += jpeg_get_bits(j, r);
								}

								r = 64;
							}
							else
							{
							}
						}
						else
						{
							if (s != 1)
							{
								return err("bad huffman code");
							}

							if ((jpeg_get_bit(j)) != 0)
							{
								s = bit;
							}
							else
							{
								s = -bit;
							}
						}

						while (k <= j.spec_end)
						{
							short* p = &data[jpeg_dezigzag[k++]];
							if (*p != 0)
							{
								if ((jpeg_get_bit(j)) != 0)
								{
									if ((*p & bit) == (0))
									{
										if ((*p) > (0))
										{
											*p += bit;
										}
										else
										{
											*p -= bit;
										}
									}
								}
							}
							else
							{
								if ((r) == (0))
								{
									*p = ((short)(s));
									break;
								}

								--r;
							}
						}
					} while (k <= j.spec_end);
				}
			}

			return 1;
		}

		public static byte clamp(int x)
		{
			if (((uint)(x)) > (255))
			{
				if ((x) < (0))
				{
					return 0;
				}

				if ((x) > (255))
				{
					return 255;
				}
			}

			return (byte)(x);
		}

		public static void idct_block(byte* _out_, int out_stride, short* data)
		{
			int i;
			int* val = stackalloc int[64];
			int* v = val;
			byte* o;
			short* d = data;
			for (i = 0; (i) < (8); ++i, ++d, ++v)
			{
				if ((((((((d[8]) == (0)) && ((d[16]) == (0))) && ((d[24]) == (0))) && ((d[32]) == (0))) &&
					  ((d[40]) == (0))) &&
					 ((d[48]) == (0))) && ((d[56]) == (0)))
				{
					int dcterm = d[0] << 2;
					v[0] =

						v[8] =
							v[16] = v[24] =
								v[32] = v[40] = v[48] = v[56] = dcterm;
				}
				else
				{
					int t0;
					int t1;
					int t2;
					int t3;
					int p1;
					int p2;
					int p3;
					int p4;
					int p5;
					int x0;
					int x1;
					int x2;
					int x3;
					p2 = d[16];
					p3 = d[48];
					p1 = (p2 + p3) * ((int)((0.5411961f) * 4096 + 0.5));
					t2 = p1 + p3 * ((int)((-1.847759065f) * 4096 + 0.5));
					t3 = p1 + p2 * ((int)((0.765366865f) * 4096 + 0.5));
					p2 = d[0];
					p3 = d[32];
					t0 = (p2 + p3) << 12;
					t1 = (p2 - p3) << 12;
					x0 = t0 + t3;
					x3 = t0 - t3;
					x1 = t1 + t2;
					x2 = t1 - t2;
					t0 = d[56];
					t1 = d[40];
					t2 = d[24];
					t3 = d[8];
					p3 = t0 + t2;
					p4 = t1 + t3;
					p1 = t0 + t3;
					p2 = t1 + t2;
					p5 = (p3 + p4) * ((int)((1.175875602f) * 4096 + 0.5));
					t0 = t0 * ((int)((0.298631336f) * 4096 + 0.5));
					t1 = t1 * ((int)((2.053119869f) * 4096 + 0.5));
					t2 = t2 * ((int)((3.072711026f) * 4096 + 0.5));
					t3 = t3 * ((int)((1.501321110f) * 4096 + 0.5));
					p1 = p5 + p1 * ((int)((-0.899976223f) * 4096 + 0.5));
					p2 = p5 + p2 * ((int)((-2.562915447f) * 4096 + 0.5));
					p3 = p3 * ((int)((-1.961570560f) * 4096 + 0.5));
					p4 = p4 * ((int)((-0.390180644f) * 4096 + 0.5));
					t3 += p1 + p4;
					t2 += p2 + p3;
					t1 += p2 + p4;
					t0 += p1 + p3;
					x0 += 512;
					x1 += 512;
					x2 += 512;
					x3 += 512;
					v[0] = (x0 + t3) >> 10;
					v[56] = (x0 - t3) >> 10;
					v[8] = (x1 + t2) >> 10;
					v[48] = (x1 - t2) >> 10;
					v[16] = (x2 + t1) >> 10;
					v[40] = (x2 - t1) >> 10;
					v[24] = (x3 + t0) >> 10;
					v[32] = (x3 - t0) >> 10;
				}
			}

			for (i = 0, v = val, o = _out_; (i) < (8); ++i, v += 8, o += out_stride)
			{
				int t0;
				int t1;
				int t2;
				int t3;
				int p1;
				int p2;
				int p3;
				int p4;
				int p5;
				int x0;
				int x1;
				int x2;
				int x3;
				p2 = v[2];
				p3 = v[6];
				p1 = (p2 + p3) * ((int)((0.5411961f) * 4096 + 0.5));
				t2 = p1 + p3 * ((int)((-1.847759065f) * 4096 + 0.5));
				t3 = p1 + p2 * ((int)((0.765366865f) * 4096 + 0.5));
				p2 = v[0];
				p3 = v[4];
				t0 = (p2 + p3) << 12;
				t1 = (p2 - p3) << 12;
				x0 = t0 + t3;
				x3 = t0 - t3;
				x1 = t1 + t2;
				x2 = t1 - t2;
				t0 = v[7];
				t1 = v[5];
				t2 = v[3];
				t3 = v[1];
				p3 = t0 + t2;
				p4 = t1 + t3;
				p1 = t0 + t3;
				p2 = t1 + t2;
				p5 = (p3 + p4) * ((int)((1.175875602f) * 4096 + 0.5));
				t0 = t0 * ((int)((0.298631336f) * 4096 + 0.5));
				t1 = t1 * ((int)((2.053119869f) * 4096 + 0.5));
				t2 = t2 * ((int)((3.072711026f) * 4096 + 0.5));
				t3 = t3 * ((int)((1.501321110f) * 4096 + 0.5));
				p1 = p5 + p1 * ((int)((-0.899976223f) * 4096 + 0.5));
				p2 = p5 + p2 * ((int)((-2.562915447f) * 4096 + 0.5));
				p3 = p3 * ((int)((-1.961570560f) * 4096 + 0.5));
				p4 = p4 * ((int)((-0.390180644f) * 4096 + 0.5));
				t3 += p1 + p4;
				t2 += p2 + p3;
				t1 += p2 + p4;
				t0 += p1 + p3;
				x0 += 65536 + (128 << 17);
				x1 += 65536 + (128 << 17);
				x2 += 65536 + (128 << 17);
				x3 += 65536 + (128 << 17);
				o[0] = clamp((x0 + t3) >> 17);
				o[7] = clamp((x0 - t3) >> 17);
				o[1] = clamp((x1 + t2) >> 17);
				o[6] = clamp((x1 - t2) >> 17);
				o[2] = clamp((x2 + t1) >> 17);
				o[5] = clamp((x2 - t1) >> 17);
				o[3] = clamp((x3 + t0) >> 17);
				o[4] = clamp((x3 - t0) >> 17);
			}
		}

		public static byte get_marker(jpeg j)
		{
			byte x;
			if (j.marker != 0xff)
			{
				x = j.marker;
				j.marker = 0xff;
				return x;
			}

			x = get8(j.s);
			if (x != 0xff)
			{
				return 0xff;
			}

			while ((x) == (0xff))
			{
				x = get8(j.s);
			}

			return x;
		}

		public static void jpeg_reset(jpeg j)
		{
			j.code_bits = 0;
			j.code_buffer = 0;
			j.nomore = 0;
			j.img_comp[0].dc_pred =
				j.img_comp[1].dc_pred =
					j.img_comp[2].dc_pred = j.img_comp[3].dc_pred = 0;
			j.marker = 0xff;
			j.todo = (j.restart_interval) != 0 ? j.restart_interval : 0x7fffffff;
			j.eob_run = 0;
		}

		public static int parse_entropy_coded_data(jpeg z)
		{
			jpeg_reset(z);
			if (z.progressive == 0)
			{
				if ((z.scan_n) == (1))
				{
					int i;
					int j;
					short* data = stackalloc short[64];
					int n = z.order[0];
					int w = (z.img_comp[n].x + 7) >> 3;
					int h = (z.img_comp[n].y + 7) >> 3;
					for (j = 0; (j) < (h); ++j)
					{
						for (i = 0; (i) < (w); ++i)
						{
							int ha = z.img_comp[n].ha;
							if (
								jpeg_decode_block(z, data, z.huff_dc[z.img_comp[n].hd],
									z.huff_ac[ha],
									z.fast_ac[ha], n, z.dequant[z.img_comp[n].tq]) ==
								0)
							{
								return 0;
							}

							z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2 * j * 8 + i * 8,
								z.img_comp[n].w2, data);
							if (--z.todo <= 0)
							{
								if ((z.code_bits) < (24))
								{
									grow_buffer_unsafe(z);
								}

								if (!(((z.marker) >= (0xd0)) && ((z.marker) <= 0xd7)))
								{
									return 1;
								}

								jpeg_reset(z);
							}
						}
					}

					return 1;
				}
				else
				{
					int i;
					int j;
					int k;
					int x;
					int y;
					short* data = stackalloc short[64];
					for (j = 0; (j) < (z.img_mcu_y); ++j)
					{
						for (i = 0; (i) < (z.img_mcu_x); ++i)
						{
							for (k = 0; (k) < (z.scan_n); ++k)
							{
								int n = z.order[k];
								for (y = 0; (y) < (z.img_comp[n].v); ++y)
								{
									for (x = 0; (x) < (z.img_comp[n].h); ++x)
									{
										int x2 = (i * z.img_comp[n].h + x) * 8;
										int y2 = (j * z.img_comp[n].v + y) * 8;
										int ha = z.img_comp[n].ha;
										if (
											jpeg_decode_block(z, data,
												z.huff_dc[z.img_comp[n].hd],
												z.huff_ac[ha], z.fast_ac[ha], n,
												z.dequant[z.img_comp[n].tq]) == 0)
										{
											return 0;
										}

										z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2 * y2 + x2,
											z.img_comp[n].w2, data);
									}
								}
							}

							if (--z.todo <= 0)
							{
								if ((z.code_bits) < (24))
								{
									grow_buffer_unsafe(z);
								}

								if (!(((z.marker) >= (0xd0)) && ((z.marker) <= 0xd7)))
								{
									return 1;
								}

								jpeg_reset(z);
							}
						}
					}

					return 1;
				}
			}
			else
			{
				if ((z.scan_n) == (1))
				{
					int i;
					int j;
					int n = z.order[0];
					int w = (z.img_comp[n].x + 7) >> 3;
					int h = (z.img_comp[n].y + 7) >> 3;
					for (j = 0; (j) < (h); ++j)
					{
						for (i = 0; (i) < (w); ++i)
						{
							short* data = z.img_comp[n].coeff + 64 * (i + j * z.img_comp[n].coeff_w);
							if ((z.spec_start) == (0))
							{
								if (jpeg_decode_block_prog_dc(z, data,
										z.huff_dc[z.img_comp[n].hd], n) == 0)
								{
									return 0;
								}
							}
							else
							{
								int ha = z.img_comp[n].ha;
								if (jpeg_decode_block_prog_ac(z, data, z.huff_ac[ha], z.fast_ac[ha]) == 0)
								{
									return 0;
								}
							}

							if (--z.todo <= 0)
							{
								if ((z.code_bits) < (24))
								{
									grow_buffer_unsafe(z);
								}

								if (!(((z.marker) >= (0xd0)) && ((z.marker) <= 0xd7)))
								{
									return 1;
								}

								jpeg_reset(z);
							}
						}
					}

					return 1;
				}
				else
				{
					int i;
					int j;
					int k;
					int x;
					int y;
					for (j = 0; (j) < (z.img_mcu_y); ++j)
					{
						for (i = 0; (i) < (z.img_mcu_x); ++i)
						{
							for (k = 0; (k) < (z.scan_n); ++k)
							{
								int n = z.order[k];
								for (y = 0; (y) < (z.img_comp[n].v); ++y)
								{
									for (x = 0; (x) < (z.img_comp[n].h); ++x)
									{
										int x2 = i * z.img_comp[n].h + x;
										int y2 = j * z.img_comp[n].v + y;
										short* data = z.img_comp[n].coeff + 64 * (x2 + y2 * z.img_comp[n].coeff_w);
										if (jpeg_decode_block_prog_dc(z, data,
												z.huff_dc[z.img_comp[n].hd], n) == 0)
										{
											return 0;
										}
									}
								}
							}

							if (--z.todo <= 0)
							{
								if ((z.code_bits) < (24))
								{
									grow_buffer_unsafe(z);
								}

								if (!(((z.marker) >= (0xd0)) && ((z.marker) <= 0xd7)))
								{
									return 1;
								}

								jpeg_reset(z);
							}
						}
					}

					return 1;
				}
			}

		}

		public static void jpeg_dequantize(short* data, ushort[] dequant)
		{
			int i;
			for (i = 0; (i) < (64); ++i)
			{
				data[i] *= (short)(dequant[i]);
			}
		}

		public static void jpeg_finish(jpeg z)
		{
			if ((z.progressive) != 0)
			{
				int i;
				int j;
				int n;
				for (n = 0; (n) < (z.s.img_n); ++n)
				{
					int w = (z.img_comp[n].x + 7) >> 3;
					int h = (z.img_comp[n].y + 7) >> 3;
					for (j = 0; (j) < (h); ++j)
					{
						for (i = 0; (i) < (w); ++i)
						{
							short* data = z.img_comp[n].coeff + 64 * (i + j * z.img_comp[n].coeff_w);
							jpeg_dequantize(data, z.dequant[z.img_comp[n].tq]);
							z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2 * j * 8 + i * 8,
								z.img_comp[n].w2, data);
						}
					}
				}
			}

		}

		public static int process_marker(jpeg z, int m)
		{
			int L;
			switch (m)
			{
				case 0xff:
					return err("expected marker");
				case 0xDD:
					if (get16be(z.s) != 4)
					{
						return err("bad DRI len");
					}

					z.restart_interval = get16be(z.s);
					return 1;
				case 0xDB:
					L = get16be(z.s) - 2;
					while ((L) > (0))
					{
						int q = get8(z.s);
						int p = q >> 4;
						int sixteen = (p != 0) ? 1 : 0;
						int t = q & 15;
						int i;
						if ((p != 0) && (p != 1))
						{
							return err("bad DQT type");
						}

						if ((t) > (3))
						{
							return err("bad DQT table");
						}

						for (i = 0; (i) < (64); ++i)
						{
							z.dequant[t][jpeg_dezigzag[i]] =
								((ushort)((sixteen) != 0 ? get16be(z.s) : get8(z.s)));
						}

						L -= (sixteen) != 0 ? 129 : 65;
					}

					return (L) == (0) ? 1 : 0;
				case 0xC4:
					L = get16be(z.s) - 2;
					while ((L) > (0))
					{
						byte[] v;
						int* sizes = stackalloc int[16];
						int i;
						int n = 0;
						int q = get8(z.s);
						int tc = q >> 4;
						int th = q & 15;
						if (((tc) > (1)) || ((th) > (3)))
						{
							return err("bad DHT header");
						}

						for (i = 0; (i) < (16); ++i)
						{
							sizes[i] = get8(z.s);
							n += sizes[i];
						}

						L -= 17;
						if ((tc) == (0))
						{
							if (build_huffman(z.huff_dc[th], sizes) == 0)
							{
								return 0;
							}

							huffman h = z.huff_dc[th];
							v = h.values;
						}
						else
						{
							if (build_huffman(z.huff_ac[th], sizes) == 0)
							{
								return 0;
							}

							huffman h = z.huff_ac[th];
							v = h.values;
						}

						for (i = 0; (i) < (n); ++i)
						{
							v[i] = get8(z.s);
						}

						if (tc != 0)
						{
							build_fast_ac(z.fast_ac[th], z.huff_ac[th]);
						}

						L -= n;
					}

					return (L) == (0) ? 1 : 0;
			}

			if ((((m) >= (0xE0)) && (m <= 0xEF)) || ((m) == (0xFE)))
			{
				L = get16be(z.s);
				if ((L) < (2))
				{
					if ((m) == (0xFE))
					{
						return err("bad COM len");
					}
					else
					{
						return err("bad APP len");
					}
				}

				L -= 2;
				if (((m) == (0xE0)) && ((L) >= (5)))
				{
					byte* tag = stackalloc byte[5];
					tag[0] = (byte)('J');
					tag[1] = (byte)('F');
					tag[2] = (byte)('I');
					tag[3] = (byte)('F');
					tag[4] = (byte)('\0');
					int ok = 1;
					int i;
					for (i = 0; (i) < (5); ++i)
					{
						if (get8(z.s) != tag[i])
						{
							ok = 0;
						}
					}

					L -= 5;
					if ((ok) != 0)
					{
						z.jfif = 1;
					}
				}
				else if (((m) == (0xEE)) && ((L) >= (12)))
				{
					byte* tag = stackalloc byte[6];
					tag[0] = (byte)('A');
					tag[1] = (byte)('d');
					tag[2] = (byte)('o');
					tag[3] = (byte)('b');
					tag[4] = (byte)('e');
					tag[5] = (byte)('\0');
					int ok = 1;
					int i;
					for (i = 0; (i) < (6); ++i)
					{
						if (get8(z.s) != tag[i])
						{
							ok = 0;
						}
					}

					L -= 6;
					if ((ok) != 0)
					{
						get8(z.s);
						get16be(z.s);
						get16be(z.s);
						z.app14_color_transform = get8(z.s);
						L -= 6;
					}
				}

				skip(z.s, L);
				return 1;
			}

			return err("unknown marker");
		}

		public static int process_scan_header(jpeg z)
		{
			int i;
			int Ls = get16be(z.s);
			z.scan_n = get8(z.s);
			if ((((z.scan_n) < (1)) || ((z.scan_n) > (4))) || ((z.scan_n) > (z.s.img_n)))
			{
				return err("bad SOS component count");
			}

			if (Ls != 6 + 2 * z.scan_n)
			{
				return err("bad SOS len");
			}

			for (i = 0; (i) < (z.scan_n); ++i)
			{
				int id = get8(z.s);
				int which;
				int q = get8(z.s);
				for (which = 0; (which) < (z.s.img_n); ++which)
				{
					if ((z.img_comp[which].id) == (id))
					{
						break;
					}
				}

				if ((which) == (z.s.img_n))
				{
					return 0;
				}

				z.img_comp[which].hd = q >> 4;
				if ((z.img_comp[which].hd) > (3))
				{
					return err("bad DC huff");
				}

				z.img_comp[which].ha = q & 15;
				if ((z.img_comp[which].ha) > (3))
				{
					return err("bad AC huff");
				}

				z.order[i] = which;
			}

			{
				int aa;
				z.spec_start = get8(z.s);
				z.spec_end = get8(z.s);
				aa = get8(z.s);
				z.succ_high = aa >> 4;
				z.succ_low = aa & 15;
				if ((z.progressive) != 0)
				{
					if ((((((z.spec_start) > (63)) || ((z.spec_end) > (63))) || ((z.spec_start) > (z.spec_end))) ||
						 ((z.succ_high) > (13))) || ((z.succ_low) > (13)))
					{
						return err("bad SOS");
					}
				}
				else
				{
					if (z.spec_start != 0)
					{
						return err("bad SOS");
					}

					if ((z.succ_high != 0) || (z.succ_low != 0))
					{
						return err("bad SOS");
					}

					z.spec_end = 63;
				}
			}

			return 1;
		}

		public static int free_jpeg_components(jpeg z, int ncomp, int why)
		{
			int i;
			for (i = 0; (i) < (ncomp); ++i)
			{
				if ((z.img_comp[i].raw_data) != null)
				{
					free(z.img_comp[i].raw_data);
					z.img_comp[i].raw_data = (null);
					z.img_comp[i].data = (null);
				}

				if ((z.img_comp[i].raw_coeff) != null)
				{
					free(z.img_comp[i].raw_coeff);
					z.img_comp[i].raw_coeff = null;
					z.img_comp[i].coeff = null;
				}

				if ((z.img_comp[i].linebuf) != null)
				{
					free(z.img_comp[i].linebuf);
					z.img_comp[i].linebuf = (null);
				}
			}

			return why;
		}

		public static int process_frame_header(jpeg z, int scan)
		{
			Context s = z.s;
			int Lf;
			int p;
			int i;
			int q;
			int h_max = 1;
			int v_max = 1;
			int c;
			Lf = get16be(s);
			if ((Lf) < (11))
			{
				return err("bad SOF len");
			}

			p = get8(s);
			if (p != 8)
			{
				return err("only 8-bit");
			}

			s.img_y = (uint)(get16be(s));
			if ((s.img_y) == (0))
			{
				return err("no header height");
			}

			s.img_x = (uint)(get16be(s));
			if ((s.img_x) == (0))
			{
				return err("0 width");
			}

			c = get8(s);
			if (((c != 3) && (c != 1)) && (c != 4))
			{
				return err("bad component count");
			}

			s.img_n = c;
			for (i = 0; (i) < (c); ++i)
			{
				z.img_comp[i].data = (null);
				z.img_comp[i].linebuf = (null);
			}

			if (Lf != 8 + 3 * s.img_n)
			{
				return err("bad SOF len");
			}

			z.rgb = 0;
			for (i = 0; (i) < (s.img_n); ++i)
			{
				byte* rgb = stackalloc byte[3];
				rgb[0] = (byte)('R');
				rgb[1] = (byte)('G');
				rgb[2] = (byte)('B');
				z.img_comp[i].id = get8(s);
				if (((s.img_n) == (3)) && ((z.img_comp[i].id) == (rgb[i])))
				{
					++z.rgb;
				}

				q = get8(s);
				z.img_comp[i].h = q >> 4;
				if ((z.img_comp[i].h == 0) || ((z.img_comp[i].h) > (4)))
				{
					return err("bad H");
				}

				z.img_comp[i].v = q & 15;
				if ((z.img_comp[i].v == 0) || ((z.img_comp[i].v) > (4)))
				{
					return err("bad V");
				}

				z.img_comp[i].tq = get8(s);
				if ((z.img_comp[i].tq) > (3))
				{
					return err("bad TQ");
				}
			}

			if (scan != SCAN_load)
			{
				return 1;
			}

			if (mad3sizes_valid((int)(s.img_x), (int)(s.img_y), s.img_n, 0) == 0)
			{
				return err("too large");
			}

			for (i = 0; (i) < (s.img_n); ++i)
			{
				if ((z.img_comp[i].h) > (h_max))
				{
					h_max = z.img_comp[i].h;
				}

				if ((z.img_comp[i].v) > (v_max))
				{
					v_max = z.img_comp[i].v;
				}
			}

			z.img_h_max = h_max;
			z.img_v_max = v_max;
			z.img_mcu_w = h_max * 8;
			z.img_mcu_h = v_max * 8;
			z.img_mcu_x = (int)((s.img_x + z.img_mcu_w - 1) / z.img_mcu_w);
			z.img_mcu_y = (int)((s.img_y + z.img_mcu_h - 1) / z.img_mcu_h);
			for (i = 0; (i) < (s.img_n); ++i)
			{
				z.img_comp[i].x = (int)((s.img_x * z.img_comp[i].h + h_max - 1) / h_max);
				z.img_comp[i].y = (int)((s.img_y * z.img_comp[i].v + v_max - 1) / v_max);
				z.img_comp[i].w2 = z.img_mcu_x * z.img_comp[i].h * 8;
				z.img_comp[i].h2 = z.img_mcu_y * z.img_comp[i].v * 8;
				z.img_comp[i].coeff = null;
				z.img_comp[i].raw_coeff = null;
				z.img_comp[i].linebuf = (null);
				z.img_comp[i].raw_data =
					malloc_mad2(z.img_comp[i].w2, z.img_comp[i].h2, 15);
				if ((z.img_comp[i].raw_data) == (null))
				{
					return free_jpeg_components(z, i + 1, err("outofmem"));
				}

				z.img_comp[i].data = (byte*)((((long)z.img_comp[i].raw_data + 15) & ~15));
				if ((z.progressive) != 0)
				{
					z.img_comp[i].coeff_w = z.img_comp[i].w2 / 8;
					z.img_comp[i].coeff_h = z.img_comp[i].h2 / 8;
					z.img_comp[i].raw_coeff = malloc_mad3(z.img_comp[i].w2, z.img_comp[i].h2,
						2,
						15);
					if ((z.img_comp[i].raw_coeff) == (null))
					{
						return free_jpeg_components(z, i + 1, err("outofmem"));
					}

					z.img_comp[i].coeff = (short*)((((long)z.img_comp[i].raw_coeff + 15) & ~15));
				}
			}

			return 1;
		}

		public static int decode_jpeg_header(jpeg z, int scan)
		{
			int m;
			z.jfif = 0;
			z.app14_color_transform = -1;
			z.marker = 0xff;
			m = get_marker(z);
			if (!((m) == (0xd8)))
			{
				return err("no SOI");
			}

			if ((scan) == (SCAN_type))
			{
				return 1;
			}

			m = get_marker(z);
			while (!((((m) == (0xc0)) || ((m) == (0xc1))) || ((m) == (0xc2))))
			{
				if (process_marker(z, m) == 0)
				{
					return 0;
				}

				m = get_marker(z);
				while ((m) == (0xff))
				{
					if ((at_eof(z.s)) != 0)
					{
						return err("no SOF");
					}

					m = get_marker(z);
				}
			}

			z.progressive = (m) == (0xc2) ? 1 : 0;
			if (process_frame_header(z, scan) == 0)
			{
				return 0;
			}

			return 1;
		}

		public static int decode_jpeg_image(jpeg j)
		{
			int m;
			for (m = 0; (m) < (4); m++)
			{
				j.img_comp[m].raw_data = (null);
				j.img_comp[m].raw_coeff = (null);
			}

			j.restart_interval = 0;
			if (decode_jpeg_header(j, SCAN_load) == 0)
			{
				return 0;
			}

			m = get_marker(j);
			while (!((m) == (0xd9)))
			{
				if (((m) == (0xda)))
				{
					if (process_scan_header(j) == 0)
					{
						return 0;
					}

					if (parse_entropy_coded_data(j) == 0)
					{
						return 0;
					}

					if ((j.marker) == (0xff))
					{
						while (at_eof(j.s) == 0)
						{
							int x = get8(j.s);
							if ((x) == (255))
							{
								j.marker = get8(j.s);
								break;
							}
						}
					}
				}
				else if (((m) == (0xdc)))
				{
					int Ld = get16be(j.s);
					uint NL = (uint)(get16be(j.s));
					if (Ld != 4)
					{
						err("bad DNL len");
					}

					if (NL != j.s.img_y)
					{
						err("bad DNL height");
					}
				}
				else
				{
					if (process_marker(j, m) == 0)
					{
						return 0;
					}
				}

				m = get_marker(j);
			}

			if ((j.progressive) != 0)
			{
				jpeg_finish(j);
			}

			return 1;
		}

		public static byte* resample_row_1(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
		{
			return in_near;
		}

		public static byte* resample_row_v_2(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
		{
			int i;
			for (i = 0; (i) < (w); ++i)
			{
				_out_[i] = ((byte)((3 * in_near[i] + in_far[i] + 2) >> 2));
			}

			return _out_;
		}

		public static byte* resample_row_h_2(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
		{
			int i;
			byte* input = in_near;
			if ((w) == (1))
			{
				_out_[0] = _out_[1] = input[0];
				return _out_;
			}

			_out_[0] = input[0];
			_out_[1] = ((byte)((input[0] * 3 + input[1] + 2) >> 2));
			for (i = 1; (i) < (w - 1); ++i)
			{
				int n = 3 * input[i] + 2;
				_out_[i * 2 + 0] = ((byte)((n + input[i - 1]) >> 2));
				_out_[i * 2 + 1] = ((byte)((n + input[i + 1]) >> 2));
			}

			_out_[i * 2 + 0] = ((byte)((input[w - 2] * 3 + input[w - 1] + 2) >> 2));
			_out_[i * 2 + 1] = input[w - 1];
			return _out_;
		}

		public static byte* resample_row_hv_2(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
		{
			int i;
			int t0;
			int t1;
			if ((w) == (1))
			{
				_out_[0] = _out_[1] = ((byte)((3 * in_near[0] + in_far[0] + 2) >> 2));
				return _out_;
			}

			t1 = 3 * in_near[0] + in_far[0];
			_out_[0] = ((byte)((t1 + 2) >> 2));
			for (i = 1; (i) < (w); ++i)
			{
				t0 = t1;
				t1 = 3 * in_near[i] + in_far[i];
				_out_[i * 2 - 1] = ((byte)((3 * t0 + t1 + 8) >> 4));
				_out_[i * 2] = ((byte)((3 * t1 + t0 + 8) >> 4));
			}

			_out_[w * 2 - 1] = ((byte)((t1 + 2) >> 2));
			return _out_;
		}

		public static byte* resample_row_generic(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
		{
			int i;
			int j;
			for (i = 0; (i) < (w); ++i)
			{
				for (j = 0; (j) < (hs); ++j)
				{
					_out_[i * hs + j] = in_near[i];
				}
			}

			return _out_;
		}

		public static void YCbCr_to_RGB_row(byte* _out_, byte* y, byte* pcb, byte* pcr, int count, int step)
		{
			int i;
			for (i = 0; (i) < (count); ++i)
			{
				int y_fixed = (y[i] << 20) + (1 << 19);
				int r;
				int g;
				int b;
				int cr = pcr[i] - 128;
				int cb = pcb[i] - 128;
				r = y_fixed + cr * (((int)((1.40200f) * 4096.0f + 0.5f)) << 8);
				g =
					(int)
					(y_fixed + (cr * -(((int)((0.71414f) * 4096.0f + 0.5f)) << 8)) +
					 ((cb * -(((int)((0.34414f) * 4096.0f + 0.5f)) << 8)) & 0xffff0000));
				b = y_fixed + cb * (((int)((1.77200f) * 4096.0f + 0.5f)) << 8);
				r >>= 20;
				g >>= 20;
				b >>= 20;
				if (((uint)(r)) > (255))
				{
					if ((r) < (0))
					{
						r = 0;
					}
					else
					{
						r = 255;
					}
				}

				if (((uint)(g)) > (255))
				{
					if ((g) < (0))
					{
						g = 0;
					}
					else
					{
						g = 255;
					}
				}

				if (((uint)(b)) > (255))
				{
					if ((b) < (0))
					{
						b = 0;
					}
					else
					{
						b = 255;
					}
				}

				_out_[0] = ((byte)(r));
				_out_[1] = ((byte)(g));
				_out_[2] = ((byte)(b));
				_out_[3] = 255;
				_out_ += step;
			}
		}

		public static void setup_jpeg(jpeg j)
		{
			j.idct_block_kernel = idct_block;
			j.YCbCr_to_RGB_kernel = YCbCr_to_RGB_row;
			j.resample_row_hv_2_kernel = resample_row_hv_2;
		}

		public static void cleanup_jpeg(jpeg j)
		{
			free_jpeg_components(j, j.s.img_n, 0);
		}

		public static byte blinn_8x8(byte x, byte y)
		{
			uint t = (uint)(x * y + 128);
			return (byte)((t + (t >> 8)) >> 8);
		}

		public static byte* load_jpeg_image(jpeg z, int* out_x, int* out_y, int* comp, int req_comp)
		{
			int n;
			int decode_n;
			int is_rgb;
			z.s.img_n = 0;
			if (((req_comp) < (0)) || ((req_comp) > (4)))
			{
				return (err("bad req_comp")) != 0 ? ((byte*)null) : (null);
			}

			if (decode_jpeg_image(z) == 0)
			{
				cleanup_jpeg(z);
				return (null);
			}

			n = (req_comp) != 0 ? req_comp : (z.s.img_n) >= (3) ? 3 : 1;
			is_rgb =
				((z.s.img_n) == (3)) &&
					   (((z.rgb) == (3)) || (((z.app14_color_transform) == (0)) && (z.jfif == 0)))
					? 1
					: 0;
			if ((((z.s.img_n) == (3)) && ((n) < (3))) && (is_rgb == 0))
			{
				decode_n = 1;
			}
			else
			{
				decode_n = z.s.img_n;
			}

			{
				int k;
				uint i;
				uint j;
				byte* output;
				byte** coutput = stackalloc byte*[4];
				Resample[] res_comp = new Resample[4];
				for (int kkk = 0; kkk < res_comp.Length; ++kkk)
				{
					res_comp[kkk] = new Resample();
				}

				for (k = 0; (k) < (decode_n); ++k)
				{
					Resample r = res_comp[k];
					z.img_comp[k].linebuf = (byte*)(malloc(z.s.img_x + 3));
					if (z.img_comp[k].linebuf == null)
					{
						cleanup_jpeg(z);
						return (err("outofmem")) != 0 ? ((byte*)null) : (null);
					}

					r.hs = z.img_h_max / z.img_comp[k].h;
					r.vs = z.img_v_max / z.img_comp[k].v;
					r.ystep = r.vs >> 1;
					r.w_lores = (int)((z.s.img_x + r.hs - 1) / r.hs);
					r.ypos = 0;
					r.line0 = r.line1 = z.img_comp[k].data;
					if (((r.hs) == (1)) && ((r.vs) == (1)))
					{
						r.resample = resample_row_1;
					}
					else if (((r.hs) == (1)) && ((r.vs) == (2)))
					{
						r.resample = resample_row_v_2;
					}
					else if (((r.hs) == (2)) && ((r.vs) == (1)))
					{
						r.resample = resample_row_h_2;
					}
					else if (((r.hs) == (2)) && ((r.vs) == (2)))
					{
						r.resample = z.resample_row_hv_2_kernel;
					}
					else
					{
						r.resample = resample_row_generic;
					}
				}

				output = (byte*)(malloc_mad3(n, (int)(z.s.img_x), (int)(z.s.img_y), 1));
				if (output == null)
				{
					cleanup_jpeg(z);
					return (err("outofmem")) != 0 ? ((byte*)null) : (null);
				}

				for (j = 0; (j) < (z.s.img_y); ++j)
				{
					byte* _out_ = output + n * z.s.img_x * j;
					for (k = 0; (k) < (decode_n); ++k)
					{
						Resample r = res_comp[k];
						int y_bot = (r.ystep) >= (r.vs >> 1) ? 1 : 0;
						coutput[k] = r.resample(z.img_comp[k].linebuf, (y_bot) != 0 ? r.line1 : r.line0,
							(y_bot) != 0 ? r.line0 : r.line1,
							r.w_lores, r.hs);
						if ((++r.ystep) >= (r.vs))
						{
							r.ystep = 0;
							r.line0 = r.line1;
							if ((++r.ypos) < (z.img_comp[k].y))
							{
								r.line1 += z.img_comp[k].w2;
							}
						}
					}

					if ((n) >= (3))
					{
						byte* y = coutput[0];
						if ((z.s.img_n) == (3))
						{
							if ((is_rgb) != 0)
							{
								for (i = 0; (i) < (z.s.img_x); ++i)
								{
									_out_[0] = y[i];
									_out_[1] = coutput[1][i];
									_out_[2] = coutput[2][i];
									_out_[3] = 255;
									_out_ += n;
								}
							}
							else
							{
								z.YCbCr_to_RGB_kernel(_out_, y, coutput[1], coutput[2], (int)(z.s.img_x), n);
							}
						}
						else if ((z.s.img_n) == (4))
						{
							if ((z.app14_color_transform) == (0))
							{
								for (i = 0; (i) < (z.s.img_x); ++i)
								{
									byte m = coutput[3][i];
									_out_[0] = blinn_8x8(coutput[0][i], m);
									_out_[1] = blinn_8x8(coutput[1][i], m);
									_out_[2] = blinn_8x8(coutput[2][i], m);
									_out_[3] = 255;
									_out_ += n;
								}
							}
							else if ((z.app14_color_transform) == (2))
							{
								z.YCbCr_to_RGB_kernel(_out_, y, coutput[1], coutput[2], (int)(z.s.img_x), n);
								for (i = 0; (i) < (z.s.img_x); ++i)
								{
									byte m = coutput[3][i];
									_out_[0] = blinn_8x8((byte)(255 - _out_[0]), m);
									_out_[1] = blinn_8x8((byte)(255 - _out_[1]), m);
									_out_[2] = blinn_8x8((byte)(255 - _out_[2]), m);
									_out_ += n;
								}
							}
							else
							{
								z.YCbCr_to_RGB_kernel(_out_, y, coutput[1], coutput[2], (int)(z.s.img_x), n);
							}
						}
						else
						{
							for (i = 0; (i) < (z.s.img_x); ++i)
							{
								_out_[0] = _out_[1] = _out_[2] = y[i];
								_out_[3] = 255;
								_out_ += n;
							}
						}
					}
					else
					{
						if ((is_rgb) != 0)
						{
							if ((n) == (1))
							{
								for (i = 0; (i) < (z.s.img_x); ++i)
								{
									*_out_++ = compute_y(coutput[0][i], coutput[1][i],
										coutput[2][i]);
								}
							}
							else
							{
								for (i = 0; (i) < (z.s.img_x); ++i, _out_ += 2)
								{
									_out_[0] = compute_y(coutput[0][i], coutput[1][i],
										coutput[2][i]);
									_out_[1] = 255;
								}
							}
						}
						else if (((z.s.img_n) == (4)) && ((z.app14_color_transform) == (0)))
						{
							for (i = 0; (i) < (z.s.img_x); ++i)
							{
								byte m = coutput[3][i];
								byte r = blinn_8x8(coutput[0][i], m);
								byte g = blinn_8x8(coutput[1][i], m);
								byte b = blinn_8x8(coutput[2][i], m);
								_out_[0] = compute_y(r, g, b);
								_out_[1] = 255;
								_out_ += n;
							}
						}
						else if (((z.s.img_n) == (4)) && ((z.app14_color_transform) == (2)))
						{
							for (i = 0; (i) < (z.s.img_x); ++i)
							{
								_out_[0] =
									blinn_8x8((byte)(255 - coutput[0][i]), coutput[3][i]);
								_out_[1] = 255;
								_out_ += n;
							}
						}
						else
						{
							byte* y = coutput[0];
							if ((n) == (1))
							{
								for (i = 0; (i) < (z.s.img_x); ++i)
								{
									_out_[i] = y[i];
								}
							}
							else
							{
								for (i = 0; (i) < (z.s.img_x); ++i)
								{
									*_out_++ = y[i];
									*_out_++ = 255;
								}
							}
						}
					}
				}

				cleanup_jpeg(z);
				*out_x = (int)(z.s.img_x);
				*out_y = (int)(z.s.img_y);
				if ((comp) != null)
				{
					*comp = (z.s.img_n) >= (3) ? 3 : 1;
				}

				return output;
			}

		}

		public static void* jpeg_load(Context s, int* x, int* y, int* comp, int req_comp,
			result_info* ri)
		{
			byte* result;
			jpeg j = new jpeg
			{
				s = s
			};
			setup_jpeg(j);
			result = load_jpeg_image(j, x, y, comp, req_comp);

			return result;
		}

		public static int jpeg_test(Context s)
		{
			int r;
			jpeg j = new jpeg
			{
				s = s
			};
			setup_jpeg(j);
			r = decode_jpeg_header(j, SCAN_type);
			rewind(s);

			return r;
		}

		public static int jpeg_info_raw(jpeg j, int* x, int* y, int* comp)
		{
			if (decode_jpeg_header(j, SCAN_header) == 0)
			{
				rewind(j.s);
				return 0;
			}

			if ((x) != null)
			{
				*x = (int)(j.s.img_x);
			}

			if ((y) != null)
			{
				*y = (int)(j.s.img_y);
			}

			if ((comp) != null)
			{
				*comp = (j.s.img_n) >= (3) ? 3 : 1;
			}

			return 1;
		}

		public static int jpeg_info(Context s, int* x, int* y, int* comp)
		{
			int result;
			jpeg j = new jpeg
			{
				s = s
			};
			result = jpeg_info_raw(j, x, y, comp);

			return result;
		}

		public static int bitreverse16(int n)
		{
			n = ((n & 0xAAAA) >> 1) | ((n & 0x5555) << 1);
			n = ((n & 0xCCCC) >> 2) | ((n & 0x3333) << 2);
			n = ((n & 0xF0F0) >> 4) | ((n & 0x0F0F) << 4);
			n = ((n & 0xFF00) >> 8) | ((n & 0x00FF) << 8);
			return n;
		}

		public static int bit_reverse(int v, int bits)
		{
			return bitreverse16(v) >> (16 - bits);
		}

		public static int zbuild_huffman(zhuffman* z, byte* sizelist, int num)
		{
			int i;
			int k = 0;
			int code;
			int* next_code = stackalloc int[16];
			int* sizes = stackalloc int[17];
			memset(sizes, 0, (ulong)(sizeof(int)));
			memset(z->fast, 0, (ulong)((1 << 9) * sizeof(ushort)));
			for (i = 0; (i) < (num); ++i)
			{
				++sizes[sizelist[i]];
			}

			sizes[0] = 0;
			for (i = 1; (i) < (16); ++i)
			{
				if ((sizes[i]) > (1 << i))
				{
					return err("bad sizes");
				}
			}

			code = 0;
			for (i = 1; (i) < (16); ++i)
			{
				next_code[i] = code;
				z->firstcode[i] = ((ushort)(code));
				z->firstsymbol[i] = ((ushort)(k));
				code = code + sizes[i];
				if ((sizes[i]) != 0)
				{
					if ((code - 1) >= (1 << i))
					{
						return err("bad codelengths");
					}
				}

				z->maxcode[i] = code << (16 - i);
				code <<= 1;
				k += sizes[i];
			}

			z->maxcode[16] = 0x10000;
			for (i = 0; (i) < (num); ++i)
			{
				int s = sizelist[i];
				if ((s) != 0)
				{
					int c = next_code[s] - z->firstcode[s] + z->firstsymbol[s];
					ushort fastv = (ushort)((s << 9) | i);
					z->size[c] = ((byte)(s));
					z->value[c] = ((ushort)(i));
					if (s <= 9)
					{
						int j = bit_reverse(next_code[s], s);
						while ((j) < (1 << 9))
						{
							z->fast[j] = fastv;
							j += 1 << s;
						}
					}

					++next_code[s];
				}
			}

			return 1;
		}

		public static byte zget8(zbuf* z)
		{
			if ((z->zbuffer) >= (z->zbuffer_end))
			{
				return 0;
			}

			return *z->zbuffer++;
		}

		public static void fill_bits(zbuf* z)
		{
			do
			{
				z->code_buffer |= (uint)(zget8(z)) << z->num_bits;
				z->num_bits += 8;
			} while (z->num_bits <= 24);
		}

		public static uint zreceive(zbuf* z, int n)
		{
			uint k;
			if ((z->num_bits) < (n))
			{
				fill_bits(z);
			}

			k = (uint)(z->code_buffer & ((1 << n) - 1));
			z->code_buffer >>= n;
			z->num_bits -= n;
			return k;
		}

		public static int zhuffman_decode_slowpath(zbuf* a, zhuffman* z)
		{
			int b;
			int s;
			int k;
			k = bit_reverse((int)(a->code_buffer), 16);
			for (s = 9 + 1; ; ++s)
			{
				if ((k) < (z->maxcode[s]))
				{
					break;
				}
			}

			if ((s) == (16))
			{
				return -1;
			}

			b = (k >> (16 - s)) - z->firstcode[s] + z->firstsymbol[s];
			a->code_buffer >>= s;
			a->num_bits -= s;
			return z->value[b];
		}

		public static int zhuffman_decode(zbuf* a, zhuffman* z)
		{
			int b;
			int s;
			if ((a->num_bits) < (16))
			{
				fill_bits(a);
			}

			b = z->fast[a->code_buffer & ((1 << 9) - 1)];
			if ((b) != 0)
			{
				s = b >> 9;
				a->code_buffer >>= s;
				a->num_bits -= s;
				return b & 511;
			}

			return zhuffman_decode_slowpath(a, z);
		}

		public static int zexpand(zbuf* z, sbyte* zout, int n)
		{
			sbyte* q;
			int cur;
			int limit;
			int old_limit;
			z->zout = zout;
			if (z->z_expandable == 0)
			{
				return err("output buffer limit");
			}

			cur = ((int)(z->zout - z->zout_start));
			limit = old_limit = ((int)(z->zout_end - z->zout_start));
			while ((cur + n) > (limit))
			{
				limit *= 2;
			}

			q = (sbyte*)(realloc(z->zout_start, (ulong)(limit)));
			if ((q) == (null))
			{
				return err("outofmem");
			}

			z->zout_start = q;
			z->zout = q + cur;
			z->zout_end = q + limit;
			return 1;
		}

		public static int parse_huffman_block(zbuf* a)
		{
			sbyte* zout = a->zout;
			for (; ; )
			{
				int z = zhuffman_decode(a, &a->z_length);
				if ((z) < (256))
				{
					if ((z) < (0))
					{
						return err("bad huffman code");
					}

					if ((zout) >= (a->zout_end))
					{
						if (zexpand(a, zout, 1) == 0)
						{
							return 0;
						}

						zout = a->zout;
					}

					*zout++ = ((sbyte)(z));
				}
				else
				{
					byte* p;
					int len;
					int dist;
					if ((z) == (256))
					{
						a->zout = zout;
						return 1;
					}

					z -= 257;
					len = zlength_base[z];
					if ((zlength_extra[z]) != 0)
					{
						len += (int)(zreceive(a, zlength_extra[z]));
					}

					z = zhuffman_decode(a, &a->z_distance);
					if ((z) < (0))
					{
						return err("bad huffman code");
					}

					dist = zdist_base[z];
					if ((zdist_extra[z]) != 0)
					{
						dist += (int)(zreceive(a, zdist_extra[z]));
					}

					if ((zout - a->zout_start) < (dist))
					{
						return err("bad dist");
					}

					if ((zout + len) > (a->zout_end))
					{
						if (zexpand(a, zout, len) == 0)
						{
							return 0;
						}

						zout = a->zout;
					}

					p = (byte*)(zout - dist);
					if ((dist) == (1))
					{
						byte v = *p;
						if ((len) != 0)
						{
							do
							{
								*zout++ = (sbyte)(v);
							}
							while ((--len) != 0);
						}
					}
					else
					{
						if ((len) != 0)
						{
							do
							{
								*zout++ = (sbyte)(*p++);
							}
							while ((--len) != 0);
						}
					}
				}
			}
		}

		public static int compute_huffman_codes(zbuf* a)
		{
			zhuffman z_codelength = new zhuffman();
			byte* lencodes = stackalloc byte[286 + 32 + 137];
			byte* codelength_sizes = stackalloc byte[19];
			int i;
			int n;
			int hlit = (int)(zreceive(a, 5) + 257);
			int hdist = (int)(zreceive(a, 5) + 1);
			int hclen = (int)(zreceive(a, 4) + 4);
			int ntot = hlit + hdist;
			memset(codelength_sizes, 0, (ulong)(19 * sizeof(byte)));
			for (i = 0; (i) < (hclen); ++i)
			{
				int s = (int)(zreceive(a, 3));
				codelength_sizes[length_dezigzag[i]] = ((byte)(s));
			}

			if (zbuild_huffman(&z_codelength, codelength_sizes, 19) == 0)
			{
				return 0;
			}

			n = 0;
			while ((n) < (ntot))
			{
				int c = zhuffman_decode(a, &z_codelength);
				if (((c) < (0)) || ((c) >= (19)))
				{
					return err("bad codelengths");
				}

				if ((c) < (16))
				{
					lencodes[n++] = ((byte)(c));
				}
				else
				{
					byte fill = 0;
					if ((c) == (16))
					{
						c = (int)(zreceive(a, 2) + 3);
						if ((n) == (0))
						{
							return err("bad codelengths");
						}

						fill = lencodes[n - 1];
					}
					else if ((c) == (17))
					{
						c = (int)(zreceive(a, 3) + 3);
					}
					else
					{
						c = (int)(zreceive(a, 7) + 11);
					}

					if ((ntot - n) < (c))
					{
						return err("bad codelengths");
					}

					memset(lencodes + n, fill, (ulong)(c));
					n += c;
				}
			}

			if (n != ntot)
			{
				return err("bad codelengths");
			}

			if (zbuild_huffman(&a->z_length, lencodes, hlit) == 0)
			{
				return 0;
			}

			if (zbuild_huffman(&a->z_distance, lencodes + hlit, hdist) == 0)
			{
				return 0;
			}

			return 1;
		}

		public static int parse_uncompressed_block(zbuf* a)
		{
			byte* header = stackalloc byte[4];
			int len;
			int nlen;
			int k;
			if ((a->num_bits & 7) != 0)
			{
				zreceive(a, a->num_bits & 7);
			}

			k = 0;
			while ((a->num_bits) > (0))
			{
				header[k++] = ((byte)(a->code_buffer & 255));
				a->code_buffer >>= 8;
				a->num_bits -= 8;
			}

			while ((k) < (4))
			{
				header[k++] = zget8(a);
			}

			len = header[1] * 256 + header[0];
			nlen = header[3] * 256 + header[2];
			if (nlen != (len ^ 0xffff))
			{
				return err("zlib corrupt");
			}

			if ((a->zbuffer + len) > (a->zbuffer_end))
			{
				return err("read past buffer");
			}

			if ((a->zout + len) > (a->zout_end))
			{
				if (zexpand(a, a->zout, len) == 0)
				{
					return 0;
				}
			}

			memcpy(a->zout, a->zbuffer, (ulong)(len));
			a->zbuffer += len;
			a->zout += len;
			return 1;
		}

		public static int parse_zlib_header(zbuf* a)
		{
			int cmf = zget8(a);
			int cm = cmf & 15;
			int flg = zget8(a);
			if ((cmf * 256 + flg) % 31 != 0)
			{
				return err("bad zlib header");
			}

			if ((flg & 32) != 0)
			{
				return err("no preset dict");
			}

			if (cm != 8)
			{
				return err("bad compression");
			}

			return 1;
		}

		public static int parse_zlib(zbuf* a, int parse_header)
		{
			int final;
			int type;
			if ((parse_header) != 0)
			{
				if (parse_zlib_header(a) == 0)
				{
					return 0;
				}
			}

			a->num_bits = 0;
			a->code_buffer = 0;
			do
			{
				final = (int)(zreceive(a, 1));
				type = (int)(zreceive(a, 2));
				if ((type) == (0))
				{
					if (parse_uncompressed_block(a) == 0)
					{
						return 0;
					}
				}
				else if ((type) == (3))
				{
					return 0;
				}
				else
				{
					if ((type) == (1))
					{
						fixed (byte* b = zdefault_length)
						{
							if (zbuild_huffman(&a->z_length, b, 288) == 0)
							{
								return 0;
							}
						}

						fixed (byte* b = zdefault_distance)
						{
							if (zbuild_huffman(&a->z_distance, b, 32) == 0)
							{
								return 0;
							}
						}
					}
					else
					{
						if (compute_huffman_codes(a) == 0)
						{
							return 0;
						}
					}

					if (parse_huffman_block(a) == 0)
					{
						return 0;
					}
				}
			} while (final == 0);

			return 1;
		}

		public static int do_zlib(zbuf* a, sbyte* obuf, int olen, int exp, int parse_header)
		{
			a->zout_start = obuf;
			a->zout = obuf;
			a->zout_end = obuf + olen;
			a->z_expandable = exp;
			return parse_zlib(a, parse_header);
		}

		public static sbyte* zlib_decode_malloc_guesssize(sbyte* buffer, int len, int initial_size, int* outlen)
		{
			zbuf a = new zbuf();
			sbyte* p = (sbyte*)(malloc((ulong)(initial_size)));
			if ((p) == (null))
			{
				return (null);
			}

			a.zbuffer = (byte*)(buffer);
			a.zbuffer_end = (byte*)(buffer) + len;
			if ((do_zlib(&a, p, initial_size, 1, 1)) != 0)
			{
				if ((outlen) != null)
				{
					*outlen = ((int)(a.zout - a.zout_start));
				}

				return a.zout_start;
			}
			else
			{
				free(a.zout_start);
				return (null);
			}

		}

		public static sbyte* zlib_decode_malloc(sbyte* buffer, int len, int* outlen)
		{
			return zlib_decode_malloc_guesssize(buffer, len, 16384, outlen);
		}

		public static sbyte* zlib_decode_malloc_guesssize_headerflag(sbyte* buffer, int len, int initial_size,
			int* outlen, int parse_header)
		{
			zbuf a = new zbuf();
			sbyte* p = (sbyte*)(malloc((ulong)(initial_size)));
			if ((p) == (null))
			{
				return (null);
			}

			a.zbuffer = (byte*)(buffer);
			a.zbuffer_end = (byte*)(buffer) + len;
			if ((do_zlib(&a, p, initial_size, 1, parse_header)) != 0)
			{
				if ((outlen) != null)
				{
					*outlen = ((int)(a.zout - a.zout_start));
				}

				return a.zout_start;
			}
			else
			{
				free(a.zout_start);
				return (null);
			}

		}

		public static int zlib_decode_buffer(sbyte* obuffer, int olen, sbyte* ibuffer, int ilen)
		{
			zbuf a = new zbuf
			{
				zbuffer = (byte*)(ibuffer),
				zbuffer_end = (byte*)(ibuffer) + ilen
			};
			if ((do_zlib(&a, obuffer, olen, 0, 1)) != 0)
			{
				return (int)(a.zout - a.zout_start);
			}
			else
			{
				return -1;
			}
		}

		public static sbyte* zlib_decode_noheader_malloc(sbyte* buffer, int len, int* outlen)
		{
			zbuf a = new zbuf();
			sbyte* p = (sbyte*)(malloc((ulong)(16384)));
			if ((p) == (null))
			{
				return (null);
			}

			a.zbuffer = (byte*)(buffer);
			a.zbuffer_end = (byte*)(buffer) + len;
			if ((do_zlib(&a, p, 16384, 1, 0)) != 0)
			{
				if ((outlen) != null)
				{
					*outlen = ((int)(a.zout - a.zout_start));
				}

				return a.zout_start;
			}
			else
			{
				free(a.zout_start);
				return (null);
			}

		}

		public static int zlib_decode_noheader_buffer(sbyte* obuffer, int olen, sbyte* ibuffer, int ilen)
		{
			zbuf a = new zbuf
			{
				zbuffer = (byte*)(ibuffer),
				zbuffer_end = (byte*)(ibuffer) + ilen
			};
			if ((do_zlib(&a, obuffer, olen, 0, 0)) != 0)
			{
				return (int)(a.zout - a.zout_start);
			}
			else
			{
				return -1;
			}
		}

		public static pngchunk get_chunk_header(Context s)
		{
			pngchunk c = new pngchunk
			{
				length = get32be(s),
				type = get32be(s)
			};
			return c;
		}

		public static int check_png_header(Context s)
		{
			int i;
			for (i = 0; (i) < (8); ++i)
			{
				if (get8(s) != png_sig[i])
				{
					return err("bad png sig");
				}
			}

			return 1;
		}

		public static int paeth(int a, int b, int c)
		{
			int p = a + b - c;
			int pa = Math.Abs(p - a);
			int pb = Math.Abs(p - b);
			int pc = Math.Abs(p - c);
			if ((pa <= pb) && (pa <= pc))
			{
				return a;
			}

			if (pb <= pc)
			{
				return b;
			}

			return c;
		}

		public static int create_png_image_raw(png a, byte* raw, uint raw_len, int out_n, uint x, uint y,
			int depth, int color)
		{
			int bytes = (depth) == (16) ? 2 : 1;
			Context s = a.s;
			uint i;
			uint j;
			uint stride = (uint)(x * out_n * bytes);
			uint img_len;
			uint img_width_bytes;
			int k;
			int img_n = s.img_n;
			int output_bytes = out_n * bytes;
			int filter_bytes = img_n * bytes;
			int width = (int)(x);
			a._out_ = (byte*)(malloc_mad3((int)(x), (int)(y), output_bytes, 0));
			if (a._out_ == null)
			{
				return err("outofmem");
			}

			img_width_bytes = (uint)(((img_n * x * depth) + 7) >> 3);
			img_len = (img_width_bytes + 1) * y;
			if ((raw_len) < (img_len))
			{
				return err("not enough pixels");
			}

			for (j = 0; (j) < (y); ++j)
			{
				byte* cur = a._out_ + stride * j;
				byte* prior;
				int filter = *raw++;
				if ((filter) > (4))
				{
					return err("invalid filter");
				}

				if ((depth) < (8))
				{
					cur += x * out_n - img_width_bytes;
					filter_bytes = 1;
					width = (int)(img_width_bytes);
				}

				prior = cur - stride;
				if ((j) == (0))
				{
					filter = first_row_filter[filter];
				}

				for (k = 0; (k) < (filter_bytes); ++k)
				{
					switch (filter)
					{
						case F_none:
							cur[k] = raw[k];
							break;
						case F_sub:
							cur[k] = raw[k];
							break;
						case F_up:
							cur[k] = ((byte)((raw[k] + prior[k]) & 255));
							break;
						case F_avg:
							cur[k] = ((byte)((raw[k] + (prior[k] >> 1)) & 255));
							break;
						case F_paeth:
							cur[k] = ((byte)((raw[k] + paeth(0, prior[k], 0)) & 255));
							break;
						case F_avg_first:
							cur[k] = raw[k];
							break;
						case F_paeth_first:
							cur[k] = raw[k];
							break;
					}
				}

				if ((depth) == (8))
				{
					if (img_n != out_n)
					{
						cur[img_n] = 255;
					}

					raw += img_n;
					cur += out_n;
					prior += out_n;
				}
				else if ((depth) == (16))
				{
					if (img_n != out_n)
					{
						cur[filter_bytes] = 255;
						cur[filter_bytes + 1] = 255;
					}

					raw += filter_bytes;
					cur += output_bytes;
					prior += output_bytes;
				}
				else
				{
					raw += 1;
					cur += 1;
					prior += 1;
				}

				if (((depth) < (8)) || ((img_n) == (out_n)))
				{
					int nk = (width - 1) * filter_bytes;
					switch (filter)
					{
						case F_none:
							memcpy(cur, raw, (ulong)(nk));
							break;
						case F_sub:
							for (k = 0; (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + cur[k - filter_bytes]) & 255));
							}

							break;
						case F_up:
							for (k = 0; (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + prior[k]) & 255));
							}

							break;
						case F_avg:
							for (k = 0; (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + ((prior[k] + cur[k - filter_bytes]) >> 1)) & 255));
							}

							break;
						case F_paeth:
							for (k = 0; (k) < (nk); ++k)
							{
								cur[k] =
									((byte)
										((raw[k] + paeth(cur[k - filter_bytes], prior[k],
											  prior[k - filter_bytes])) &
										 255));
							}

							break;
						case F_avg_first:
							for (k = 0; (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + (cur[k - filter_bytes] >> 1)) & 255));
							}

							break;
						case F_paeth_first:
							for (k = 0; (k) < (nk); ++k)
							{
								cur[k] = ((byte)((raw[k] + paeth(cur[k - filter_bytes], 0,
													   0)) & 255));
							}

							break;
					}

					raw += nk;
				}
				else
				{
					switch (filter)
					{
						case F_none:
							for (i = x - 1;
								(i) >= (1);
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes,
								prior += output_bytes)
							{
								for (k = 0; (k) < (filter_bytes); ++k)
								{
									cur[k] = raw[k];
								}
							}

							break;
						case F_sub:
							for (i = x - 1;
								(i) >= (1);
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes,
								prior += output_bytes)
							{
								for (k = 0; (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + cur[k - output_bytes]) & 255));
								}
							}

							break;
						case F_up:
							for (i = x - 1;
								(i) >= (1);
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes,
								prior += output_bytes)
							{
								for (k = 0; (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + prior[k]) & 255));
								}
							}

							break;
						case F_avg:
							for (i = x - 1;
								(i) >= (1);
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes,
								prior += output_bytes)
							{
								for (k = 0; (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + ((prior[k] + cur[k - output_bytes]) >> 1)) & 255));
								}
							}

							break;
						case F_paeth:
							for (i = x - 1;
								(i) >= (1);
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes,
								prior += output_bytes)
							{
								for (k = 0; (k) < (filter_bytes); ++k)
								{
									cur[k] =
										((byte)
											((raw[k] + paeth(cur[k - output_bytes], prior[k],
												  prior[k - output_bytes])) &
											 255));
								}
							}

							break;
						case F_avg_first:
							for (i = x - 1;
								(i) >= (1);
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes,
								prior += output_bytes)
							{
								for (k = 0; (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + (cur[k - output_bytes] >> 1)) & 255));
								}
							}

							break;
						case F_paeth_first:
							for (i = x - 1;
								(i) >= (1);
								--i, cur[filter_bytes] = 255, raw += filter_bytes, cur += output_bytes,
								prior += output_bytes)
							{
								for (k = 0; (k) < (filter_bytes); ++k)
								{
									cur[k] = ((byte)((raw[k] + paeth(cur[k - output_bytes], 0,
														   0)) & 255));
								}
							}

							break;
					}

					if ((depth) == (16))
					{
						cur = a._out_ + stride * j;
						for (i = 0; (i) < (x); ++i, cur += output_bytes)
						{
							cur[filter_bytes + 1] = 255;
						}
					}
				}
			}

			if ((depth) < (8))
			{
				for (j = 0; (j) < (y); ++j)
				{
					byte* cur = a._out_ + stride * j;
					byte* _in_ = a._out_ + stride * j + x * out_n - img_width_bytes;
					byte scale = (byte)(((color) == (0)) ? depth_scale_table[depth] : 1);
					if ((depth) == (4))
					{
						for (k = (int)(x * img_n); (k) >= (2); k -= 2, ++_in_)
						{
							*cur++ = (byte)(scale * (*_in_ >> 4));
							*cur++ = (byte)(scale * ((*_in_) & 0x0f));
						}

						if ((k) > (0))
						{
							*cur++ = (byte)(scale * (*_in_ >> 4));
						}
					}
					else if ((depth) == (2))
					{
						for (k = (int)(x * img_n); (k) >= (4); k -= 4, ++_in_)
						{
							*cur++ = (byte)(scale * (*_in_ >> 6));
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x03));
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x03));
							*cur++ = (byte)(scale * ((*_in_) & 0x03));
						}

						if ((k) > (0))
						{
							*cur++ = (byte)(scale * (*_in_ >> 6));
						}

						if ((k) > (1))
						{
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x03));
						}

						if ((k) > (2))
						{
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x03));
						}
					}
					else if ((depth) == (1))
					{
						for (k = (int)(x * img_n); (k) >= (8); k -= 8, ++_in_)
						{
							*cur++ = (byte)(scale * (*_in_ >> 7));
							*cur++ = (byte)(scale * ((*_in_ >> 6) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 5) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 3) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x01));
							*cur++ = (byte)(scale * ((*_in_ >> 1) & 0x01));
							*cur++ = (byte)(scale * ((*_in_) & 0x01));
						}

						if ((k) > (0))
						{
							*cur++ = (byte)(scale * (*_in_ >> 7));
						}

						if ((k) > (1))
						{
							*cur++ = (byte)(scale * ((*_in_ >> 6) & 0x01));
						}

						if ((k) > (2))
						{
							*cur++ = (byte)(scale * ((*_in_ >> 5) & 0x01));
						}

						if ((k) > (3))
						{
							*cur++ = (byte)(scale * ((*_in_ >> 4) & 0x01));
						}

						if ((k) > (4))
						{
							*cur++ = (byte)(scale * ((*_in_ >> 3) & 0x01));
						}

						if ((k) > (5))
						{
							*cur++ = (byte)(scale * ((*_in_ >> 2) & 0x01));
						}

						if ((k) > (6))
						{
							*cur++ = (byte)(scale * ((*_in_ >> 1) & 0x01));
						}
					}

					if (img_n != out_n)
					{
						int q;
						cur = a._out_ + stride * j;
						if ((img_n) == (1))
						{
							for (q = (int)(x - 1); (q) >= (0); --q)
							{
								cur[q * 2 + 1] = 255;
								cur[q * 2 + 0] = cur[q];
							}
						}
						else
						{
							for (q = (int)(x - 1); (q) >= (0); --q)
							{
								cur[q * 4 + 3] = 255;
								cur[q * 4 + 2] = cur[q * 3 + 2];
								cur[q * 4 + 1] = cur[q * 3 + 1];
								cur[q * 4 + 0] = cur[q * 3 + 0];
							}
						}
					}
				}
			}
			else if ((depth) == (16))
			{
				byte* cur = a._out_;
				ushort* cur16 = (ushort*)(cur);
				for (i = 0; (i) < (x * y * out_n); ++i, cur16++, cur += 2)
				{
					*cur16 = (ushort)((cur[0] << 8) | cur[1]);
				}
			}

			return 1;
		}

		public static int create_png_image(png a, byte* image_data, uint image_data_len, int out_n,
			int depth,
			int color, int interlaced)
		{
			int bytes = (depth) == (16) ? 2 : 1;
			int out_bytes = out_n * bytes;
			byte* final;
			int p;
			if (interlaced == 0)
			{
				return

					create_png_image_raw(a, image_data, image_data_len, out_n,
						a.s.img_x,
						a.s.img_y, depth, color);
			}

			final = (byte*)(malloc_mad3((int)(a.s.img_x), (int)(a.s.img_y), out_bytes, 0));
			for (p = 0; (p) < (7); ++p)
			{
				int* xorig = stackalloc int[7];
				xorig[0] = 0;
				xorig[1] = 4;
				xorig[2] = 0;
				xorig[3] = 2;
				xorig[4] = 0;
				xorig[5] = 1;
				xorig[6] = 0;
				int* yorig = stackalloc int[7];
				yorig[0] = 0;
				yorig[1] = 0;
				yorig[2] = 4;
				yorig[3] = 0;
				yorig[4] = 2;
				yorig[5] = 0;
				yorig[6] = 1;
				int* xspc = stackalloc int[7];
				xspc[0] = 8;
				xspc[1] = 8;
				xspc[2] = 4;
				xspc[3] = 4;
				xspc[4] = 2;
				xspc[5] = 2;
				xspc[6] = 1;
				int* yspc = stackalloc int[7];
				yspc[0] = 8;
				yspc[1] = 8;
				yspc[2] = 8;
				yspc[3] = 4;
				yspc[4] = 4;
				yspc[5] = 2;
				yspc[6] = 2;
				int i;
				int j;
				int x;
				int y;
				x = (int)((a.s.img_x - xorig[p] + xspc[p] - 1) / xspc[p]);
				y = (int)((a.s.img_y - yorig[p] + yspc[p] - 1) / yspc[p]);
				if (((x) != 0) && ((y) != 0))
				{
					uint img_len = (uint)(((((a.s.img_n * x * depth) + 7) >> 3) + 1) * y);
					if (
						create_png_image_raw(a, image_data, image_data_len, out_n, (uint)(x),
							(uint)(y),
							depth, color) == 0)
					{
						free(final);
						return 0;
					}

					for (j = 0; (j) < (y); ++j)
					{
						for (i = 0; (i) < (x); ++i)
						{
							int out_y = j * yspc[p] + yorig[p];
							int out_x = i * xspc[p] + xorig[p];
							memcpy(final + out_y * a.s.img_x * out_bytes + out_x * out_bytes,
								a._out_ + (j * x + i) * out_bytes,
								(ulong)(out_bytes));
						}
					}

					free(a._out_);
					image_data += img_len;
					image_data_len -= img_len;
				}
			}

			a._out_ = final;
			return 1;
		}

		public static int compute_transparency(png z, byte* tc, int out_n)
		{
			Context s = z.s;
			uint i;
			uint pixel_count = s.img_x * s.img_y;
			byte* p = z._out_;
			if ((out_n) == (2))
			{
				for (i = 0; (i) < (pixel_count); ++i)
				{
					p[1] = (byte)((p[0]) == (tc[0]) ? 0 : 255);
					p += 2;
				}
			}
			else
			{
				for (i = 0; (i) < (pixel_count); ++i)
				{
					if ((((p[0]) == (tc[0])) && ((p[1]) == (tc[1]))) && ((p[2]) == (tc[2])))
					{
						p[3] = 0;
					}

					p += 4;
				}
			}

			return 1;
		}

		public static int compute_transparency16(png z, ushort* tc, int out_n)
		{
			Context s = z.s;
			uint i;
			uint pixel_count = s.img_x * s.img_y;
			ushort* p = (ushort*)(z._out_);
			if ((out_n) == (2))
			{
				for (i = 0; (i) < (pixel_count); ++i)
				{
					p[1] = (ushort)((p[0]) == (tc[0]) ? 0 : 65535);
					p += 2;
				}
			}
			else
			{
				for (i = 0; (i) < (pixel_count); ++i)
				{
					if ((((p[0]) == (tc[0])) && ((p[1]) == (tc[1]))) && ((p[2]) == (tc[2])))
					{
						p[3] = 0;
					}

					p += 4;
				}
			}

			return 1;
		}

		public static int expand_png_palette(png a, byte* palette, int len, int pal_img_n)
		{
			uint i;
			uint pixel_count = a.s.img_x * a.s.img_y;
			byte* p;
			byte* temp_out;
			byte* orig = a._out_;
			p = (byte*)(malloc_mad2((int)(pixel_count), pal_img_n, 0));
			if ((p) == (null))
			{
				return err("outofmem");
			}

			temp_out = p;
			if ((pal_img_n) == (3))
			{
				for (i = 0; (i) < (pixel_count); ++i)
				{
					int n = orig[i] * 4;
					p[0] = palette[n];
					p[1] = palette[n + 1];
					p[2] = palette[n + 2];
					p += 3;
				}
			}
			else
			{
				for (i = 0; (i) < (pixel_count); ++i)
				{
					int n = orig[i] * 4;
					p[0] = palette[n];
					p[1] = palette[n + 1];
					p[2] = palette[n + 2];
					p[3] = palette[n + 3];
					p += 4;
				}
			}

			free(a._out_);
			a._out_ = temp_out;
			return 1;
		}

		public static void set_unpremultiply_on_load(int flag_true_if_should_unpremultiply)
		{
			unpremultiply_on_load = flag_true_if_should_unpremultiply;
		}

		public static void convert_iphone_png_to_rgb(int flag_true_if_should_convert)
		{
			de_iphone_flag = flag_true_if_should_convert;
		}

		public static void de_iphone(png z)
		{
			Context s = z.s;
			uint i;
			uint pixel_count = s.img_x * s.img_y;
			byte* p = z._out_;
			if ((s.img_out_n) == (3))
			{
				for (i = 0; (i) < (pixel_count); ++i)
				{
					byte t = p[0];
					p[0] = p[2];
					p[2] = t;
					p += 3;
				}
			}
			else
			{
				if ((unpremultiply_on_load) != 0)
				{
					for (i = 0; (i) < (pixel_count); ++i)
					{
						byte a = p[3];
						byte t = p[0];
						if ((a) != 0)
						{
							byte half = (byte)(a / 2);
							p[0] = (byte)((p[2] * 255 + half) / a);
							p[1] = (byte)((p[1] * 255 + half) / a);
							p[2] = (byte)((t * 255 + half) / a);
						}
						else
						{
							p[0] = p[2];
							p[2] = t;
						}

						p += 4;
					}
				}
				else
				{
					for (i = 0; (i) < (pixel_count); ++i)
					{
						byte t = p[0];
						p[0] = p[2];
						p[2] = t;
						p += 4;
					}
				}
			}

		}

		public static int parse_png_file(png z, int scan, int req_comp)
		{
			byte* palette = stackalloc byte[1024];
			byte pal_img_n = 0;
			byte has_trans = 0;
			byte* tc = stackalloc byte[3];
			ushort* tc16 = stackalloc ushort[3];
			uint ioff = 0;
			uint idata_limit = 0;
			uint i;
			uint pal_len = 0;
			int first = 1;
			int k;
			int interlace = 0;
			int color = 0;
			int is_iphone = 0;
			Context s = z.s;
			z.expanded = (null);
			z.idata = (null);
			z._out_ = (null);
			if (check_png_header(s) == 0)
			{
				return 0;
			}

			if ((scan) == (SCAN_type))
			{
				return 1;
			}

			for (; ; )
			{
				pngchunk c = get_chunk_header(s);
				switch (c.type)
				{
					case ((('C') << 24) + (('g') << 16) + (('B') << 8) + ('I')):
						is_iphone = 1;
						skip(s, (int)(c.length));
						break;
					case ((('I') << 24) + (('H') << 16) + (('D') << 8) + ('R')):
						{
							int comp;
							int filter;
							if (first == 0)
							{
								return err("multiple IHDR");
							}

							first = 0;
							if (c.length != 13)
							{
								return err("bad IHDR len");
							}

							s.img_x = get32be(s);
							if ((s.img_x) > (1 << 24))
							{
								return err("too large");
							}

							s.img_y = get32be(s);
							if ((s.img_y) > (1 << 24))
							{
								return err("too large");
							}

							z.depth = get8(s);
							if (((((z.depth != 1) && (z.depth != 2)) && (z.depth != 4)) && (z.depth != 8)) &&
								(z.depth != 16))
							{
								return err("1/2/4/8/16-bit only");
							}

							color = get8(s);
							if ((color) > (6))
							{
								return err("bad ctype");
							}

							if (((color) == (3)) && ((z.depth) == (16)))
							{
								return err("bad ctype");
							}

							if ((color) == (3))
							{
								pal_img_n = 3;
							}
							else if ((color & 1) != 0)
							{
								return err("bad ctype");
							}

							comp = get8(s);
							if ((comp) != 0)
							{
								return err("bad comp method");
							}

							filter = get8(s);
							if ((filter) != 0)
							{
								return err("bad filter method");
							}

							interlace = get8(s);
							if ((interlace) > (1))
							{
								return err("bad interlace method");
							}

							if ((s.img_x == 0) || (s.img_y == 0))
							{
								return err("0-pixel image");
							}

							if (pal_img_n == 0)
							{
								s.img_n = ((color & 2) != 0 ? 3 : 1) + ((color & 4) != 0 ? 1 : 0);
								if (((1 << 30) / s.img_x / s.img_n) < (s.img_y))
								{
									return err("too large");
								}

								if ((scan) == (SCAN_header))
								{
									return 1;
								}
							}
							else
							{
								s.img_n = 1;
								if (((1 << 30) / s.img_x / 4) < (s.img_y))
								{
									return err("too large");
								}
							}

							break;
						}
					case ((('P') << 24) + (('L') << 16) + (('T') << 8) + ('E')):
						{
							if ((first) != 0)
							{
								return err("first not IHDR");
							}

							if ((c.length) > (256 * 3))
							{
								return err("invalid PLTE");
							}

							pal_len = c.length / 3;
							if (pal_len * 3 != c.length)
							{
								return err("invalid PLTE");
							}

							for (i = 0; (i) < (pal_len); ++i)
							{
								palette[i * 4 + 0] = get8(s);
								palette[i * 4 + 1] = get8(s);
								palette[i * 4 + 2] = get8(s);
								palette[i * 4 + 3] = 255;
							}

							break;
						}
					case ((('t') << 24) + (('R') << 16) + (('N') << 8) + ('S')):
						{
							if ((first) != 0)
							{
								return err("first not IHDR");
							}

							if ((z.idata) != null)
							{
								return err("tRNS after IDAT");
							}

							if ((pal_img_n) != 0)
							{
								if ((scan) == (SCAN_header))
								{
									s.img_n = 4;
									return 1;
								}

								if ((pal_len) == (0))
								{
									return err("tRNS before PLTE");
								}

								if ((c.length) > (pal_len))
								{
									return err("bad tRNS len");
								}

								pal_img_n = 4;
								for (i = 0; (i) < (c.length); ++i)
								{
									palette[i * 4 + 3] = get8(s);
								}
							}
							else
							{
								if ((s.img_n & 1) == 0)
								{
									return err("tRNS with alpha");
								}

								if (c.length != (uint)(s.img_n) * 2)
								{
									return err("bad tRNS len");
								}

								has_trans = 1;
								if ((z.depth) == (16))
								{
									for (k = 0; (k) < (s.img_n); ++k)
									{
										tc16[k] = ((ushort)(get16be(s)));
									}
								}
								else
								{
									for (k = 0; (k) < (s.img_n); ++k)
									{
										tc[k] = (byte)((byte)(get16be(s) & 255) * depth_scale_table[z.depth]);
									}
								}
							}

							break;
						}
					case ((('I') << 24) + (('D') << 16) + (('A') << 8) + ('T')):
						{
							if ((first) != 0)
							{
								return err("first not IHDR");
							}

							if (((pal_img_n) != 0) && (pal_len == 0))
							{
								return err("no PLTE");
							}

							if ((scan) == (SCAN_header))
							{
								s.img_n = pal_img_n;
								return 1;
							}

							if (((int)(ioff + c.length)) < ((int)(ioff)))
							{
								return 0;
							}

							if ((ioff + c.length) > (idata_limit))
							{
								uint idata_limit_old = idata_limit;
								byte* p;
								if ((idata_limit) == (0))
								{
									idata_limit = (c.length) > (4096) ? c.length : 4096;
								}

								while ((ioff + c.length) > (idata_limit))
								{
									idata_limit *= 2;
								}

								p = (byte*)(realloc(z.idata, (ulong)(idata_limit)));
								if ((p) == (null))
								{
									return err("outofmem");
								}

								z.idata = p;
							}

							if (getn(s, z.idata + ioff, (int)(c.length)) == 0)
							{
								return err("outofdata");
							}

							ioff += c.length;
							break;
						}
					case ((('I') << 24) + (('E') << 16) + (('N') << 8) + ('D')):
						{
							uint raw_len;
							uint bpl;
							if ((first) != 0)
							{
								return err("first not IHDR");
							}

							if (scan != SCAN_load)
							{
								return 1;
							}

							if ((z.idata) == (null))
							{
								return err("no IDAT");
							}

							bpl = (uint)((s.img_x * z.depth + 7) / 8);
							raw_len = (uint)(bpl * s.img_y * s.img_n + s.img_y);
							z.expanded =
								(byte*)
								(zlib_decode_malloc_guesssize_headerflag((sbyte*)(z.idata), (int)(ioff),
									(int)(raw_len),
									(int*)(&raw_len), is_iphone != 0 ? 0 : 1));
							if ((z.expanded) == (null))
							{
								return 0;
							}

							free(z.idata);
							z.idata = (null);
							if (((((req_comp) == (s.img_n + 1)) && (req_comp != 3)) && (pal_img_n == 0)) ||
								((has_trans) != 0))
							{
								s.img_out_n = s.img_n + 1;
							}
							else
							{
								s.img_out_n = s.img_n;
							}

							if (
							create_png_image(z, z.expanded, raw_len, s.img_out_n,
								z.depth, color,
								interlace) == 0)
							{
								return 0;
							}

							if ((has_trans) != 0)
							{
								if ((z.depth) == (16))
								{
									if (compute_transparency16(z, tc16, s.img_out_n) == 0)
									{
										return 0;
									}
								}
								else
								{
									if (compute_transparency(z, tc, s.img_out_n) == 0)
									{
										return 0;
									}
								}
							}

							if ((((is_iphone) != 0) && ((de_iphone_flag) != 0)) && ((s.img_out_n) > (2)))
							{
								de_iphone(z);
							}

							if ((pal_img_n) != 0)
							{
								s.img_n = pal_img_n;
								s.img_out_n = pal_img_n;
								if ((req_comp) >= (3))
								{
									s.img_out_n = req_comp;
								}

								if (expand_png_palette(z, palette, (int)(pal_len), s.img_out_n) == 0)
								{
									return 0;
								}
							}
							else if ((has_trans) != 0)
							{
								++s.img_n;
							}

							free(z.expanded);
							z.expanded = (null);
							return 1;
						}
					default:
						if ((first) != 0)
						{
							return err("first not IHDR");
						}

						if ((c.type & (1 << 29)) == (0))
						{
							string invalid_chunk = "XXXX PNG chunk not known";
							return err(invalid_chunk);
						}

						skip(s, (int)(c.length));
						break;
				}

				get32be(s);
			}
		}

		public static void* do_png(png p, int* x, int* y, int* n, int req_comp, result_info* ri)
		{
			void* result = (null);
			if (((req_comp) < (0)) || ((req_comp) > (4)))
			{
				return (err("bad req_comp")) != 0 ? ((byte*)null) : (null);
			}

			if ((parse_png_file(p, SCAN_load, req_comp)) != 0)
			{
				if ((p.depth) < (8))
				{
					ri->bits_per_channel = 8;
				}
				else
				{
					ri->bits_per_channel = p.depth;
				}

				result = p._out_;
				p._out_ = (null);
				if (((req_comp) != 0) && (req_comp != p.s.img_out_n))
				{
					if ((ri->bits_per_channel) == (8))
					{
						result = convert_format((byte*)(result), p.s.img_out_n, req_comp,
							p.s.img_x,
							p.s.img_y);
					}
					else
					{
						result = convert_format16((ushort*)(result), p.s.img_out_n, req_comp,
							p.s.img_x,
							p.s.img_y);
					}

					p.s.img_out_n = req_comp;
					if ((result) == (null))
					{
						return result;
					}
				}

				*x = (int)(p.s.img_x);
				*y = (int)(p.s.img_y);
				if ((n) != null)
				{
					*n = p.s.img_n;
				}
			}

			free(p._out_);
			p._out_ = (null);
			free(p.expanded);
			p.expanded = (null);
			free(p.idata);
			p.idata = (null);
			return result;
		}

		public static void* png_load(Context s, int* x, int* y, int* comp, int req_comp,
			result_info* ri)
		{
			png p = new png
			{
				s = s
			};
			return do_png(p, x, y, comp, req_comp, ri);
		}

		public static int png_test(Context s)
		{
			int r;
			r = check_png_header(s);
			rewind(s);
			return r;
		}

		public static int png_info_raw(png p, int* x, int* y, int* comp)
		{
			if (parse_png_file(p, SCAN_header, 0) == 0)
			{
				rewind(p.s);
				return 0;
			}

			if ((x) != null)
			{
				*x = (int)(p.s.img_x);
			}

			if ((y) != null)
			{
				*y = (int)(p.s.img_y);
			}

			if ((comp) != null)
			{
				*comp = p.s.img_n;
			}

			return 1;
		}

		public static int png_info(Context s, int* x, int* y, int* comp)
		{
			png p = new png
			{
				s = s
			};
			return png_info_raw(p, x, y, comp);
		}

		public static int bmp_test_raw(Context s)
		{
			int r;
			int sz;
			if (get8(s) != 'B')
			{
				return 0;
			}

			if (get8(s) != 'M')
			{
				return 0;
			}

			get32le(s);
			get16le(s);
			get16le(s);
			get32le(s);
			sz = (int)(get32le(s));
			r = (((((sz) == (12)) || ((sz) == (40))) || ((sz) == (56))) || ((sz) == (108))) || ((sz) == (124))
				? 1
				: 0;
			return r;
		}

		public static int bmp_test(Context s)
		{
			int r = bmp_test_raw(s);
			rewind(s);
			return r;
		}

		public static int high_bit(uint z)
		{
			int n = 0;
			if ((z) == (0))
			{
				return -1;
			}

			if ((z) >= (0x10000))
			{
				n += 16;
				z >>= 16;
			}

			if ((z) >= (0x00100))
			{
				n += 8;
				z >>= 8;
			}

			if ((z) >= (0x00010))
			{
				n += 4;
				z >>= 4;
			}

			if ((z) >= (0x00004))
			{
				n += 2;
				z >>= 2;
			}

			if ((z) >= (0x00002))
			{
				n += 1;
				z >>= 1;
			}

			return n;
		}

		public static int bitcount(uint a)
		{
			a = (a & 0x55555555) + ((a >> 1) & 0x55555555);
			a = (a & 0x33333333) + ((a >> 2) & 0x33333333);
			a = (a + (a >> 4)) & 0x0f0f0f0f;
			a = a + (a >> 8);
			a = a + (a >> 16);
			return (int)(a & 0xff);
		}

		public static int shiftsigned(int v, int shift, int bits)
		{
			int result;
			int z = 0;
			if ((shift) < (0))
			{
				v <<= -shift;
			}
			else
			{
				v >>= shift;
			}

			result = v;
			z = bits;
			while ((z) < (8))
			{
				result += v >> z;
				z += bits;
			}

			return result;
		}

		public static void* bmp_parse_header(Context s, bmp_data* info)
		{
			int hsz;
			if ((get8(s) != 'B') || (get8(s) != 'M'))
			{
				return (err("not BMP")) != 0 ? ((byte*)null) : (null);
			}

			get32le(s);
			get16le(s);
			get16le(s);
			info->offset = (int)(get32le(s));
			info->hsz = hsz = (int)(get32le(s));
			info->mr = info->mg = info->mb = info->ma = 0;
			if (((((hsz != 12) && (hsz != 40)) && (hsz != 56)) && (hsz != 108)) && (hsz != 124))
			{
				return (err("unknown BMP")) != 0 ? ((byte*)null) : (null);
			}

			if ((hsz) == (12))
			{
				s.img_x = (uint)(get16le(s));
				s.img_y = (uint)(get16le(s));
			}
			else
			{
				s.img_x = get32le(s);
				s.img_y = get32le(s);
			}

			if (get16le(s) != 1)
			{
				return (err("bad BMP")) != 0 ? ((byte*)null) : (null);
			}

			info->bpp = get16le(s);
			if ((info->bpp) == (1))
			{
				return (err("monochrome")) != 0 ? ((byte*)null) : (null);
			}

			if (hsz != 12)
			{
				int compress = (int)(get32le(s));
				if (((compress) == (1)) || ((compress) == (2)))
				{
					return (err("BMP RLE")) != 0 ? ((byte*)null) : (null);
				}

				get32le(s);
				get32le(s);
				get32le(s);
				get32le(s);
				get32le(s);
				if (((hsz) == (40)) || ((hsz) == (56)))
				{
					if ((hsz) == (56))
					{
						get32le(s);
						get32le(s);
						get32le(s);
						get32le(s);
					}

					if (((info->bpp) == (16)) || ((info->bpp) == (32)))
					{
						if ((compress) == (0))
						{
							if ((info->bpp) == (32))
							{
								info->mr = 0xffu << 16;
								info->mg = 0xffu << 8;
								info->mb = 0xffu << 0;
								info->ma = 0xffu << 24;
								info->all_a = 0;
							}
							else
							{
								info->mr = 31u << 10;
								info->mg = 31u << 5;
								info->mb = 31u << 0;
							}
						}
						else if ((compress) == (3))
						{
							info->mr = get32le(s);
							info->mg = get32le(s);
							info->mb = get32le(s);
							if (((info->mr) == (info->mg)) && ((info->mg) == (info->mb)))
							{
								return ((err("bad BMP")) != 0 ? ((byte*)null) : (null));
							}
						}
						else
						{
							return ((err("bad BMP")) != 0 ? ((byte*)null) : (null));
						}
					}
				}
				else
				{
					int i;
					if ((hsz != 108) && (hsz != 124))
					{
						return (err("bad BMP")) != 0 ? ((byte*)null) : (null);
					}

					info->mr = get32le(s);
					info->mg = get32le(s);
					info->mb = get32le(s);
					info->ma = get32le(s);
					get32le(s);
					for (i = 0; (i) < (12); ++i)
					{
						get32le(s);
					}

					if ((hsz) == (124))
					{
						get32le(s);
						get32le(s);
						get32le(s);
						get32le(s);
					}
				}
			}

			return (void*)(1);
		}

		public static void* bmp_load(Context s, int* x, int* y, int* comp, int req_comp, result_info* ri)
		{
			byte* _out_;
			uint mr = 0;
			uint mg = 0;
			uint mb = 0;
			uint ma = 0;
			uint all_a;
			byte* pal = stackalloc byte[256 * 4];
			int psize = 0;
			int i;
			int j;
			int width;
			int flip_vertically;
			int pad;
			int target;
			bmp_data info = new bmp_data
			{
				all_a = 255
			};
			if ((bmp_parse_header(s, &info)) == (null))
			{
				return (null);
			}

			flip_vertically = ((int)(s.img_y)) > (0) ? 1 : 0;
			s.img_y = (uint)(Math.Abs(s.img_y));
			mr = info.mr;
			mg = info.mg;
			mb = info.mb;
			ma = info.ma;
			all_a = info.all_a;
			if ((info.hsz) == (12))
			{
				if ((info.bpp) < (24))
				{
					psize = (info.offset - 14 - 24) / 3;
				}
			}
			else
			{
				if ((info.bpp) < (16))
				{
					psize = (info.offset - 14 - info.hsz) >> 2;
				}
			}

			s.img_n = (ma) != 0 ? 4 : 3;
			if (((req_comp) != 0) && ((req_comp) >= (3)))
			{
				target = req_comp;
			}
			else
			{
				target = s.img_n;
			}

			if (mad3sizes_valid(target, (int)(s.img_x), (int)(s.img_y), 0) == 0)
			{
				return err("too large") != 0 ? (byte*)null : null;
			}

			_out_ = (byte*)(malloc_mad3(target, (int)(s.img_x), (int)(s.img_y), 0));
			if (_out_ == null)
			{
				return ((err("outofmem")) != 0 ? ((byte*)null) : (null));
			}

			if ((info.bpp) < (16))
			{
				int z = 0;
				if (((psize) == (0)) || ((psize) > (256)))
				{
					free(_out_);
					return (err("invalid")) != 0 ? ((byte*)null) : (null);
				}

				for (i = 0; (i) < (psize); ++i)
				{
					pal[i * 4 + 2] = get8(s);
					pal[i * 4 + 1] = get8(s);
					pal[i * 4 + 0] = get8(s);
					if (info.hsz != 12)
					{
						get8(s);
					}

					pal[i * 4 + 3] = 255;
				}

				skip(s, info.offset - 14 - info.hsz - psize * ((info.hsz) == (12) ? 3 : 4));
				if ((info.bpp) == (4))
				{
					width = (int)((s.img_x + 1) >> 1);
				}
				else if ((info.bpp) == (8))
				{
					width = (int)(s.img_x);
				}
				else
				{
					free(_out_);
					return (err("bad bpp")) != 0 ? ((byte*)null) : (null);
				}

				pad = (-width) & 3;
				for (j = 0; (j) < ((int)(s.img_y)); ++j)
				{
					for (i = 0; (i) < ((int)(s.img_x)); i += 2)
					{
						int v = get8(s);
						int v2 = 0;
						if ((info.bpp) == (4))
						{
							v2 = v & 15;
							v >>= 4;
						}

						_out_[z++] = pal[v * 4 + 0];
						_out_[z++] = pal[v * 4 + 1];
						_out_[z++] = pal[v * 4 + 2];
						if ((target) == (4))
						{
							_out_[z++] = 255;
						}

						if ((i + 1) == ((int)(s.img_x)))
						{
							break;
						}

						v = ((info.bpp) == (8)) ? get8(s) : v2;
						_out_[z++] = pal[v * 4 + 0];
						_out_[z++] = pal[v * 4 + 1];
						_out_[z++] = pal[v * 4 + 2];
						if ((target) == (4))
						{
							_out_[z++] = 255;
						}
					}

					skip(s, pad);
				}
			}
			else
			{
				int rshift = 0;
				int gshift = 0;
				int bshift = 0;
				int ashift = 0;
				int rcount = 0;
				int gcount = 0;
				int bcount = 0;
				int acount = 0;
				int z = 0;
				int easy = 0;
				skip(s, info.offset - 14 - info.hsz);
				if ((info.bpp) == (24))
				{
					width = (int)(3 * s.img_x);
				}
				else if ((info.bpp) == (16))
				{
					width = (int)(2 * s.img_x);
				}
				else
				{
					width = 0;
				}

				pad = (-width) & 3;
				if ((info.bpp) == (24))
				{
					easy = 1;
				}
				else if ((info.bpp) == (32))
				{
					if (((((mb) == (0xff)) && ((mg) == (0xff00))) && ((mr) == (0x00ff0000))) && ((ma) == (0xff000000)))
					{
						easy = 2;
					}
				}

				if (easy == 0)
				{
					if (((mr == 0) || (mg == 0)) || (mb == 0))
					{
						free(_out_);
						return (err("bad masks")) != 0 ? ((byte*)null) : (null);
					}

					rshift = high_bit(mr) - 7;
					rcount = bitcount(mr);
					gshift = high_bit(mg) - 7;
					gcount = bitcount(mg);
					bshift = high_bit(mb) - 7;
					bcount = bitcount(mb);
					ashift = high_bit(ma) - 7;
					acount = bitcount(ma);
				}

				for (j = 0; (j) < ((int)(s.img_y)); ++j)
				{
					if ((easy) != 0)
					{
						for (i = 0; (i) < ((int)(s.img_x)); ++i)
						{
							byte a;
							_out_[z + 2] = get8(s);
							_out_[z + 1] = get8(s);
							_out_[z + 0] = get8(s);
							z += 3;
							a = (byte)((easy) == (2) ? get8(s) : 255);
							all_a |= a;
							if ((target) == (4))
							{
								_out_[z++] = a;
							}
						}
					}
					else
					{
						int bpp = info.bpp;
						for (i = 0; (i) < ((int)(s.img_x)); ++i)
						{
							uint v = (bpp) == (16) ? (uint)(get16le(s)) : get32le(s);
							int a;
							_out_[z++] = ((byte)((shiftsigned((int)(v & mr), rshift, rcount)) &
												  255));
							_out_[z++] = ((byte)((shiftsigned((int)(v & mg), gshift, gcount)) &
												  255));
							_out_[z++] = ((byte)((shiftsigned((int)(v & mb), bshift, bcount)) &
												  255));
							a = (ma) != 0
								? shiftsigned((int)(v & ma), ashift, acount)
								: 255;
							all_a |= (uint)(a);
							if ((target) == (4))
							{
								_out_[z++] = ((byte)((a) & 255));
							}
						}
					}

					skip(s, pad);
				}
			}

			if (((target) == (4)) && ((all_a) == (0)))
			{
				for (i = (int)(4 * s.img_x * s.img_y - 1); (i) >= (0); i -= 4)
				{
					_out_[i] = 255;
				}
			}

			if ((flip_vertically) != 0)
			{
				byte t;
				for (j = 0; (j) < ((int)(s.img_y) >> 1); ++j)
				{
					byte* p1 = _out_ + j * s.img_x * target;
					byte* p2 = _out_ + (s.img_y - 1 - j) * s.img_x * target;
					for (i = 0; (i) < ((int)(s.img_x) * target); ++i)
					{
						t = p1[i];
						p1[i] = p2[i];
						p2[i] = t;
					}
				}
			}

			if (((req_comp) != 0) && (req_comp != target))
			{
				_out_ = convert_format(_out_, target, req_comp, s.img_x,
					s.img_y);
				if ((_out_) == (null))
				{
					return _out_;
				}
			}

			*x = (int)(s.img_x);
			*y = (int)(s.img_y);
			if ((comp) != null)
			{
				*comp = s.img_n;
			}

			return _out_;
		}

		public static int tga_get_comp(int bits_per_pixel, int is_grey, int* is_rgb16)
		{
			if ((is_rgb16) != null)
			{
				*is_rgb16 = 0;
			}

			switch (bits_per_pixel)
			{
				case 8:
					return grey;
				case 15:
				case 16:
					if (((bits_per_pixel) == (16)) && ((is_grey) != 0))
					{
						return grey_alpha;
					}

					if ((is_rgb16) != null)
					{
						*is_rgb16 = 1;
					}

					return rgb;
				case 24:
				case 32:
					return bits_per_pixel / 8;
				default:
					return 0;
			}

		}

		public static int tga_info(Context s, int* x, int* y, int* comp)
		{
			int tga_w;
			int tga_h;
			int tga_comp;
			int tga_image_type;
			int tga_bits_per_pixel;
			int tga_colormap_bpp;
			int sz;
			int tga_colormap_type;
			get8(s);
			tga_colormap_type = get8(s);
			if ((tga_colormap_type) > (1))
			{
				rewind(s);
				return 0;
			}

			tga_image_type = get8(s);
			if ((tga_colormap_type) == (1))
			{
				if ((tga_image_type != 1) && (tga_image_type != 9))
				{
					rewind(s);
					return 0;
				}

				skip(s, 4);
				sz = get8(s);
				if (((((sz != 8) && (sz != 15)) && (sz != 16)) && (sz != 24)) && (sz != 32))
				{
					rewind(s);
					return 0;
				}

				skip(s, 4);
				tga_colormap_bpp = sz;
			}
			else
			{
				if ((((tga_image_type != 2) && (tga_image_type != 3)) && (tga_image_type != 10)) &&
					(tga_image_type != 11))
				{
					rewind(s);
					return 0;
				}

				skip(s, 9);
				tga_colormap_bpp = 0;
			}

			tga_w = get16le(s);
			if ((tga_w) < (1))
			{
				rewind(s);
				return 0;
			}

			tga_h = get16le(s);
			if ((tga_h) < (1))
			{
				rewind(s);
				return 0;
			}

			tga_bits_per_pixel = get8(s);
			get8(s);
			if (tga_colormap_bpp != 0)
			{
				if ((tga_bits_per_pixel != 8) && (tga_bits_per_pixel != 16))
				{
					rewind(s);
					return 0;
				}

				tga_comp = tga_get_comp(tga_colormap_bpp, 0, (null));
			}
			else
			{
				tga_comp =

					tga_get_comp(tga_bits_per_pixel,
						(((tga_image_type) == (3))) || (((tga_image_type) == (11))) ? 1 : 0, (null));
			}

			if (tga_comp == 0)
			{
				rewind(s);
				return 0;
			}

			if ((x) != null)
			{
				*x = tga_w;
			}

			if ((y) != null)
			{
				*y = tga_h;
			}

			if ((comp) != null)
			{
				*comp = tga_comp;
			}

			return 1;
		}

		public static int tga_test(Context s)
		{
			int res = 0;
			int sz;
			int tga_color_type;
			get8(s);
			tga_color_type = get8(s);
			if ((tga_color_type) > (1))
			{
				goto errorEnd;
			}

			sz = get8(s);
			if ((tga_color_type) == (1))
			{
				if ((sz != 1) && (sz != 9))
				{
					goto errorEnd;
				}

				skip(s, 4);
				sz = get8(s);
				if (((((sz != 8) && (sz != 15)) && (sz != 16)) && (sz != 24)) && (sz != 32))
				{
					goto errorEnd;
				}

				skip(s, 4);
			}
			else
			{
				if ((((sz != 2) && (sz != 3)) && (sz != 10)) && (sz != 11))
				{
					goto errorEnd;
				}

				skip(s, 9);
			}

			if ((get16le(s)) < (1))
			{
				goto errorEnd;
			}

			if ((get16le(s)) < (1))
			{
				goto errorEnd;
			}

			sz = get8(s);
			if ((((tga_color_type) == (1)) && (sz != 8)) && (sz != 16))
			{
				goto errorEnd;
			}

			if (((((sz != 8) && (sz != 15)) && (sz != 16)) && (sz != 24)) && (sz != 32))
			{
				goto errorEnd;
			}

			res = 1;
		errorEnd:
			;
			rewind(s);
			return res;
		}

		public static void tga_read_rgb16(Context s, byte* _out_)
		{
			ushort px = (ushort)(get16le(s));
			ushort fiveBitMask = 31;
			int r = (px >> 10) & fiveBitMask;
			int g = (px >> 5) & fiveBitMask;
			int b = px & fiveBitMask;
			_out_[0] = ((byte)((r * 255) / 31));
			_out_[1] = ((byte)((g * 255) / 31));
			_out_[2] = ((byte)((b * 255) / 31));
		}

		public static void* tga_load(Context s, int* x, int* y, int* comp, int req_comp, result_info* ri)
		{
			int tga_offset = get8(s);
			int tga_indexed = get8(s);
			int tga_image_type = get8(s);
			int tga_is_RLE = 0;
			int tga_palette_start = get16le(s);
			int tga_palette_len = get16le(s);
			int tga_palette_bits = get8(s);
			int tga_x_origin = get16le(s);
			int tga_y_origin = get16le(s);
			int tga_width = get16le(s);
			int tga_height = get16le(s);
			int tga_bits_per_pixel = get8(s);
			int tga_comp;
			int tga_rgb16 = 0;
			int tga_inverted = get8(s);
			byte* tga_data;
			byte* tga_palette = (null);
			int i;
			int j;
			byte* raw_data = stackalloc byte[4];
			raw_data[0] = 0;

			int RLE_count = 0;
			int RLE_repeating = 0;
			int read_next_pixel = 1;
			if ((tga_image_type) >= (8))
			{
				tga_image_type -= 8;
				tga_is_RLE = 1;
			}

			tga_inverted = 1 - ((tga_inverted >> 5) & 1);
			if ((tga_indexed) != 0)
			{
				tga_comp = tga_get_comp(tga_palette_bits, 0, &tga_rgb16);
			}
			else
			{
				tga_comp = tga_get_comp(tga_bits_per_pixel, (tga_image_type) == (3) ? 1 : 0,
					&tga_rgb16);
			}

			if (tga_comp == 0)
			{
				return (err("bad format")) != 0 ? ((byte*)null) : (null);
			}

			*x = tga_width;
			*y = tga_height;
			if ((comp) != null)
			{
				*comp = tga_comp;
			}

			if (mad3sizes_valid(tga_width, tga_height, tga_comp, 0) == 0)
			{
				return (err("too large")) != 0 ? ((byte*)null) : (null);
			}

			tga_data = (byte*)(malloc_mad3(tga_width, tga_height, tga_comp, 0));
			if (tga_data == null)
			{
				return (err("outofmem")) != 0 ? ((byte*)null) : (null);
			}

			skip(s, tga_offset);
			if (((tga_indexed == 0) && (tga_is_RLE == 0)) && (tga_rgb16 == 0))
			{
				for (i = 0; (i) < (tga_height); ++i)
				{
					int row = (tga_inverted) != 0 ? tga_height - i - 1 : i;
					byte* tga_row = tga_data + row * tga_width * tga_comp;
					getn(s, tga_row, tga_width * tga_comp);
				}
			}
			else
			{
				if ((tga_indexed) != 0)
				{
					skip(s, tga_palette_start);
					tga_palette = (byte*)(malloc_mad2(tga_palette_len, tga_comp, 0));
					if (tga_palette == null)
					{
						free(tga_data);
						return (err("outofmem")) != 0 ? ((byte*)null) : (null);
					}

					if ((tga_rgb16) != 0)
					{
						byte* pal_entry = tga_palette;
						for (i = 0; (i) < (tga_palette_len); ++i)
						{
							tga_read_rgb16(s, pal_entry);
							pal_entry += tga_comp;
						}
					}
					else if (getn(s, tga_palette, tga_palette_len * tga_comp) == 0)
					{
						free(tga_data);
						free(tga_palette);
						return (err("bad palette")) != 0 ? ((byte*)null) : (null);
					}
				}

				for (i = 0; (i) < (tga_width * tga_height); ++i)
				{
					if ((tga_is_RLE) != 0)
					{
						if ((RLE_count) == (0))
						{
							int RLE_cmd = get8(s);
							RLE_count = 1 + (RLE_cmd & 127);
							RLE_repeating = RLE_cmd >> 7;
							read_next_pixel = 1;
						}
						else if (RLE_repeating == 0)
						{
							read_next_pixel = 1;
						}
					}
					else
					{
						read_next_pixel = 1;
					}

					if ((read_next_pixel) != 0)
					{
						if ((tga_indexed) != 0)
						{
							int pal_idx = ((tga_bits_per_pixel) == (8)) ? get8(s) : get16le(s);
							if ((pal_idx) >= (tga_palette_len))
							{
								pal_idx = 0;
							}

							pal_idx *= tga_comp;
							for (j = 0; (j) < (tga_comp); ++j)
							{
								raw_data[j] = tga_palette[pal_idx + j];
							}
						}
						else if ((tga_rgb16) != 0)
						{
							tga_read_rgb16(s, raw_data);
						}
						else
						{
							for (j = 0; (j) < (tga_comp); ++j)
							{
								raw_data[j] = get8(s);
							}
						}

						read_next_pixel = 0;
					}

					for (j = 0; (j) < (tga_comp); ++j)
					{
						tga_data[i * tga_comp + j] = raw_data[j];
					}

					--RLE_count;
				}

				if ((tga_inverted) != 0)
				{
					for (j = 0; (j * 2) < (tga_height); ++j)
					{
						int index1 = j * tga_width * tga_comp;
						int index2 = (tga_height - 1 - j) * tga_width * tga_comp;
						for (i = tga_width * tga_comp; (i) > (0); --i)
						{
							byte temp = tga_data[index1];
							tga_data[index1] = tga_data[index2];
							tga_data[index2] = temp;
							++index1;
							++index2;
						}
					}
				}

				if (tga_palette != (null))
				{
					free(tga_palette);
				}
			}

			if (((tga_comp) >= (3)) && (tga_rgb16 == 0))
			{
				byte* tga_pixel = tga_data;
				for (i = 0; (i) < (tga_width * tga_height); ++i)
				{
					byte temp = tga_pixel[0];
					tga_pixel[0] = tga_pixel[2];
					tga_pixel[2] = temp;
					tga_pixel += tga_comp;
				}
			}

			if (((req_comp) != 0) && (req_comp != tga_comp))
			{
				tga_data = convert_format(tga_data, tga_comp, req_comp, (uint)(tga_width),
					(uint)(tga_height));
			}

			tga_palette_start =
				tga_palette_len =
					tga_palette_bits = tga_x_origin = tga_y_origin = 0;
			return tga_data;
		}







		public static int bmp_info(Context s, int* x, int* y, int* comp)
		{
			void* p;
			bmp_data info = new bmp_data
			{
				all_a = 255
			};
			p = bmp_parse_header(s, &info);
			rewind(s);
			if ((p) == (null))
			{
				return 0;
			}

			if ((x) != null)
			{
				*x = (int)(s.img_x);
			}

			if ((y) != null)
			{
				*y = (int)(s.img_y);
			}

			if ((comp) != null)
			{
				*comp = (info.ma) != 0 ? 4 : 3;
			}

			return 1;
		}


		public static int info_main(Context s, int* x, int* y, int* comp)
		{
			if ((jpeg_info(s, x, y, comp)) != 0)
			{
				return 1;
			}

			if ((png_info(s, x, y, comp)) != 0)
			{
				return 1;
			}

			if ((bmp_info(s, x, y, comp)) != 0)
			{
				return 1;
			}


			if ((tga_info(s, x, y, comp)) != 0)
			{
				return 1;
			}

			return err("unknown image type");
		}

		public static int info_from_memory(byte* buffer, int len, int* x, int* y, int* comp)
		{
			Context s = new Context();
			start_mem(s, buffer, len);
			return info_main(s, x, y, comp);
		}

		public static int info_from_callbacks(io_callbacks c, void* user, int* x, int* y, int* comp)
		{
			Context s = new Context();
			start_callbacks(s, c, user);
			return info_main(s, x, y, comp);
		}
	}
}