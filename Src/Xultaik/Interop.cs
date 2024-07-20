// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Xultaik
{
    public static unsafe class Interop
    {

        public static int SizeOf<T>() => Unsafe.SizeOf<T>();


        public static int SizeOf<T>(T[] values) => Unsafe.SizeOf<T>() * values.Length;

        public static IntPtr Alloc<T>(int count = 1) => Alloc(Unsafe.SizeOf<T>() * count);


        public static IntPtr Alloc(int byteCount)
        {
            if (byteCount == 0)
                return IntPtr.Zero;

            return Marshal.AllocHGlobal(byteCount);
        }

        public static T ToStructure<T>(byte[] bytes, int start, int count) where T : struct
        {
            byte[] temp = bytes.Skip(start).Take(count).ToArray();
            GCHandle handle = GCHandle.Alloc(temp, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return stuff;
        }

        public static T AllocToPointer<T>(T[] values) where T : unmanaged
        {
            //if (values == null || values.Length == 0)
            //    return (;
            

            int structSize = SizeOf<T>();
            int totalSize = values.Length * structSize;
            IntPtr ptr = Marshal.AllocHGlobal(totalSize);

            byte* walk = (byte*)ptr;
            for (int i = 0; i < values.Length; i++)
            {
                Unsafe.Copy(walk, ref values[i]);
                walk += structSize;
            }

            return (T)Marshal.PtrToStructure(ptr, typeof(T)); ;
        }



        public static class MemoryHelper
        {
            public static void Read<T>(IntPtr srcPointer, ref T value) => Unsafe.Copy(ref value, srcPointer.ToPointer());


            public static void Write<T>(IntPtr dstPointer, ref T value) => Unsafe.Copy(dstPointer.ToPointer(), ref value);


            public static void Write<T>(IntPtr dstPointer, T[] values)
            {
                if (values == null || values.Length == 0)
                    return;

                int stride = SizeOf<T>();
                uint size = (uint)(stride * values.Length);
                void* srcPtr = Unsafe.AsPointer(ref values[0]);
                Unsafe.CopyBlock(dstPointer.ToPointer(), srcPtr, size);
            }


            public static void Read<T>(IntPtr srcPointer, Span<T> values)
            {
                int stride = SizeOf<T>();
                long size = stride * values.Length;
                void* dstPtr = Unsafe.AsPointer(ref values[0]);
                System.Buffer.MemoryCopy(srcPointer.ToPointer(), dstPtr, size, size);
            }

            public static void CopyMemory(object uploadMemory, IntPtr dataPointer, object sizeInBytes)
            {
                throw new NotImplementedException();
            }
        }

        public static class String
        {

            public static byte* ToPointer(string value) => (byte*)(void*)AllocToPointer(value);


            public static string FromPointer(IntPtr pointer) => FromPointer((byte*)(void*)pointer);


            public static string FromPointer(byte* pointer)
            {
                if (pointer == null)
                    return string.Empty;

                // Read until null-terminator.
                byte* walkPtr = pointer;
                while (*walkPtr != 0)
                    walkPtr++;

                // Decode UTF-8 bytes to string.
                return Encoding.UTF8.GetString(pointer, (int)(walkPtr - pointer));
            }


            public static IntPtr AllocToPointer(string value)
            {
                if (value == null)
                    return IntPtr.Zero;

                // Get max number of bytes the string may need.
                int maxSize = GetMaxByteCount(value);

                // Allocate unmanaged memory.
                IntPtr managedPtr = Alloc(maxSize);
                byte* ptr = (byte*)managedPtr;

                // Encode to utf-8, null-terminate and write to unmanaged memory.
                int actualNumberOfBytesWritten;
                fixed (char* ch = value)
                    actualNumberOfBytesWritten = Encoding.UTF8.GetBytes(ch, value.Length, ptr, maxSize);
                ptr[actualNumberOfBytesWritten] = 0; 

                // Return pointer to the beginning of unmanaged memory.
                return managedPtr;
            }

            public static byte** AllocToPointers(string[] values)
            {
                if (values == null || values.Length == 0)
                    return null;

                // Allocate unmanaged memory for string pointers.
                IntPtr* stringHandlesPtr = (IntPtr*)Alloc<IntPtr>(values.Length);

                for (var i = 0; i < values.Length; i++)
                    // Store the pointer to the string.
                    stringHandlesPtr[i] = AllocToPointer(values[i]);

                return (byte**)stringHandlesPtr;
            }

            public static int GetMaxByteCount(string value) => value == null ? 0 : Encoding.UTF8.GetMaxByteCount(value.Length + 1);
        }
    }


}
