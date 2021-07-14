// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vortice.Vulkan;
using Vultaik.GLTF.Schema;
using Vultaik;
using Interop = Vultaik.Interop;

namespace Vultaik.GLTF
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
        public bool Skinning { get; set; }
    }
    public class ModelAssetImporter<TVertex>
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
        public List<Scene> Scenes;
        private readonly string _path;


        public ModelAssetImporter(Device device, string path, GLTFOptions options = default)
        {
            _device = device;
            _path = path;

            if (options == default)
            {
                options = new()
                {
                    FileLoading = FileLoadingFlags.PreTransformPushConstantVertex,
                    Skinning = false,
                };
            }

            Options = options;
            baseDirectory = Path.GetDirectoryName(path);


            gltf = Interface.LoadModel(path);
            loadedBuffers = new byte[gltf.Buffers.Length][];
            bufferHandles = new GCHandle[gltf.Buffers.Length];


            if (gltf.Skins is not null && gltf.Skins.Length > 0)
            {
                LoadSkins(gltf);

            }



            Meshes = LoadMeshes();

            Scenes = LoadScenes();


        }




        public uint ImageCount => gltf.Images is null ? 0 : (uint)gltf.Images.Length;

        public GLTFOptions Options { get; }

        public ColorType ColorType { get; set; }

        public ImageDescription ImageDescription { get; set; }

        public Vultaik.Buffer VertexBuffer { get; private set; }
        public int VertexCount { get; private set; }

        public Vultaik.Buffer IndexBuffer { get; private set; }
        public int IndexCount { get; private set; }

        public List<Mesh> Meshes { get; set; }

        public VkIndexType IndexType { get; set; }


        internal byte[] LoadDataUri(Schema.Image img)
        {
            int idxComa = img.Uri.IndexOf(",", 5, StringComparison.Ordinal);
            return Convert.FromBase64String(img.Uri.Substring(idxComa + 1));
        }

        internal byte[] LoadDataUri(Schema.Buffer buff)
        {
            int idxComa = buff.Uri.IndexOf(",", 5, StringComparison.Ordinal);
            return Convert.FromBase64String(buff.Uri.Substring(idxComa + 1));
        }




        public List<Vultaik.Image> LoadTexture()
        {
            if (gltf.Images is null)
                return Array.Empty<Vultaik.Image>().ToList();

            List<Vultaik.Image> textures = new();

            foreach (Schema.Image img in gltf.Images)
            {
                Vultaik.Image texture = null;

                string imgName = img.Name;

                if (img.BufferView is not null)
                {
                    //load image from gltf buffer view
                    Schema.BufferView bv = gltf.BufferViews[(int)img.BufferView];
                    EnsureBufferIsLoaded(bv.Buffer);
                }
                else if (img.Uri.StartsWith("data:", StringComparison.Ordinal))
                {
                    //load base64 encoded image
                    texture = ImageFile.Load2DFromBytes(_device, LoadDataUri(img));
                }
                else
                {
                    texture = ImageFile.Load2DFromFile(_device, Path.Combine(baseDirectory, img.Uri));
                    imgName += ";" + img.Uri;
                }

                if (texture is not null)
                    textures.Add(texture);


            }
            return textures;
        }

        private void EnsureBufferIsLoaded(int index)
        {
            if (loadedBuffers[index] == null)
            {
                //load full buffer
                string uri = gltf.Buffers[index].Uri;
                if (string.IsNullOrEmpty(uri))//glb
                {
                    loadedBuffers[index] = gltf.LoadBinaryBuffer(index, _path);
                }
                else if (uri.StartsWith("data", StringComparison.Ordinal))
                {
                    loadedBuffers[index] = LoadDataUri(gltf.Buffers[index]); //TODO: check this func=>System.Buffers.Text.Base64.EncodeToUtf8InPlace
                }
                else
                {
                    loadedBuffers[index] = File.ReadAllBytes(Path.Combine(baseDirectory, gltf.Buffers[index].Uri));
                }

                bufferHandles[index] = GCHandle.Alloc(loadedBuffers[index], GCHandleType.Pinned);
            }
        }



        public void GetVertexCount(out ulong vertexCount, out ulong indexCount, out VkIndexType largestIndexType)
        {
            vertexCount = 0;
            indexCount = 0;
            largestIndexType = VkIndexType.Uint16;
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
                            largestIndexType = VkIndexType.Uint32;
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
            ulong indexSize = (ulong)iCount * (idxType is VkIndexType.Uint16 ? 2ul : 4ul);

            int autoNamedMesh = 1;

            int vertexCount = 0, indexCount = 0;

            VertexBuffer = new(_device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = (int)vertSize
            });

            IndexBuffer = new(_device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = (int)indexSize,
            });



            Meshes = new();


            IEnumerable<PropertyInfo> propertyInfos = typeof(TVertex).GetTypeInfo().GetRuntimeProperties();


            byte* stagVertPtrInit = (byte*)VertexBuffer.Map();
            byte* stagIdxPtrInit = (byte*)(IndexBuffer.Map());
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


                    Schema.Accessor AccPos = null, AccNorm = null, AccUv = null, AccUv1 = null, AccColor = null, AccTan = null, Accjoint = null, Accweights = null;

                    if (p.Attributes.TryGetValue("POSITION", out int accessorIdx))
                    {
                        AccPos = gltf.Accessors[accessorIdx];
                        EnsureBufferIsLoaded(gltf.BufferViews[(int)AccPos.BufferView].Buffer);
                    }
                    if (p.Attributes.TryGetValue("NORMAL", out accessorIdx))
                    {
                        AccNorm = gltf.Accessors[accessorIdx];
                        EnsureBufferIsLoaded(gltf.BufferViews[(int)AccNorm.BufferView].Buffer);
                    }
                    if (p.Attributes.TryGetValue("TEXCOORD_0", out accessorIdx))
                    {
                        AccUv = gltf.Accessors[accessorIdx];
                        EnsureBufferIsLoaded(gltf.BufferViews[(int)AccUv.BufferView].Buffer);
                    }
                    if (p.Attributes.TryGetValue("TEXCOORD_1", out accessorIdx))
                    {
                        AccUv1 = gltf.Accessors[accessorIdx];
                        EnsureBufferIsLoaded(gltf.BufferViews[(int)AccUv1.BufferView].Buffer);
                    }

                    if (p.Attributes.TryGetValue("COLOR_0", out accessorIdx))
                    {
                        AccColor = gltf.Accessors[accessorIdx];
                        EnsureBufferIsLoaded(gltf.BufferViews[(int)AccColor.BufferView].Buffer);
                        ColorType = AccColor.Type is GltfType.Vec3 ? ColorType.Vec3 : ColorType.Vec4;
                    }

                    if (p.Attributes.TryGetValue("TANGENT", out accessorIdx))
                    {
                        AccTan = gltf.Accessors[accessorIdx];
                        EnsureBufferIsLoaded(gltf.BufferViews[(int)AccTan.BufferView].Buffer);
                    }

                    if (Options.Skinning)
                    {
                        if (p.Attributes.TryGetValue("JOINTS_0", out accessorIdx))
                        {
                            Accjoint = gltf.Accessors[accessorIdx];
                            EnsureBufferIsLoaded(gltf.BufferViews[(int)Accjoint.BufferView].Buffer);
                        }

                        if (p.Attributes.TryGetValue("WEIGHTS_0", out accessorIdx))
                        {
                            Accweights = gltf.Accessors[accessorIdx];
                            EnsureBufferIsLoaded(gltf.BufferViews[(int)Accweights.BufferView].Buffer);
                        }

                        
                    }


                    


                    Primitive prim = new()
                    {
                        FirstIndex = indexCount,
                        FirstVertex = vertexCount,
                        VertexCount = AccPos.Count,
                        Material = p.Material ?? 0
                    };

                    //prim.BoundingBox.Min.ImportFloatArray(AccPos.Min);
                    //prim.BoundingBox.Max.ImportFloatArray(AccPos.Max);
                    //prim.BoundingBox.IsValid = true;

                    //Interleaving vertices
                    byte* inPosPtr = null;
                    byte* inNormPtr = null;
                    byte* inUvPtr = null;
                    byte* inUv1Ptr = null;
                    byte* inColorPtr = null;
                    byte* inTanPtr = null;
                    byte* inJointPtr = null;
                    byte* inWeightsPtr = null;


                    Schema.BufferView bv = gltf.BufferViews[(int)AccPos.BufferView!];
                    inPosPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                    inPosPtr += AccPos.ByteOffset + bv.ByteOffset;

                    if (AccNorm is not null)
                    {
                        bv = gltf.BufferViews[(int)AccNorm.BufferView!];
                        inNormPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                        inNormPtr += AccNorm.ByteOffset + bv.ByteOffset;

                    }
                    if (AccUv is not null)
                    {
                        bv = gltf.BufferViews[(int)AccUv.BufferView!];
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



                    if (Accjoint is not null)
                    {
                        bv = gltf.BufferViews[(int)Accjoint.BufferView];
                        inJointPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                        inJointPtr += Accjoint.ByteOffset + bv.ByteOffset;
                    }



                    if (Accweights is not null)
                    {
                        bv = gltf.BufferViews[(int)Accweights.BufferView];
                        inWeightsPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                        inWeightsPtr += Accweights.ByteOffset + bv.ByteOffset;
                    }

                    for (int j = 0; j < AccPos.Count; j++)
                    {

                        int pad = 0;

                        foreach (PropertyInfo info in propertyInfos)
                        {
                            VertexAttribute attribute = info.GetCustomAttribute<VertexAttribute>();


                            if (attribute is not null)
                            {

                                if (inPosPtr is not null && attribute.Type is VertexType.Position)
                                {
                                    if (propertyInfos.First().GetCustomAttribute<VertexAttribute>().Type == attribute.Type)
                                        pad = 0;
                                    else
                                        pad += 12;


                                    System.Buffer.MemoryCopy(inPosPtr, stagVertPtr + pad, 12, 12);
                                    inPosPtr += 12;
                                }



                                if (inTanPtr is not null && attribute.Type is VertexType.Tangent)
                                {
                                    if (propertyInfos.First().GetCustomAttribute<VertexAttribute>().Type == attribute.Type)
                                        pad += 0;
                                    else
                                        pad += 16;


                                    System.Buffer.MemoryCopy(inTanPtr, stagVertPtr + pad, 16, 16);
                                    inTanPtr += 16;
                                }


                                if (inColorPtr is not null && attribute.Type is VertexType.Color)
                                {
                                    if (ColorType is ColorType.Vec4)
                                    {
                                        if (propertyInfos.First().GetCustomAttribute<VertexAttribute>().Type == attribute.Type)
                                            pad += 0;
                                        else
                                            pad += 16;
                                        System.Buffer.MemoryCopy(inColorPtr, stagVertPtr + pad, 16, 16);
                                        inColorPtr += 16;
                                    }
                                    else
                                    {
                                        if (propertyInfos.First().GetCustomAttribute<VertexAttribute>()!.Type == attribute.Type)
                                            pad += 0;
                                        else
                                            pad += 12;


                                        System.Buffer.MemoryCopy(inColorPtr, stagVertPtr + pad, 12, 12);
                                        inColorPtr += 12;
                                    }

                                }

                                if (inNormPtr is not null && attribute.Type is VertexType.Normal)
                                {
                                    if (propertyInfos.First().GetCustomAttribute<VertexAttribute>()!.Type == attribute.Type)
                                        pad += 0;
                                    else
                                        pad += 12;

                                    System.Buffer.MemoryCopy(inNormPtr, stagVertPtr + pad, 12, 12);
                                    inNormPtr += 12;
                                }

                                if (inUvPtr is not null && attribute.Type is VertexType.TextureCoordinate)
                                {
                                    if (propertyInfos.First().GetCustomAttribute<VertexAttribute>().Type == attribute.Type)
                                        pad += 0;
                                    else
                                        pad += 12;

                                    System.Buffer.MemoryCopy(inUvPtr, stagVertPtr + pad, 8, 8);
                                    inUvPtr += 8;

                                }

                                if (inUv1Ptr is not null && attribute.Type is VertexType.TextureCoordinate1)
                                {
                                    if (propertyInfos.First().GetCustomAttribute<VertexAttribute>()!.Type == attribute.Type)
                                        pad += 0;
                                    else
                                        pad += 8;

                                    System.Buffer.MemoryCopy(inUv1Ptr, stagVertPtr + pad, 8, 8);
                                    inUv1Ptr += 8;
                                }



                                if (inJointPtr is not null && attribute.Type is VertexType.Joint0)
                                {
                                    if (propertyInfos.First().GetCustomAttribute<VertexAttribute>()!.Type == attribute.Type)
                                        pad += 0;
                                    else
                                        pad += 16;

                                    System.Buffer.MemoryCopy(inJointPtr, stagVertPtr + pad, 16, 16);
                                    inUv1Ptr += 16;
                                }


                                if (inWeightsPtr is not null && attribute.Type is VertexType.Weight0)
                                {
                                    if (propertyInfos.First().GetCustomAttribute<VertexAttribute>()!.Type == attribute.Type)
                                        pad += 0;
                                    else
                                        pad += 16;

                                    System.Buffer.MemoryCopy(inWeightsPtr, stagVertPtr + pad, 16, 16);
                                    inUv1Ptr += 16;
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
                            if (IndexType == VkIndexType.Uint16)
                            {
                                Interop.MemoryHelper.CopyMemory(inIdxPtr, stagIdxPtr, acc.Count * 2, acc.Count * 2);
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
                            if (IndexType == VkIndexType.Uint32)
                            {
                                Interop.MemoryHelper.CopyMemory(inIdxPtr, stagIdxPtr, acc.Count * 4, acc.Count * 4);
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
                            if (IndexType == VkIndexType.Uint16)
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


        // TODO: PreTransform?
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

            //            
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

        public void RenderNode(CommandBuffer cmd, Node node, Matrix4x4 currentTransform, GraphicsPipeline pipelineState)
        {
            Matrix4x4 localMat = node.LocalMatrix * currentTransform;

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


        public void Draw(CommandBuffer commandBuffer, GraphicsPipeline pipelineState)
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


        public void LoadSkins(Gltf input)
        {
            for (int i = 0; i < input.Skins.Length ; i++)
            {
                var skin = input.Skins[i];

                Console.WriteLine(skin.Name);
                Console.WriteLine(skin.Joints.Length);
            }
        }


        public void UpdateJoints(Schema.Node node)
        {
            if (node.Skin > -1)
            {
                //// Update the joint matrices
                //glm::mat4 inverseTransform = glm::inverse(getNodeMatrix(node));
                //Skin skin = skins[node->skin];
                //size_t numJoints = (uint32_t)skin.joints.size();
                //std::vector<glm::mat4> jointMatrices(numJoints);
                //for (size_t i = 0; i < numJoints; i++)
                //{
                //    jointMatrices[i] = getNodeMatrix(skin.joints[i]) * skin.inverseBindMatrices[i];
                //    jointMatrices[i] = inverseTransform * jointMatrices[i];
                //}
                //// Update ssbo
                //skin.ssbo.copyTo(jointMatrices.data(), jointMatrices.size() * sizeof(glm::mat4));
            }


            foreach (var child in node.Children)
            {
                //UpdateJoints(child);
            }
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

            if (gltfNode.Rotation is not null)
            {
                FromFloatArray(ref rotation, gltfNode.Rotation);
            }

            if (gltfNode.Scale is not null)
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

            if (gltfNode.Children is not null)
            {
                node.Children = new();

                for (int i = 0; i < gltfNode.Children.Length; i++)
                {
                    LoadNode(node, gltf.Nodes[gltfNode.Children[i]]);
                }
            }

            if (gltfNode.Mesh is not null)
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
