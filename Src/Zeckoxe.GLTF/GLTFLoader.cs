// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GLTFLoader.cs
=============================================================================*/


using glTFLoader;
using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Zeckoxe.Graphics;


namespace Zeckoxe.GLTF
{
    public class BufferInfo : IDisposable
    {
        public byte[] bufferBytes;
        public IntPtr bufferPointer;
        public GCHandle bufferHandle;

        public BufferInfo(byte[] bufferBytes)
        {
            this.bufferBytes = bufferBytes;
            bufferHandle = GCHandle.Alloc(this.bufferBytes, GCHandleType.Pinned);
            bufferPointer = Marshal.UnsafeAddrOfPinnedArrayElement(this.bufferBytes, 0);
        }

        public void Dispose()
        {
            bufferHandle.Free();
        }
    }


    public class MeshInfo
    {
        public BufferView IndicesBufferView;
        public BufferView[] AttributeBufferView;

        public MeshInfo(BufferView indices, BufferView[] attributes)
        {
            IndicesBufferView = indices;
            AttributeBufferView = attributes;
        }
    }


    public class GLTFLoader_0 : IDisposable
    {
        public Gltf model;

        public BufferInfo[] Buffers;
        public MeshInfo[] Meshes;

        public GLTFLoader_0(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                if (stream == null || !stream.CanRead)
                {
                    throw new ArgumentException("Invalid parameter. Stream must be readable", "imageStream");
                }

                Stream seekedStream = stream;
                MemoryStream memstream = null;
                if (!stream.CanSeek)
                {
                    memstream = new MemoryStream();
                    stream.CopyTo(memstream);

                    memstream.Seek(0, SeekOrigin.Begin);
                    seekedStream = memstream;
                }

                model = Interface.LoadModel(stream);
                ReadModel(filePath);
            }
        }

        private void ReadModel(string filePath)
        {
            // read all buffers
            int numBuffers = model.Buffers.Length;
            Buffers = new BufferInfo[numBuffers];

            for (int i = 0; i < numBuffers; ++i)
            {
                byte[] bufferBytes = model.LoadBinaryBuffer(i, filePath);
                Buffers[i] = new BufferInfo(bufferBytes);
            }

            // Read meshes
            int meshCount = model.Meshes.Length;
            Meshes = new MeshInfo[meshCount];
            for (int m = 0; m < meshCount; m++)
            {
                glTFLoader.Schema.Mesh mesh = model.Meshes[m];

                BufferView indices = null;
                BufferView[] attributes = null;
                for (int p = 0; p < mesh.Primitives.Length; p++)
                {
                    MeshPrimitive primitive = mesh.Primitives[p];

                    if (primitive.Indices.HasValue)
                    {
                        indices = ReadAccessor(primitive.Indices.Value);
                    }

                    int attributeCount = primitive.Attributes.Values.Count;
                    attributes = new BufferView[attributeCount];
                    int insertIndex = 0;
                    foreach (KeyValuePair<string, int> attribute in primitive.Attributes)
                    {
                        attributes[insertIndex++] = ReadAccessor(attribute.Value);
                    }
                }

                Meshes[m] = new MeshInfo(indices, attributes);
            }
        }


        private BufferView ReadAccessor(int index)
        {
            Accessor accessor = model.Accessors[index];

            if (accessor.BufferView.HasValue)
            {
                return model.BufferViews[accessor.BufferView.Value];
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (Buffers == null)
            {
                return;
            }

            for (int i = 0; i < Buffers.Length; i++)
            {
                Buffers[i].Dispose();
            }

            Buffers = null;
        }
    }


    public unsafe class GLTFLoader
    {

        private readonly List<Vector3> _Positions;
        private readonly List<Vector3> _Normal;
        private readonly List<Vector2> _Texture;
        private readonly List<Vector3> _Color;


        public int[] Indices;

        public GraphicsPipelineState GLTFPipeline { get; set; }

        public TextureData TextureData { get; set; }


        public Gltf model;

        public BufferInfo[] Buffers;
        public MeshInfo[] Meshes;
        private readonly List<VertexPositionNormal> vertices;

        public GLTFLoader(string FileName)
        {

            //Vertices = new ModelPart<Vertex>();

            //Indices = new ModelPart<int>();


            GLTFLoader_0 gltf = new GLTFLoader_0(FileName);

            MeshInfo mesh = gltf.Meshes[0];






            // Positions
            BufferView vpositionsBufferView = mesh.AttributeBufferView[1];
            int positionsCount = vpositionsBufferView.ByteLength / Unsafe.SizeOf<Vector3>();
            System.Numerics.Vector3* positions = stackalloc System.Numerics.Vector3[positionsCount];
            IntPtr vpositionsPointer = gltf.Buffers[vpositionsBufferView.Buffer].bufferPointer + vpositionsBufferView.ByteOffset;
            Unsafe.CopyBlock(positions, (void*)vpositionsPointer, (uint)vpositionsBufferView.ByteLength);

            // Normals
            BufferView vnormalsBufferView = mesh.AttributeBufferView[0];
            int normalsCount = vnormalsBufferView.ByteLength / Unsafe.SizeOf<Vector3>();
            System.Numerics.Vector3* normals = stackalloc System.Numerics.Vector3[normalsCount];
            IntPtr vnormalsPointer = gltf.Buffers[vnormalsBufferView.Buffer].bufferPointer + vnormalsBufferView.ByteOffset;
            Unsafe.CopyBlock(normals, (void*)vnormalsPointer, (uint)vnormalsBufferView.ByteLength);

            // Texcoords
            BufferView vtexcoordsBufferView = mesh.AttributeBufferView[2];
            int texcoordsCount = vtexcoordsBufferView.ByteLength / Unsafe.SizeOf<Vector2>();
            System.Numerics.Vector2* texcoords = stackalloc System.Numerics.Vector2[texcoordsCount];
            IntPtr vtexcoordsPointer = gltf.Buffers[vtexcoordsBufferView.Buffer].bufferPointer + vtexcoordsBufferView.ByteOffset;
            Unsafe.CopyBlock(texcoords, (void*)vtexcoordsPointer, (uint)vtexcoordsBufferView.ByteLength);


            // Index buffer
            // Indices
            BufferView indicesBufferView = mesh.IndicesBufferView;
            int indicesCount = indicesBufferView.ByteLength / sizeof(ushort);
            ushort[] indexData = new ushort[indicesCount];
            IntPtr indicesPointer = gltf.Buffers[indicesBufferView.Buffer].bufferPointer + indicesBufferView.ByteOffset;
            fixed (ushort* indicesPtr = indexData)
            {
                Unsafe.CopyBlock(indicesPtr, (void*)indicesPointer, (uint)indicesBufferView.ByteLength);
            }


            _Positions = new();
            _Normal = new();
            _Texture = new();

            vertices = new();

            for (int i = 0; i < positionsCount; i++)
            {

                //_Positions[i] = positions[i];
                //_Normal[i] = normals[i];
                //_Texture[i] = texcoords[i];


                vertices.Add(new VertexPositionNormal()
                {
                    Position = positions[i],
                    Normal = normals[i],
                });
            }

            Indices = new int[indexData.Length];

            for (int i = 0; i < Indices.Length; i++)
            {
                Indices[i] = indexData[i];
            }

            //foreach (ushort item in indexData)
            //{
            //    idx.Add((int)item);

            //}

            //Indices = idx.ToArray();


        }


        private void ReadModel(string filePath)
        {
            // read all buffers
            int numBuffers = model.Buffers.Length;
            Buffers = new BufferInfo[numBuffers];

            for (int i = 0; i < numBuffers; ++i)
            {
                byte[] bufferBytes = model.LoadBinaryBuffer(i, filePath);
                Buffers[i] = new BufferInfo(bufferBytes);
            }

            // Read meshes
            int meshCount = model.Meshes.Length;
            Meshes = new MeshInfo[meshCount];
            for (int m = 0; m < meshCount; m++)
            {
                glTFLoader.Schema.Mesh mesh = model.Meshes[m];

                BufferView indices = null;
                BufferView[] attributes = null;
                for (int p = 0; p < mesh.Primitives.Length; p++)
                {
                    MeshPrimitive primitive = mesh.Primitives[p];

                    if (primitive.Indices.HasValue)
                    {
                        indices = ReadAccessor(primitive.Indices.Value);
                    }

                    int attributeCount = primitive.Attributes.Values.Count;
                    attributes = new BufferView[attributeCount];
                    int insertIndex = 0;
                    foreach (KeyValuePair<string, int> attribute in primitive.Attributes)
                    {
                        attributes[insertIndex++] = ReadAccessor(attribute.Value);
                    }
                }

                Meshes[m] = new MeshInfo(indices, attributes);
            }
        }


        private BufferView ReadAccessor(int index)
        {
            Accessor accessor = model.Accessors[index];

            if (accessor.BufferView.HasValue)
            {
                return model.BufferViews[accessor.BufferView.Value];
            }
            else
            {
                return null;
            }
        }



        public Span<VertexPositionColorTexture> GetVertexPositionColorTextureAsSpan()
        {
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[_Positions.Count];

            for (int i = 0; i < _Positions.Count; i++)
            {
                vertices[i] = new VertexPositionColorTexture()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    Color = _Color[i] != null ? _Color[i] : Vector3.One,
                    TextureCoordinate = _Texture[i] != null ? _Texture[i] : Vector2.One,
                };
            }

            return vertices.AsSpan(); // TODO: GetPositionNormal AsSpan
        }


        public Span<VertexPositionNormal> GetPositionNormalAsSpan()
        {
            VertexPositionNormal[] vertices = new VertexPositionNormal[_Positions.Count];

            for (int i = 0; i < _Positions.Count; i++)
            {
                vertices[i] = new VertexPositionNormal()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    Normal = _Normal[i] != null ? _Normal[i] : Vector3.One,
                };
            }

            return vertices.AsSpan(); // TODO: GetPositionNormal AsSpan
        }





        public VertexPositionColorTexture[] GetVertexPositionColorTextureAsArray()
        {
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[_Positions.Count];

            for (int i = 0; i < _Positions.Count; i++)
            {
                vertices[i] = new VertexPositionColorTexture()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    Color = Vector3.One,
                    TextureCoordinate = new Vector2(_Texture[i].X, _Texture[i].Y) //!= null ? _Texture[i] : Vector2.One,
                };

                //vertices[i].Position.Y *= -1.0f;
                //vertices[i].normal.y *= -1.0f;


                vertices[i].FlipWinding();
            }

            return vertices;
        }


        public VertexPositionTexture[] GetVertexPositionTextureAsArray()
        {
            VertexPositionTexture[] vertices = new VertexPositionTexture[_Positions.Count];

            for (int i = 0; i < _Positions.Count; i++)
            {
                vertices[i] = new VertexPositionTexture()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    TextureCoordinate = new Vector2(2 * _Texture[i].X, 2 * _Texture[i].Y + 1) //!= null ? _Texture[i] : Vector2.One,
                };

                //vertices[i].Position.Y *= -1.0f;
                //vertices[i].normal.y *= -1.0f;


                //vertices[i].FlipWinding();
            }

            return vertices;
        }

        public VertexPositionNormal[] GetPositionNormalAsArray()
        {
            return vertices.ToArray();
        }


        public IEnumerable<VertexPositionNormal> GetPositionNormal()
        {
            for (int i = 0; i < _Positions.Count; i++)
            {
                yield return new VertexPositionNormal()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    Normal = _Normal[i] != null ? _Normal[i] : Vector3.One,
                };
            }
        }

        public IEnumerable<VertexPositionNormalColor> GetPositionNormalColor()
        {
            for (int i = 0; i < _Positions.Count; i++)
            {
                yield return new VertexPositionNormalColor()
                {
                    Position = _Positions[i] != null ? _Positions[i] : Vector3.One,
                    Normal = _Normal[i] != null ? _Normal[i] : Vector3.One,
                    Color = _Color[i] != null ? _Color[i] : Vector3.One,
                };
            }
        }


        public void Update()
        {

        }



    }
}
