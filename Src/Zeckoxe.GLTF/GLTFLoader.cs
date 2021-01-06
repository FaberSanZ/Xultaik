// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	GLTFLoader.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Zeckoxe.Core;
using Zeckoxe.GLTF.Schema;
using Zeckoxe.Vulkan;


namespace Zeckoxe.GLTF
{


    public enum ColorType
    {
        Vec3,

        Vec4,
    }

    public enum DescriptorBindingFlags
    {
        ImageBaseColor = 0x00000001,
        ImageNormalMap = 0x00000002
    }



    public enum FileLoadingFlags
    {
        None = 1 << 0,
        PreTransformPushConstantVertex = 1 << 1,
        DontLoadImages = 1 << 2
    };

    public enum RenderFlags
    {
        BindImages = 0x00000001,
        RenderOpaqueNodes = 0x00000002,
        RenderAlphaMaskedNodes = 0x00000004,
        RenderAlphaBlendedNodes = 0x00000008
    };


    public class GLTFOptions
    {
        public FileLoadingFlags FileLoading { get; set; }
    }
    public class GLTFLoader<TVertex>
    {
        internal Gltf gltf;
        internal string baseDirectory;

        internal byte[][] loadedBuffers;
        internal GCHandle[] bufferHandles;

        internal Device _device;

        internal List<Node> nodes;
        internal List<Node> linearNodes;

        internal bool buffersBound = false;

        private readonly int defaultSceneIndex = -1;
        private readonly List<Scene> Scenes;
        private readonly string _path;


        public GLTFLoader(Device device, string path, GLTFOptions options = default)
        {
            _device = device;
            _path = path;

            if (options == default)
            {
                options = new()
                {
                    FileLoading = FileLoadingFlags.PreTransformPushConstantVertex,
                };
            }

            Options = options;
            baseDirectory = Path.GetDirectoryName(path);


            gltf = Interface.LoadModel(path);
            loadedBuffers = new byte[gltf.Buffers.Length][];
            bufferHandles = new GCHandle[gltf.Buffers.Length];


            Meshes = LoadMeshes();

            Scenes = LoadScenes();
        }




        public uint ImageCount => gltf.Images == null ? 0 : (uint)gltf.Images.Length;

        public GLTFOptions Options { get; }

        public ColorType ColorType { get; set; }

        public TextureData TextureData { get; set; }

        public Vulkan.Buffer VertexBuffer { get; private set; }
        public int VertexCount { get; private set; }

        public Vulkan.Buffer IndexBuffer { get; private set; }
        public int IndexCount { get; private set; }

        public List<Mesh> Meshes { get; set; }

        public IndexType IndexType { get; set; }


        internal byte[] load_data_uri(Schema.Image img)
        {
            int idxComa = img.Uri.IndexOf(",", 5, StringComparison.Ordinal);
            return Convert.FromBase64String(img.Uri.Substring(idxComa + 1));
        }

        internal byte[] load_data_uri(Schema.Buffer buff)
        {
            int idxComa = buff.Uri.IndexOf(",", 5, StringComparison.Ordinal);
            return Convert.FromBase64String(buff.Uri.Substring(idxComa + 1));
        }




        public List<Texture2D> LoadTexture()
        {
            if (gltf.Images == null)
                return null;

            List<Texture2D> textures = new List<Texture2D>();

            foreach (Schema.Image img in gltf.Images)
            {
                Texture2D texture = null;

                string imgName = img.Name;

                if (img.BufferView is not null)
                {
                    //load image from gltf buffer view
                    Schema.BufferView bv = gltf.BufferViews[(int)img.BufferView];
                    ensure_buffer_is_loaded(bv.Buffer);
                }
                else if (img.Uri.StartsWith("data:", StringComparison.Ordinal))
                {
                    //load base64 encoded image
                    texture = Texture2D.LoadFromData(_device, load_data_uri(img));
                }
                else
                {
                    texture = Texture2D.LoadFromFile(_device, Path.Combine(baseDirectory, img.Uri));
                    imgName += ";" + img.Uri;
                }

                if (texture is not null)
                    textures.Add(texture);


            }
            return textures;
        }

        private void ensure_buffer_is_loaded(int bufferIdx)
        {
            if (loadedBuffers[bufferIdx] == null)
            {
                //load full buffer
                string uri = gltf.Buffers[bufferIdx].Uri;
                if (string.IsNullOrEmpty(uri))//glb
                {
                    loadedBuffers[bufferIdx] = gltf.LoadBinaryBuffer(bufferIdx, _path);
                }
                else if (uri.StartsWith("data", StringComparison.Ordinal))
                {
                    loadedBuffers[bufferIdx] = load_data_uri(gltf.Buffers[bufferIdx]); //TODO: check this func=>System.Buffers.Text.Base64.EncodeToUtf8InPlace
                }
                else
                {
                    loadedBuffers[bufferIdx] = File.ReadAllBytes(Path.Combine(baseDirectory, gltf.Buffers[bufferIdx].Uri));
                }

                bufferHandles[bufferIdx] = GCHandle.Alloc(loadedBuffers[bufferIdx], GCHandleType.Pinned);
            }
        }

        private unsafe void AttributeCopyData<T>(ref T[] array, int attributeByteLength, void* attributePointer) where T : unmanaged
        {
            array = new T[attributeByteLength / Unsafe.SizeOf<T>()];

            fixed (T* arrayPtr = &array[0])
            {
                Unsafe.CopyBlock(arrayPtr, attributePointer, (uint)attributeByteLength);
            }
        }

        internal unsafe Mesh loadNode(Schema.Mesh mesh, List<int> indexBuffer)
        {
            Mesh m = new Mesh { Name = mesh.Name };

            foreach (Schema.MeshPrimitive primitive in mesh.Primitives)
            {
                ulong indexStart = (ulong)indexBuffer.Count;

                ulong vertexStart = 0;



                int indexCount = 0;
                int vertexCount = 0;

                Schema.Accessor AccPos = null;
                Schema.Accessor AccNorm = null;
                Schema.Accessor AccUv = null;
                Schema.Accessor AccColor = null;
                Schema.Accessor AccUv1 = null;

                Schema.BufferView bufferView = default;

                if (primitive.Attributes.TryGetValue("POSITION", out int accessorIdx))
                {
                    AccPos = gltf.Accessors[accessorIdx];
                    ensure_buffer_is_loaded(gltf.BufferViews[(int)AccPos.BufferView].Buffer);
                }
                if (primitive.Attributes.TryGetValue("NORMAL", out accessorIdx))
                {
                    AccNorm = gltf.Accessors[accessorIdx];
                    ensure_buffer_is_loaded(gltf.BufferViews[(int)AccNorm.BufferView].Buffer);
                }


                if (primitive.Attributes.TryGetValue("TEXCOORD_0", out accessorIdx))
                {
                    AccColor = gltf.Accessors[accessorIdx];
                    ensure_buffer_is_loaded(gltf.BufferViews[(int)AccColor.BufferView].Buffer);
                }
                if (primitive.Attributes.TryGetValue("TEXCOORD_1", out accessorIdx))
                {
                    AccUv1 = gltf.Accessors[accessorIdx];
                    ensure_buffer_is_loaded(gltf.BufferViews[(int)AccUv1.BufferView].Buffer);
                }



                //prim.BoundingBox.Min.ImportFloatArray(AccPos.Min);
                //prim.BoundingBox.Max.ImportFloatArray(AccPos.Max);
                //prim.BoundingBox.IsValid = true;

                //Interleaving vertices
                byte* inPosPtr = null, inNormPtr = null, inUvPtr = null, inUv1Ptr = null;

                Vector3[] positions = Array.Empty<Vector3>();
                Vector3[] normals = Array.Empty<Vector3>();
                Vector2[] texcoords = Array.Empty<Vector2>();
                Vector3[] tangents = Array.Empty<Vector3>();

                bufferView = gltf.BufferViews[(int)AccPos.BufferView];
                inPosPtr = (byte*)bufferHandles[bufferView.Buffer].AddrOfPinnedObject().ToPointer();
                inPosPtr += AccPos.ByteOffset + bufferView.ByteOffset;
                this.AttributeCopyData(ref positions, AccPos.Count * Unsafe.SizeOf<Vector3>(), inPosPtr);


                if (AccNorm is not null)
                {
                    bufferView = gltf.BufferViews[(int)AccNorm.BufferView];
                    inNormPtr = (byte*)bufferHandles[bufferView.Buffer].AddrOfPinnedObject().ToPointer();
                    inNormPtr += AccNorm.ByteOffset + bufferView.ByteOffset;

                    AttributeCopyData(ref normals, AccPos.Count * Unsafe.SizeOf<Vector3>(), inNormPtr);
                }


                if (AccUv is not null)
                {
                    bufferView = gltf.BufferViews[(int)AccUv.BufferView];
                    inUvPtr = (byte*)bufferHandles[bufferView.Buffer].AddrOfPinnedObject().ToPointer();
                    inUvPtr += AccUv.ByteOffset + bufferView.ByteOffset;

                    AttributeCopyData(ref texcoords, AccPos.Count * Unsafe.SizeOf<Vector2>(), inUvPtr);
                }


                vertexCount = AccPos.Count;


                //TODO: use vertex attributes scan for copying data if they exists
                for (int i = 0; i < AccPos.Count; i++)
                {
                    Vector3 position = positions[i];
                    Vector3 normal = (normals.Length > i) ? normals[i] : Vector3.Zero;
                    Vector2 uv = (texcoords.Length > i) ? texcoords[i] : Vector2.Zero;

                    //if (Options.VertexType == VertexType.VertexPositionColor)
                    //    vertexPositionColors.Add(new VertexPositionColor(position, normal));

                    //if (Options.VertexType == VertexType.VertexPositionTexture)
                    //    vertexPositionTextures.Add(new VertexPositionTexture(position, uv, Vector3.UnitZ));


                }

                //indices loading
                if (primitive.Indices is not null)
                {
                    Schema.Accessor acc = gltf.Accessors[(int)primitive.Indices];
                    bufferView = gltf.BufferViews[(int)acc.BufferView];


                    indexCount = acc.Count;

                    byte* inIdxPtr = (byte*)bufferHandles[bufferView.Buffer].AddrOfPinnedObject().ToPointer();
                    inIdxPtr += acc.ByteOffset + bufferView.ByteOffset;

                    switch (acc.ComponentType)
                    {
                        case GltfComponentType.UnsignedByte:
                            byte* buf_0 = stackalloc byte[acc.Count];
                            Unsafe.CopyBlock(buf_0, inIdxPtr, (uint)acc.Count * (uint)Interop.SizeOf<byte>());
                            for (int index = 0; index < acc.Count; index++)
                            {
                                indexBuffer.Add(buf_0[index] + (int)vertexStart);
                            }

                            break;


                        case GltfComponentType.UnsignedShort:
                            ushort* buf_1 = stackalloc ushort[acc.Count];
                            Unsafe.CopyBlock(buf_1, inIdxPtr, (uint)acc.Count * (uint)Interop.SizeOf<ushort>());
                            for (int index = 0; index < acc.Count; index++)
                            {
                                indexBuffer.Add(buf_1[index] + (int)vertexStart);
                            }

                            break;


                        case GltfComponentType.UnsignedInt:
                            int* buf_2 = stackalloc int[acc.Count];
                            Unsafe.CopyBlock(buf_2, inIdxPtr, (uint)acc.Count * (uint)Interop.SizeOf<int>());
                            for (int index = 0; index < acc.Count; index++)
                            {
                                indexBuffer.Add(buf_2[index] + (int)vertexStart);
                            }

                            break;


                        case GltfComponentType.Float:
                            float* buf_3 = stackalloc float[acc.Count];
                            Unsafe.CopyBlock(buf_3, inIdxPtr, (uint)acc.Count * (uint)Interop.SizeOf<int>());
                            for (int index = 0; index < acc.Count; index++)
                            {
                                indexBuffer.Add((int)buf_3[index] + (int)vertexStart);
                            }

                            break;


                        default:
                            throw new NotImplementedException();

                    }


                }


                Primitive prim = new Primitive
                {
                    FirstIndex = (int)indexStart,
                    FirstVertex = (int)vertexStart,
                    VertexCount = vertexCount,
                    IndexCount = indexCount,
                };

                m.AddPrimitive(prim);

            }


            return m;
        }


        public void GetVertexCount(out ulong vertexCount, out ulong indexCount, out IndexType largestIndexType)
        {
            vertexCount = 0;
            indexCount = 0;
            largestIndexType = IndexType.Uint16;
            //compute size of stagging buf
            foreach (Schema.Mesh mesh in gltf.Meshes)
            {
                foreach (Schema.MeshPrimitive p in mesh.Primitives)
                {
                    int accessorIdx;
                    if (p.Attributes.TryGetValue("POSITION", out accessorIdx))
                        vertexCount += (ulong)gltf.Accessors[accessorIdx].Count;
                    if (p.Indices != null)
                    {
                        indexCount += (ulong)gltf.Accessors[(int)p.Indices].Count;
                        if (gltf.Accessors[(int)p.Indices].ComponentType == GltfComponentType.UnsignedInt)
                            largestIndexType = IndexType.Uint32;
                    }
                }
            }
        }


        public unsafe List<Mesh> LoadMeshes()
        {
            GetVertexCount(out var vCount, out var iCount, out var idxType);

            IndexType = idxType;
            int vertexByteSize = Marshal.SizeOf<TVertex>();
            ulong vertSize = (ulong)((int)vCount * vertexByteSize);
            ulong idxSize = (ulong)iCount * (idxType is IndexType.Uint16 ? 2ul : 4ul);
            ulong size = vertSize + idxSize;

            //int vertexCount = 0, indexCount = 0;
            int autoNamedMesh = 1;

            int vertexCount = 0, indexCount = 0;

            VertexBuffer = new(_device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = (int)vertSize
            });

            IndexBuffer = new(_device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = (int)idxSize,
            });



            VertexBuffer.Map();
            IndexBuffer.Map();

            Meshes = new();


            IEnumerable<PropertyInfo> propertyInfos = typeof(TVertex).GetTypeInfo().GetRuntimeProperties();


            byte* stagVertPtrInit = (byte*)VertexBuffer.MappedData.ToPointer();
            byte* stagIdxPtrInit = (byte*)(IndexBuffer.MappedData.ToPointer());
            byte* stagVertPtr = stagVertPtrInit;
            byte* stagIdxPtr = stagIdxPtrInit;

            foreach (Schema.Mesh mesh in gltf.Meshes)
            {

                string meshName = mesh.Name;
                if (string.IsNullOrEmpty(meshName))
                {
                    meshName = "mesh_" + autoNamedMesh.ToString();
                    autoNamedMesh++;
                }
                Mesh m = new Mesh { Name = meshName };

                foreach (Schema.MeshPrimitive p in mesh.Primitives)
                {


                    Schema.Accessor AccPos = null, AccNorm = null, AccUv = null, AccUv1 = null, AccColor = null, AccTan = null;

                    if (p.Attributes.TryGetValue("POSITION", out int accessorIdx))
                    {
                        AccPos = gltf.Accessors[accessorIdx];
                        ensure_buffer_is_loaded(gltf.BufferViews[(int)AccPos.BufferView].Buffer);
                    }
                    if (p.Attributes.TryGetValue("NORMAL", out accessorIdx))
                    {
                        AccNorm = gltf.Accessors[accessorIdx];
                        ensure_buffer_is_loaded(gltf.BufferViews[(int)AccNorm.BufferView].Buffer);
                    }
                    if (p.Attributes.TryGetValue("TEXCOORD_0", out accessorIdx))
                    {
                        AccUv = gltf.Accessors[accessorIdx];
                        ensure_buffer_is_loaded(gltf.BufferViews[(int)AccUv.BufferView].Buffer);
                    }
                    if (p.Attributes.TryGetValue("TEXCOORD_1", out accessorIdx))
                    {
                        AccUv1 = gltf.Accessors[accessorIdx];
                        ensure_buffer_is_loaded(gltf.BufferViews[(int)AccUv1.BufferView].Buffer);
                    }

                    if (p.Attributes.TryGetValue("COLOR_0", out accessorIdx))
                    {
                        AccColor = gltf.Accessors[accessorIdx];
                        ensure_buffer_is_loaded(gltf.BufferViews[(int)AccColor.BufferView].Buffer);
                        ColorType = AccColor.Type is GltfType.Vec3 ? ColorType.Vec3 : ColorType.Vec4;
                    }

                    if (p.Attributes.TryGetValue("TANGENT", out accessorIdx))
                    {
                        AccTan = gltf.Accessors[accessorIdx];
                        ensure_buffer_is_loaded(gltf.BufferViews[(int)AccTan.BufferView].Buffer);
                        //ColorType = AccColor.Type is GltfType.Vec3 ? ColorType.Vec3 : ColorType.Vec4;

                        Console.WriteLine(accessorIdx);
                        Console.WriteLine("AccTan");

                    }


                    Primitive prim = new()
                    {
                        FirstIndex = indexCount,
                        FirstVertex = vertexCount,
                        VertexCount = AccPos.Count,
                        Material = (p.Material ?? 0)
                    };

                    //prim.BoundingBox.Min.ImportFloatArray(AccPos.Min);
                    //prim.BoundingBox.Max.ImportFloatArray(AccPos.Max);
                    //prim.BoundingBox.IsValid = true;

                    //Interleaving vertices
                    byte* inPosPtr = null, inNormPtr = null, inUvPtr = null, inUv1Ptr = null, inColorPtr = null, inTanPtr = null;

                    Schema.BufferView bv = gltf.BufferViews[(int)AccPos.BufferView];
                    inPosPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                    inPosPtr += AccPos.ByteOffset + bv.ByteOffset;

                    if (AccNorm is not null)
                    {
                        bv = gltf.BufferViews[(int)AccNorm.BufferView];
                        inNormPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                        inNormPtr += AccNorm.ByteOffset + bv.ByteOffset;

                    }
                    if (AccUv is not null)
                    {
                        bv = gltf.BufferViews[(int)AccUv.BufferView];
                        inUvPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                        inUvPtr += AccUv.ByteOffset + bv.ByteOffset;
                    }
                    if (AccUv1 is not null)
                    {
                        bv = gltf.BufferViews[(int)AccUv1.BufferView];
                        inUv1Ptr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                        inUv1Ptr += AccUv1.ByteOffset + bv.ByteOffset;
                    }

                    if (AccColor is not null)
                    {
                        bv = gltf.BufferViews[(int)AccColor.BufferView];
                        inColorPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                        inColorPtr += AccColor.ByteOffset + bv.ByteOffset;
                    }


                    if (AccTan is not null)
                    {
                        bv = gltf.BufferViews[(int)AccTan.BufferView];
                        inTanPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                        inTanPtr += AccTan.ByteOffset + bv.ByteOffset;
                    }

                    for (int j = 0; j < AccPos.Count; j++)
                    {

                        int pad = 0;

                        foreach (PropertyInfo info in propertyInfos)
                        {
                            VertexAttribute attribute = info.GetCustomAttribute<VertexAttribute>();

                            //Console.WriteLine(attribute.Type);

                            if (attribute is not null)
                            {

                                if (inPosPtr is not null && attribute.Type is VertexAttributeType.Position)
                                {
                                    //if (propertyInfos.First().GetCustomAttribute<VertexAttribute>().Type is attribute.Type)
                                    //    pad = 0;
                                    //else
                                    //    pad += 12;

                                    pad += 0;

                                    System.Buffer.MemoryCopy(inPosPtr, stagVertPtr + pad, 12, 12);
                                    inPosPtr += 12;
                                }



                                if (inTanPtr is not null && attribute.Type is VertexAttributeType.Tangent)
                                {
                                    //if (propertyInfos.First().GetCustomAttribute<VertexAttribute>().Type is attribute.Type)
                                    //    pad += 0;
                                    //else
                                    //    pad += 12;

                                    pad += 16;

                                    System.Buffer.MemoryCopy(inTanPtr, stagVertPtr + pad, 16, 16);
                                    inTanPtr += 16;
                                }


                                if (inColorPtr is not null && attribute.Type is VertexAttributeType.Color)
                                {
                                    if (ColorType is ColorType.Vec4)
                                    {
                                        pad += 16;
                                        System.Buffer.MemoryCopy(inColorPtr, stagVertPtr + pad, 16, 16);
                                        inColorPtr += 16;
                                    }
                                    else
                                    {
                                        pad += 12;
                                        System.Buffer.MemoryCopy(inColorPtr, stagVertPtr + pad, 12, 12);
                                        inColorPtr += 12;
                                    }

                                }

                                if (inNormPtr is not null && attribute.Type is VertexAttributeType.Normal)
                                {
                                    pad += 12;
                                    System.Buffer.MemoryCopy(inNormPtr, stagVertPtr + pad, 12, 12);
                                    inNormPtr += 12;
                                }

                                if (inUvPtr is not null && attribute.Type is VertexAttributeType.TextureCoordinate)
                                {
                                    pad += 8;
                                    System.Buffer.MemoryCopy(inUvPtr, stagVertPtr + pad, 8, 8);
                                    inUvPtr += 8;

                                }

                                if (inUv1Ptr is not null && attribute.Type is VertexAttributeType.TextureCoordinate1)
                                {
                                    pad += 8;
                                    System.Buffer.MemoryCopy(inUv1Ptr, stagVertPtr + pad, 8, 8);
                                    inUv1Ptr += 8;
                                }
                            }
                        }

                        stagVertPtr += vertexByteSize;
                    }


                    //indices loading
                    if (p.Indices != null)
                    {
                        Schema.Accessor acc = gltf.Accessors[(int)p.Indices];
                        bv = gltf.BufferViews[(int)acc.BufferView];

                        byte* inIdxPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                        inIdxPtr += acc.ByteOffset + bv.ByteOffset;

                        //TODO:double check this, I dont seems to increment stag pointer
                        if (acc.ComponentType == GltfComponentType.UnsignedShort)
                        {
                            if (IndexType == IndexType.Uint16)
                            {
                                System.Buffer.MemoryCopy(inIdxPtr, stagIdxPtr, (long)acc.Count * 2, (long)acc.Count * 2);
                                stagIdxPtr += (long)acc.Count * 2;
                            }
                            else
                            {
                                uint* usPtr = (uint*)stagIdxPtr;
                                ushort* inPtr = (ushort*)inIdxPtr;
                                for (int i = 0; i < acc.Count; i++)
                                    usPtr[i] = inPtr[i];
                                stagIdxPtr += (long)acc.Count * 4;
                            }
                        }
                        else if (acc.ComponentType == GltfComponentType.UnsignedInt)
                        {
                            if (IndexType == IndexType.Uint32)
                            {
                                System.Buffer.MemoryCopy(inIdxPtr, stagIdxPtr, (long)acc.Count * 4, (long)acc.Count * 4);
                                stagIdxPtr += (long)acc.Count * 4;
                            }
                            else
                            {
                                ushort* usPtr = (ushort*)stagIdxPtr;
                                uint* inPtr = (uint*)inIdxPtr;
                                for (int i = 0; i < acc.Count; i++)
                                    usPtr[i] = (ushort)inPtr[i];
                                stagIdxPtr += (long)acc.Count * 2;
                            }
                        }
                        else if (acc.ComponentType == GltfComponentType.UnsignedByte)
                        {
                            //convert
                            if (IndexType == IndexType.Uint16)
                            {
                                ushort* usPtr = (ushort*)stagIdxPtr;
                                for (int i = 0; i < acc.Count; i++)
                                    usPtr[i] = (ushort)inIdxPtr[i];
                                stagIdxPtr += (long)acc.Count * 2;
                            }
                            else
                            {
                                uint* usPtr = (uint*)stagIdxPtr;
                                for (int i = 0; i < acc.Count; i++)
                                    usPtr[i] = (uint)inIdxPtr[i];
                                stagIdxPtr += (long)acc.Count * 4;
                            }
                        }
                        else
                            throw new NotImplementedException();

                        prim.IndexCount = acc.Count;
                        indexCount += acc.Count;
                    }


                    m.AddPrimitive(prim);
                    vertexCount += AccPos.Count;
                }
                Meshes.Add(m);



                VertexBuffer.Unmap();
                IndexBuffer.Unmap();
            }

            return Meshes;
        }


        public Material[] LoadMaterial()
        {
            if (gltf.Materials == null)
                return new Material[] { };

            List<Material> materials = new List<Material>();

            foreach (Schema.Material mat in gltf.Materials)
            {
                //Debug.WriteLine("loading material: " + mat.Name);
                Material pbr = new Material();
                pbr.Name = mat.Name;

                pbr.alphaCutoff = mat.AlphaCutoff;
                //pbr.alphaCutoff = mat.NormalTexture.;
                //pbr.alphaMode = (AlphaMode)mat.AlphaMode;

                //FromFloatArray(ref pbr.emissiveFactor, mat.EmissiveFactor);
                Console.WriteLine(mat.Name);

                
                if (mat.EmissiveTexture != null)
                {
                    pbr.emissiveTexture = mat.EmissiveTexture.Index;
                    //if (mat.EmissiveTexture.TexCoord == 1)
                    //    pbr.availableAttachments1 |= AttachmentType.Emissive;
                    //else
                    //    pbr.availableAttachments |= AttachmentType.Emissive;
                }
                if (mat.NormalTexture != null)
                {
                    pbr.normalTexture = mat.NormalTexture.Index;
                    //if (mat.NormalTexture.TexCoord == 1)
                    //    pbr.availableAttachments1 |= AttachmentType.Normal;
                    //else
                    //    pbr.availableAttachments |= AttachmentType.Normal;
                }
                if (mat.OcclusionTexture != null)
                {
                    //pbr.occlusionTexture = mat.OcclusionTexture.Index;
                    //if (mat.OcclusionTexture.TexCoord == 1)
                    //    pbr.availableAttachments1 |= AttachmentType.AmbientOcclusion;
                    //else
                    //    pbr.availableAttachments |= AttachmentType.AmbientOcclusion;
                }

                if (mat.PbrMetallicRoughness != null)
                {
                    //if (mat.PbrMetallicRoughness.BaseColorTexture != null)
                    //{
                    //    pbr.baseColorTexture = mat.PbrMetallicRoughness.BaseColorTexture.Index;
                    //    if (mat.PbrMetallicRoughness.BaseColorTexture.TexCoord == 1)
                    //        pbr.availableAttachments1 |= AttachmentType.Color;
                    //    else
                    //        pbr.availableAttachments |= AttachmentType.Color;
                    //}

                    //FromFloatArray(ref pbr.baseColorFactor, mat.PbrMetallicRoughness.BaseColorFactor);

                    //if (mat.PbrMetallicRoughness.MetallicRoughnessTexture != null)
                    //{
                    //    pbr.metallicRoughnessTexture = mat.PbrMetallicRoughness.MetallicRoughnessTexture.Index;
                    //    if (mat.PbrMetallicRoughness.MetallicRoughnessTexture.TexCoord == 1)
                    //        pbr.availableAttachments1 |= AttachmentType.PhysicalProps;
                    //    else
                    //        pbr.availableAttachments |= AttachmentType.PhysicalProps;
                    //}
                    pbr.metallicFactor = mat.PbrMetallicRoughness.MetallicFactor;
                    pbr.roughnessFactor = mat.PbrMetallicRoughness.RoughnessFactor;

                    pbr.workflow = Material.Workflow.PhysicalyBaseRendering;
                }
                materials.Add(pbr);
            }
            return materials.ToArray();
        }



        public void PreTransform(Node node, Matrix4x4 currentTransform)
        {
            Matrix4x4 localMat = node.LocalMatrix * currentTransform;

            ////vertexBuffer.Clear();
            //if (node.Mesh is not null)
            //{
            //    foreach (Primitive p in node.Mesh.Primitives)
            //    {
            //        for (int i = 0; i < p.VertexCount; i++)
            //        {
            //            int index = p.FirstVertex + i;

            //            Vector3 position = Vector3.Transform(vertexPositionTextures[index].Position, localMat);
            //            Vector3 color = Vector3.TransformNormal(vertexPositionTextures[index].color, localMat);

            //            // TODO: Normalize?
            //            color = Vector3.Normalize(color);

            //            vertexPositionTextures[index] = new(position, vertexPositionTextures[index].TextureCoordinate, color);
            //        }
            //    }
            //}
            //if (node.Children is null)
            //    return;

            foreach (Node child in node.Children)
                PreTransform(child, localMat);
        }

        public void RenderNode(CommandBuffer cmd, Node node, Matrix4x4 currentTransform, GraphicsPipelineState pipelineState)
        {
            Matrix4x4 localMat = node.LocalMatrix * currentTransform;


            if ((Options.FileLoading & FileLoadingFlags.PreTransformPushConstantVertex) != 0)
                cmd.PushConstant<Matrix4x4>(pipelineState, ShaderStage.Vertex, localMat);



            if (node.Mesh is not null)
                foreach (Primitive p in node.Mesh.Primitives)
                    cmd.DrawIndexed(p.IndexCount, 1, p.FirstIndex, p.FirstVertex, 0);


            if (node.Children is null) 
                return;

            foreach (Node child in node.Children)
                RenderNode(cmd, child, localMat, pipelineState);
        }




        public void DrawNode(CommandBuffer commandBuffer, Mesh m)
        {
            foreach (Primitive primitive in m.Primitives)
            {
                commandBuffer.DrawIndexed(primitive.IndexCount, primitive.FirstIndex, primitive.FirstVertex, 0, 0);
            }
        }
        public void Draw(CommandBuffer commandBuffer)
        {
            commandBuffer.SetVertexBuffers(new[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer, 0, IndexType);


            foreach (var m in Meshes)
                DrawNode(commandBuffer, m);
        }


        public void Draw(CommandBuffer commandBuffer, GraphicsPipelineState pipelineState)
        {
            commandBuffer.SetVertexBuffers(new[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer, 0, IndexType);


            //commandBuffer.BindDescriptorSets(new DescriptorSet);

            foreach (Scene sc in Scenes)
            {
                foreach (Node node in sc.Root.Children)
                {
                    RenderNode(commandBuffer, node, sc.Root.LocalMatrix, pipelineState);
                }
            }


        }



        public List<Scene> LoadScenes()
        {
            int defaultScene = -1;
            if (gltf.Scene is null)
            {
                return new();
            }

            List<Scene> scenes = new();
            defaultScene = (int)gltf.Scene;

            for (int i = 0; i < gltf.Scenes.Length; i++)
            {
                Schema.Scene scene = gltf.Scenes[i];
                //Debug.WriteLine("Loading Scene {0}", scene.Name);

                scenes.Add(new()
                {
                    Name = scene.Name,
                });

                if (scene.Nodes.Length is 0)
                {
                    continue;
                }

                scenes[i].Root = new()
                {
                    LocalMatrix = Matrix4x4.Identity,
                    Children = new()
                };

                foreach (int nodeIdx in scene.Nodes)
                {
                    LoadNode(scenes[i].Root, gltf.Nodes[nodeIdx]);
                }
            }
            return scenes;
        }




        public unsafe void LoadNode(Node parentNode, Schema.Node gltfNode)
        {
            //Debug.WriteLine("Loading node {0}", gltfNode.Name);

            Vector3 translation = new();
            System.Numerics.Quaternion rotation = System.Numerics.Quaternion.Identity;
            Vector3 scale = new(1);
            Matrix4x4 localTransform = Matrix4x4.Identity;

            if (gltfNode.Matrix is not null)
            {
                float[] M = gltfNode.Matrix;

                //fixed(float* ptr = M)
                //    localTransform = *(Matrix4x4*)ptr;

                localTransform = new(
                    M[0], M[1], M[2], M[3],
                    M[4], M[5], M[6], M[7],
                    M[8], M[9], M[10], M[11],
                   M[12], M[13], M[14], M[15]);
            }

            if (gltfNode.Translation is not null)
            {
                FromFloatArray(ref translation, gltfNode.Translation);
            }

            if (gltfNode.Translation is not null)
            {
                FromFloatArray(ref rotation, gltfNode.Rotation);
            }

            if (gltfNode.Translation is not null)
            {
                FromFloatArray(ref scale, gltfNode.Scale);
            }

            localTransform *= Matrix4x4.CreateScale(scale) * Matrix4x4.CreateFromQuaternion(rotation) * Matrix4x4.CreateTranslation(translation);

            //localTransform = Matrix4x4.Identity;

            Node node = new()
            {
                LocalMatrix = localTransform,
                Parent = parentNode,
                Name = gltfNode.Name
            };

            parentNode.Children.Add(node);

            if (gltfNode.Children != null)
            {
                node.Children = new();
                for (int i = 0; i < gltfNode.Children.Length; i++)
                {
                    LoadNode(node, gltf.Nodes[gltfNode.Children[i]]);
                }
            }

            if (gltfNode.Mesh != null)
            {
                node.Mesh = Meshes[(int)gltfNode.Mesh];
            }
        }

        public static void FromFloatArray(ref Vector3 v, float[] floats)
        {
            if (floats.Length > 0)
            {
                v.X = floats[0];
            }

            if (floats.Length > 1)
            {
                v.Y = floats[1];
            }

            if (floats.Length > 2)
            {
                v.Z = floats[2];
            }
        }


        public static void FromFloatArray(ref System.Numerics.Quaternion v, float[] floats)
        {
            if (floats.Length > 0)
            {
                v.X = floats[0];
            }

            if (floats.Length > 1)
            {
                v.Y = floats[1];
            }

            if (floats.Length > 2)
            {
                v.Z = floats[2];
            }

            if (floats.Length > 3)
            {
                v.W = floats[3];
            }
        }

        public void Update()
        {

        }

    }
}
