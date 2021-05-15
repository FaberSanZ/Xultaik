// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	StreamExtensions.cs
=============================================================================*/

using System.IO;

namespace Vultaik.GLTF
{
    internal static class StreamExtensions
    {
        public static void Align(this Stream stream, int size, byte fillByte = 0)
        {
            long num = stream.Position % size;

            if (num == 0L)
                return;

            for (int index = 0; index < size - num; ++index)
                stream.WriteByte(fillByte);
        }
    }
}
