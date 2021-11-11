// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Vultaik
{
    public static unsafe class Interop
    {
        public static TDelegate GetDelegateForFunctionPointer<TDelegate>(IntPtr pointer)
        {
            return Marshal.GetDelegateForFunctionPointer<TDelegate>(pointer);
        }

        public static IntPtr GetFunctionPointerForDelegate<TDelegate>(TDelegate @delegate)
        {
            return Marshal.GetFunctionPointerForDelegate(@delegate);
        }

        public static int SizeOf<T>(params T[] values)
        {
            return Unsafe.SizeOf<T>() * values.Length;
        }

        public static int SizeOf<T>()
        {
            return Unsafe.SizeOf<T>();
        }

        public static IntPtr Alloc<T>(int count = 1)
        {
            return Alloc(Unsafe.SizeOf<T>() * count);
        }

        //public static void* Alloc<T>(int count) => (void*)Alloc(Unsafe.SizeOf<T>() * count);


        public static T ToStructure<T>(byte[] bytes, int start, int count) where T : struct
        {
            byte[] temp = bytes.Skip(start).Take(count).ToArray();
            GCHandle handle = GCHandle.Alloc(temp, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return stuff;
        }

        public static IntPtr Alloc(int byteCount)
        {
            if (byteCount is 0)
            {
                return IntPtr.Zero;
            }

            return Marshal.AllocHGlobal(byteCount);
        }



        public static T* AllocToPointer<T>(T[] values) where T : unmanaged
        {
            if (values is null || values.Length is 0)
            {
                return (T*)null;
            }

            int stride = SizeOf<T>();
            int totalSize = values.Length * stride;
            void* ptr = (void*)Marshal.AllocHGlobal(totalSize);

            byte* walk = (byte*)ptr;
            for (int i = 0; i < values.Length; i++)
            {
                Unsafe.Copy(walk, ref values[i]);
                walk += stride;
            }

            return (T*)ptr;
        }


        public static T* AllocToPointer<T>(ref T value) where T : unmanaged
        {
            void* ptr = (void*)Alloc<T>();
            Unsafe.Copy(ptr, ref value);
            return (T*)ptr;
        }


        public static class MemoryHelper
        {
            public static void Read<T>(IntPtr srcPointer, ref T value)
            {
                Unsafe.Copy(ref value, srcPointer.ToPointer());
            }

            public static void Write<T>(IntPtr dstPointer, ref T value)
            {
                Unsafe.Copy(dstPointer.ToPointer(), ref value);
            }

            public static void Write<T>(void* dstPointer, ref T value)
            {
                Unsafe.Copy(dstPointer, ref value);
            }

            public static void CopyBlocks<T>(void* dstPointer, T[] values)
            {
                if (values is null || values.Length is 0)
                {
                    return;
                }

                int stride = SizeOf<T>();
                uint size = (uint)(stride * values.Length);
                void* srcPtr = AsPointer(ref values[0]);
                Unsafe.CopyBlock(dstPointer, srcPtr, size);
            }

            public static void* AsPointer<T>(ref T value)
            {
                //bool is_array = value!.GetType().IsArray;

                return Unsafe.AsPointer(ref value);
            }
            public static void Write<T>(IntPtr dstPointer, T[] values)
            {
                if (values is null || values.Length is 0)
                {
                    return;
                }

                int stride = SizeOf<T>();
                uint size = (uint)(stride * values.Length);
                void* srcPtr = Unsafe.AsPointer(ref values[0]);
                Unsafe.CopyBlock(dstPointer.ToPointer(), srcPtr, size);
            }


            public static void Read<T>(IntPtr srcPointer, T[] values)
            {
                int stride = SizeOf<T>();
                long size = stride * values.Length;
                void* dstPtr = Unsafe.AsPointer(ref values[0]);
                System.Buffer.MemoryCopy(srcPointer.ToPointer(), dstPtr, size, size);
            }

            public static void CopyMemory(void* source, void* destination, long destinationSizeInBytes, long sourceBytesToCopy)
            {
                System.Buffer.MemoryCopy(source, destination, destinationSizeInBytes, sourceBytesToCopy);
            }

            public static void CopyMemory(void* source, void* destination, int destinationSizeInBytes, int sourceBytesToCopy)
            {
                System.Buffer.MemoryCopy(source, destination, destinationSizeInBytes, sourceBytesToCopy);
            }
        }

        public static class String
        {

            public static byte* ToPointer(string value)
            {
                return (byte*)(void*)AllocToPointer(value);
            }

            public static string FromPointer(IntPtr pointer)
            {
                return FromPointer((byte*)(void*)pointer);
            }

            public static string FromPointer(byte* pointer)
            {
                if (pointer is null)
                {
                    return string.Empty;
                }

                // Read until null-terminator.
                byte* walkPtr = pointer;
                while (*walkPtr != 0)
                {
                    walkPtr++;
                }

                // Decode UTF-8 bytes to string.
                return Encoding.UTF8.GetString(pointer, (int)(walkPtr - pointer));
            }


            public static IntPtr AllocToPointer(string value)
            {
                if (value is null)
                {
                    return IntPtr.Zero;
                }

                // Get max number of bytes the string may need.
                int maxSize = GetMaxByteCount(value);

                // Allocate unmanaged memory.
                IntPtr managedPtr = Alloc(maxSize);
                byte* ptr = (byte*)managedPtr;

                // Encode to utf-8, null-terminate and write to unmanaged memory.
                int actualNumberOfBytesWritten;
                fixed (char* ch = value)
                {
                    actualNumberOfBytesWritten = Encoding.UTF8.GetBytes(ch, value.Length, ptr, maxSize);
                }

                ptr[actualNumberOfBytesWritten] = 0;

                // Return pointer to the beginning of unmanaged memory.
                return managedPtr;
            }

            public static byte** AllocToPointers(string[] values)
            {
                if (values is null || values.Length == 0)
                {
                    return null;
                }

                // Allocate unmanaged memory for string pointers.
                byte** stringHandlesPtr = (byte**)(void*)Alloc<IntPtr>(values.Length);

                for (int i = 0; i < values.Length; i++)
                {
                    // Store the pointer to the string.
                    stringHandlesPtr[i] = (byte*)AllocToPointer(values[i]);
                }

                return stringHandlesPtr;
            }

            public static int GetMaxByteCount(string value)
            {
                return value is null ? 0 : Encoding.UTF8.GetMaxByteCount(value.Length + 1);
            }
        }
    }


}
