// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace Vultaik.GLTF
{
    using Vultaik.GLTF.Schema;


    public static class Interface
    {
        private const uint GLTFHEADER = 0x46546C67;
        private const uint GLTFVERSION2 = 2;
        private const uint CHUNKJSON = 0x4E4F534A;
        private const uint CHUNKBIN = 0x004E4942;
        private const string EMBEDDEDOCTETSTREAM = "data:application/octet-stream;base64,";
        private const string EMBEDDEDGLTFBUFFER = "data:application/gltf-buffer;base64,";
        private const string EMBEDDEDPNG = "data:image/png;base64,";
        private const string EMBEDDEDJPEG = "data:image/jpeg;base64,";

        /// <summary>
        /// Loads a <code>Schema.Gltf</code> model from a file
        /// </summary>
        /// <param name="filePath">Source file path to a gltf/glb model</param>
        /// <returns><code>Schema.Gltf</code> model</returns>
        public static ValueTask<Gltf> LoadModelAsync(string filePath)
        {
            string path = Path.GetFullPath(filePath);

            using Stream stream = File.OpenRead(path);
            return LoadModelAsync(stream);
        }

        /// <summary>
        /// Reads a <code>Schema.Gltf</code> model from a stream
        /// </summary>
        /// <param name="stream">Readable stream to a gltf/glb model</param>
        /// <returns><code>Schema.Gltf</code> model</returns>
        public static ValueTask<Gltf> LoadModelAsync(Stream stream)
        {
            bool binaryFile = false;

            uint magic = 0;
            magic |= (uint)stream.ReadByte();
            magic |= (uint)stream.ReadByte() << 8;
            magic |= (uint)stream.ReadByte() << 16;
            magic |= (uint)stream.ReadByte() << 24;

            if (magic is GLTFHEADER)
                binaryFile = true;

            stream.Position = 0; // restart read position

            Stream fileData = binaryFile ? new MemoryStream(ParseBinary(stream)) : stream;


            return JsonSerializer.DeserializeAsync<Gltf>(fileData);
        }

        public static Gltf LoadModel(string path)
        {
            using Stream stream = File.OpenRead(path);

            bool binaryFile = false;

            uint magic = 0;
            magic |= (uint)stream.ReadByte();
            magic |= (uint)stream.ReadByte() << 8;
            magic |= (uint)stream.ReadByte() << 16;
            magic |= (uint)stream.ReadByte() << 24;

            if (magic is GLTFHEADER)
                binaryFile = true;

            stream.Position = 0; // restart read position

            //Stream fileData = binaryFile ? new MemoryStream(ParseBinary(stream)) : stream;



            if (binaryFile)
            {
                ReadOnlySpan<byte> span = new(ParseBinary(stream));
                return JsonSerializer.Deserialize<Gltf>(span);
            }


            return JsonSerializer.Deserialize<Gltf>(File.ReadAllText(path));

        }


       




        private static byte[] ParseBinary(Stream stream)
        {
            using BinaryReader binaryReader = new(stream);

            ReadBinaryHeader(binaryReader);

            return ReadBinaryChunk(binaryReader, CHUNKJSON);
        }

        private static byte[] ReadBinaryChunk(BinaryReader binaryReader, uint format)
        {
            while (true) // keep reading until EndOfFile exception
            {
                uint chunkLength = binaryReader.ReadUInt32();

                if ((chunkLength & 3) is not 0)
                {
                    throw new InvalidDataException($"The chunk must be padded to 4 bytes: {chunkLength}");
                }

                uint chunkFormat = binaryReader.ReadUInt32();

                byte[] data = binaryReader.ReadBytes((int)chunkLength);

                if (chunkFormat == format)
                    return data;
            }
        }

        /// <summary>
        /// Loads the binary buffer chunk of a glb file
        /// </summary>
        /// <param name="filePath">Source file path to a glb model</param>
        /// <returns>Byte array of the buffer</returns>
        public static byte[] LoadBinaryBuffer(string filePath)
        {
            using Stream stream = File.OpenRead(filePath);
            return LoadBinaryBuffer(stream);
        }

        /// <summary>
        /// Reads the binary buffer chunk of a glb stream
        /// </summary>
        /// <param name="stream">Readable stream to a glb model</param>
        /// <returns>Byte array of the buffer</returns>
        public static byte[] LoadBinaryBuffer(Stream stream)
        {
            using BinaryReader binaryReader = new(stream);

            ReadBinaryHeader(binaryReader);

            return ReadBinaryChunk(binaryReader, CHUNKBIN);
        }

        private static void ReadBinaryHeader(BinaryReader binaryReader)
        {
            uint magic = binaryReader.ReadUInt32();

            if (magic is not GLTFHEADER)
                throw new InvalidDataException($"Unexpected magic number: {magic}");

            uint version = binaryReader.ReadUInt32();

            if (version is not GLTFVERSION2)
                throw new InvalidDataException($"Unknown version number: {version}");

            uint length = binaryReader.ReadUInt32();
            long fileLength = binaryReader.BaseStream.Length;

            if (length != fileLength)
                throw new InvalidDataException($"The specified length of the file ({length}) is not equal to the actual length of the file ({fileLength}).");
        }

        /// <summary>
        /// Creates an External File Solver for a given gltf file path, so we can resolve references to associated files
        /// </summary>
        /// <param name="gltfFilePath">ource file path to a gltf/glb model</param>
        /// <returns>Lambda funcion to resolve dependencies</returns>
        private static Func<string, byte[]> GetExternalFileSolver(string gltfFilePath)
        {
            return asset =>
            {
                if (string.IsNullOrEmpty(asset))
                    return LoadBinaryBuffer(gltfFilePath);

                string bufferFilePath = Path.Combine(Path.GetDirectoryName(gltfFilePath), asset);
                return File.ReadAllBytes(bufferFilePath);
            };
        }

        /// <summary>
        /// Gets a binary buffer referenced by a specific <code>Schema.Buffer</code>
        /// </summary>
        /// <param name="model">The <code>Schema.Gltf</code> model containing the <code>Schema.Buffer</code></param>
        /// <param name="bufferIndex">The index of the buffer</param>
        /// <param name="gltfFilePath">Source file path used to load the model</param>
        /// <returns>Byte array of the buffer</returns>
        public static byte[] LoadBinaryBuffer(this Gltf model, int bufferIndex, string gltfFilePath)
        {
            return LoadBinaryBuffer(model, bufferIndex, GetExternalFileSolver(gltfFilePath));
        }

        /// <summary>
        /// Opens a stream to the image referenced by a specific <code>Schema.Image</code>
        /// </summary>
        /// <param name="model">The <code>Schema.Gltf</code> model containing the <code>Schema.Buffer</code></param>
        /// <param name="imageIndex">The index of the image</param>
        /// <param name="gltfFilePath">Source file path used to load the model</param>
        /// <returns>An open stream to the image</returns>
        public static Stream OpenImageFile(this Gltf model, int imageIndex, string gltfFilePath)
        {
            return OpenImageFile(model, imageIndex, GetExternalFileSolver(gltfFilePath));
        }

        /// <summary>
        /// Gets a binary buffer referenced by a specific <code>Schema.Buffer</code>
        /// </summary>
        /// <param name="model">The <code>Schema.Gltf</code> model containing the <code>Schema.Buffer</code></param>
        /// <param name="bufferIndex">The index of the buffer</param>
        /// <param name="externalReferenceSolver">An user provided lambda function to resolve external assets</param>
        /// <returns>Byte array of the buffer</returns>
        /// <remarks>
        /// Binary buffers can be stored in three different ways:
        /// - As stand alone files.
        /// - As a binary chunk within a glb file.
        /// - Encoded to Base64 within the JSON.
        /// 
        /// The external reference solver funcion is called when the buffer is stored in an external file,
        /// or when the buffer is in the glb binary chunk, in which case, the Argument of the function will be Null.
        /// 
        /// The Lambda function must return the byte array of the requested file or buffer.
        /// </remarks>
        public static byte[] LoadBinaryBuffer(this Gltf model, int bufferIndex, Func<string, byte[]> externalReferenceSolver)
        {
            Schema.Buffer buffer = model.Buffers[bufferIndex];

            byte[] bufferData = LoadBinaryBufferUnchecked(buffer, externalReferenceSolver);

            // As per https://github.com/KhronosGroup/glTF/issues/1026
            // Due to buffer padding, buffer length can be equal or larger than expected length by only 3 bytes
            if (bufferData.Length < buffer.ByteLength || (bufferData.Length - buffer.ByteLength) > 3)
                throw new InvalidDataException($"The buffer length is {bufferData.Length}, expected {buffer.ByteLength}");

            return bufferData;
        }

        private static byte[] TryLoadBase64BinaryBufferUnchecked(Schema.Buffer buffer, string prefix)
        {
            if (buffer.Uri is null || !buffer.Uri.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return null;

            string content = buffer.Uri.Substring(prefix.Length);
            return Convert.FromBase64String(content);
        }

        private static byte[] LoadBinaryBufferUnchecked(Schema.Buffer buffer, Func<string, byte[]> externalReferenceSolver)
        {
            return TryLoadBase64BinaryBufferUnchecked(buffer, EMBEDDEDGLTFBUFFER)
                ?? TryLoadBase64BinaryBufferUnchecked(buffer, EMBEDDEDOCTETSTREAM)
                ?? externalReferenceSolver(buffer?.Uri);
        }

        /// <summary>
        /// Opens a stream to the image referenced by a specific <code>Schema.Image</code>
        /// </summary>
        /// <param name="model">The <code>Schema.Gltf</code> model containing the <code>Schema.Image</code></param>
        /// <param name="imageIndex">The index of the image</param>
        /// <param name="externalReferenceSolver">An user provided lambda function to resolve external assets</param>
        /// <returns>An open stream to the image</returns>
        /// <remarks>
        /// Images can be stored in three different ways:
        /// - As stand alone files.
        /// - As a part of binary buffer accessed via bufferView.
        /// - Encoded to Base64 within the JSON.
        /// 
        /// The external reference solver funcion is called when the image is stored in an external file,
        /// or when the image is in the glb binary chunk, in which case, the Argument of the function will be Null.
        /// 
        /// The Lambda function must return the byte array of the requested file or buffer.
        /// </remarks>
        public static Stream OpenImageFile(this Gltf model, int imageIndex, Func<string, byte[]> externalReferenceSolver)
        {
            Image image = model.Images[imageIndex];

            if (image.BufferView.HasValue)
            {
                BufferView bufferView = model.BufferViews[image.BufferView.Value];

                byte[] bufferBytes = model.LoadBinaryBuffer(bufferView.Buffer, externalReferenceSolver);

                return new MemoryStream(bufferBytes, bufferView.ByteOffset, bufferView.ByteLength);
            }

            if (image.Uri.StartsWith("data:image/"))
                return OpenEmbeddedImage(image);

            byte[] imageData = externalReferenceSolver(image.Uri);

            return new MemoryStream(imageData);
        }

        private static Stream OpenEmbeddedImage(Image image)
        {
            string content = null;

            if (image.Uri.StartsWith(EMBEDDEDPNG))
                content = image.Uri.Substring(EMBEDDEDPNG.Length);

            if (image.Uri.StartsWith(EMBEDDEDJPEG))
                content = image.Uri.Substring(EMBEDDEDJPEG.Length);

            byte[] bytes = Convert.FromBase64String(content);

            return new MemoryStream(bytes);
        }




    }
}
