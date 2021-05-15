// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Vultaik
{
    public class KTXDecoder
    {
        internal KtxHeader Header { get; private set; }
        internal KtxKeyValuePair[] KeyValuePairs { get; private set; }
        internal KtxFace[] Faces { get; private set; }

        internal readonly byte[] KtxIdentifier =
        {
            0xAB, 0x4B, 0x54, 0x58, 0x20, 0x31, 0x31, 0xBB, 0x0D, 0x0A, 0x1A, 0x0A
        };


        public KTXDecoder(string fileName, bool readKeyValuePairs = false)
        {
            using FileStream fs = File.OpenRead(fileName);

            using BinaryReader br = new BinaryReader(fs);


            byte[] identifier = br.ReadBytes(12);

            if (!identifier.SequenceEqual(KtxIdentifier))
                throw new InvalidOperationException("File is not in Khronos Texture format.");


            KtxHeader header = new KtxHeader
            {
                Identifier = KtxIdentifier,
                Endianness = br.ReadUInt32(),
                GlType = br.ReadUInt32(),
                GlTypeSize = br.ReadUInt32(),
                GlFormat = br.ReadUInt32(),
                GlInternalFormat = br.ReadUInt32(),
                GlBaseInternalFormat = br.ReadUInt32(),
                PixelWidth = Math.Max(1, br.ReadUInt32()),
                PixelHeight = Math.Max(1, br.ReadUInt32()),
                PixelDepth = Math.Max(1, br.ReadUInt32()),
                NumberOfArrayElements = br.ReadUInt32(),                //only for array text, else 0
                NumberOfFaces = br.ReadUInt32(),                        //only for cube map, else 1
                NumberOfMipmapLevels = Math.Max(1, br.ReadUInt32()),
                BytesOfKeyValueData = br.ReadUInt32(),

            }; 

            KtxKeyValuePair[] kvps = default;

            if (readKeyValuePairs)
            {
                int keyValuePairBytesRead = 0;
                List<KtxKeyValuePair> keyValuePairs = new List<KtxKeyValuePair>();
                while (keyValuePairBytesRead < header.BytesOfKeyValueData)
                {
                    int bytesRemaining = (int)(header.BytesOfKeyValueData - keyValuePairBytesRead);
                    KtxKeyValuePair kvp = ReadNextKeyValuePair(br, out int read);
                    keyValuePairBytesRead += read;
                    keyValuePairs.Add(kvp);
                }

                kvps = keyValuePairs.ToArray();
            }
            else
            {
                br.BaseStream.Seek(header.BytesOfKeyValueData, SeekOrigin.Current); // Skip over header data.
            }

            uint numberOfFaces = Math.Max(1, header.NumberOfFaces);

            List<KtxFace> faces = new List<KtxFace>((int)numberOfFaces);

            for (int i = 0; i < numberOfFaces; i++)
            {
                faces.Add(new KtxFace(header.NumberOfMipmapLevels));
            }

            for (uint mipLevel = 0; mipLevel < header.NumberOfMipmapLevels; mipLevel++)
            {
                uint imageSize = br.ReadUInt32();
                // For cubemap textures, imageSize is actually the size of an individual face.
                bool isCubemap = header.NumberOfFaces == 6 && header.NumberOfArrayElements == 0;
                for (uint face = 0; face < numberOfFaces; face++)
                {
                    byte[] faceData = br.ReadBytes((int)imageSize);
                    faces[(int)face].Mipmaps[mipLevel] = new KtxMipmap(imageSize, faceData, header.PixelWidth / (uint)(Math.Pow(2, mipLevel)), header.PixelHeight / (uint)(Math.Pow(2, mipLevel)));
                    uint cubePadding = 0u;
                    if (isCubemap)
                    {
                        cubePadding = 3 - ((imageSize + 3) % 4);
                    }
                    br.BaseStream.Seek(cubePadding, SeekOrigin.Current);
                }

                uint mipPaddingBytes = 3 - ((imageSize + 3) % 4);
                br.BaseStream.Seek(mipPaddingBytes, SeekOrigin.Current);
            }

            Header = header;
            KeyValuePairs = kvps;
            Faces = faces.ToArray();




            ImageDescription data = new()
            {
                Width = (int)Header.PixelWidth,
                Height = (int)Header.PixelHeight,
                Depth = (int)Header.PixelDepth,
                Format = FormatExtensions.GetFormatFromOpenGLFormat(Header.GlInternalFormat),
                Size = (int)GetTotalSize(),
                IsCubeMap = Header.NumberOfFaces is 6,
                MipLevels = (int)Header.NumberOfMipmapLevels,
                Data = GetAllTextureData(),
            };


            ImageDescription = data;
        }

        public ImageDescription ImageDescription { get; private set; }

        public int Width => (int)Header.PixelWidth;

        public int Height => (int)Header.PixelHeight;

        public int Depth => (int)Header.PixelDepth;

        public int ArraySlices => (int)Header.NumberOfArrayElements;

        public int MipMaps => (int)Header.NumberOfMipmapLevels;

        public int Size => (int)GetTotalSize();

        public byte[] Data => GetAllTextureData();

        public bool Is1D => (Header.PixelHeight <= 1) && (Header.PixelDepth <= 1);

        public bool Is2D => Header.PixelHeight > 1 && Header.PixelDepth <= 1;

        public bool Is3D => Header.PixelHeight > 1 && Header.PixelDepth > 1;

        public bool IsCubeMap => Header.NumberOfFaces is 6;

        public bool IsArray => Header.NumberOfArrayElements > 1;

        public bool Dimensions => Header.NumberOfArrayElements > 1;





        public static ImageDescription LoadFromFile(string filename)
        {
            return new KTXDecoder(filename).ImageDescription;
        }

        private static unsafe KtxKeyValuePair ReadNextKeyValuePair(BinaryReader br, out int bytesRead)
        {
            uint keyAndValueByteSize = br.ReadUInt32();
            byte* keyAndValueBytes = stackalloc byte[(int)keyAndValueByteSize];
            ReadBytes(br, keyAndValueBytes, (int)keyAndValueByteSize);
            int paddingByteCount = (int)(3 - ((keyAndValueByteSize + 3) % 4));
            br.BaseStream.Seek(paddingByteCount, SeekOrigin.Current); // Skip padding bytes

            // Find the key's null terminator
            int i;
            for (i = 0; i < keyAndValueByteSize; i++)
            {
                if (keyAndValueBytes[i] == 0)
                {
                    break;
                }
                Debug.Assert(i != keyAndValueByteSize); // Fail
            }


            int keySize = i;                                                    // Don't include null terminator.
            string key = Encoding.UTF8.GetString(keyAndValueBytes, keySize);
            byte* valueStart = keyAndValueBytes + i + 1;                        // Skip null terminator
            int valueSize = (int)(keyAndValueByteSize - keySize - 1);
            byte[] value = new byte[valueSize];

            for (int v = 0; v < valueSize; v++)
            {
                value[v] = valueStart[v];
            }

            bytesRead = (int)(keyAndValueByteSize + paddingByteCount + sizeof(uint));
            return new KtxKeyValuePair(key, value);
        }

        private static unsafe void ReadBytes(BinaryReader br, byte* destination, int count)
        {
            for (int i = 0; i < count; i++)
            {
                destination[i] = br.ReadByte();
            }
        }

        private ulong GetTotalSize()
        {
            ulong totalSize = 0;

            for (int mipLevel = 0; mipLevel < Header.NumberOfMipmapLevels; mipLevel++)
            {
                for (int face = 0; face < Header.NumberOfFaces; face++)
                {
                    KtxMipmap mipmap = Faces[face].Mipmaps[mipLevel];
                    totalSize += mipmap.SizeInBytes;
                }
            }

            return totalSize;
        }

        private byte[] GetAllTextureData()
        {
            byte[] result = new byte[GetTotalSize()];
            uint start = 0;
            for (int face = 0; face < Header.NumberOfFaces; face++)
            {
                for (int mipLevel = 0; mipLevel < Header.NumberOfMipmapLevels; mipLevel++)
                {
                    KtxMipmap mipmap = Faces[face].Mipmaps[mipLevel];
                    mipmap.Data.CopyTo(result, (int)start);
                    start += mipmap.SizeInBytes;
                }
            }

            return result;
        }

        internal class KtxKeyValuePair
        {
            public string Key { get; }
            public byte[] Value { get; }
            public KtxKeyValuePair(string key, byte[] value)
            {
                Key = key;
                Value = value;
            }
        }

        internal class KtxHeader
        {
            public byte[] Identifier; //=> new byte[] { 0xAB, 0x4B, 0x54, 0x58, 0x20, 0x31, 0x31, 0xBB, 0x0D, 0x0A, 0x1A, 0x0A };
            public uint Endianness;
            public uint GlType;
            public uint GlTypeSize;
            public uint GlFormat;
            public uint GlInternalFormat;
            public uint GlBaseInternalFormat;
            public uint PixelWidth;
            private readonly uint _pixelHeight;
            public uint PixelHeight;
            public uint PixelDepth;
            public uint NumberOfArrayElements;
            public uint NumberOfFaces;
            public uint NumberOfMipmapLevels;
            public uint BytesOfKeyValueData;
        }

        internal class KtxFace
        {
            public uint Width { get; set; }
            public uint Height { get; set; }
            public uint NumberOfMipmapLevels { get; }
            public KtxMipmap[] Mipmaps { get; }

            public KtxFace(uint width, uint height, uint numberOfMipmapLevels, KtxMipmap[] mipmaps)
            {
                Width = width;
                Height = height;
                NumberOfMipmapLevels = numberOfMipmapLevels;
                Mipmaps = mipmaps;
            }

            public KtxFace(uint numberOfMipmapLevels)
            {
                NumberOfMipmapLevels = numberOfMipmapLevels;
                Mipmaps = new KtxMipmap[numberOfMipmapLevels];
            }
        }

        internal class KtxMipmap
        {
            public uint SizeInBytes { get; }
            public byte[] Data { get; }
            public uint Width { get; }
            public uint Height { get; }
            public KtxMipmap(uint sizeInBytes, byte[] data, uint width, uint height)
            {
                SizeInBytes = sizeInBytes;
                Data = data;
                Width = Math.Max(1, width);
                Height = Math.Max(1, height);
            }
        }
    }
}
