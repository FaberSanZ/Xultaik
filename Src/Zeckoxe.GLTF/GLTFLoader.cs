// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	GLTFLoader.cs
=============================================================================*/


using glTFLoader;
using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Zeckoxe.Graphics;
using Zeckoxe.Physics;
using Schema = glTFLoader.Schema;


namespace Zeckoxe.GLTF
{
    public class Primitive
    {
        public string name;
        public int indexBase;
        public int vertexBase;
        public int vertexCount;
        public int indexCount;
        public int material;
        public BoundingBox bb;

        public Primitive() { }
        public Primitive(int vertexCount, int indexCount, int vertexBase = 0, int indexBase = 0)
        {
            this.vertexCount = vertexCount;
            this.indexCount = indexCount;
            this.vertexBase = vertexBase;
            this.indexBase = indexBase;
        }

        public void Draw(CommandBuffer cmd, int instanceCount = 1, int firstInstance = 0)
        {
            cmd.DrawIndexed(indexCount, instanceCount, indexBase, vertexBase, firstInstance);
        }
    }



    public static partial class Utils
    {
        public static Stream GetStreamFromPath(string path)
        {
            Stream stream = null;

            if (path.StartsWith("#", StringComparison.Ordinal))
            {
                string resId = path.Substring(1);
                //first search entry assembly
                stream = Assembly.GetEntryAssembly().GetManifestResourceStream(resId);
                if (stream != null)
                    return stream;
                //if not found, search assembly named with the 1st element of the resId
                string assemblyName = resId.Split('.')[0];
                Assembly a = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(aa => aa.GetName().Name == assemblyName);
                if (a == null)
                    throw new Exception($"Assembly '{assemblyName}' not found for ressource '{path}'.");
                stream = a.GetManifestResourceStream(resId);
                if (stream == null)
                    throw new Exception("Resource not found: " + path);
            }
            else
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException("File not found: ", path);
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            return stream;
        }
        /// <summary>Convert angle from degree to radian.</summary>
        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }

        /// <summary>
        /// Populate a Vector3 with values from a float array
        /// </summary>
        public static void FromFloatArray(ref Vector3 v, float[] floats)
        {
            if (floats.Length > 0)
                v.X = floats[0];
            if (floats.Length > 1)
                v.Y = floats[1];
            if (floats.Length > 2)
                v.Z = floats[2];
        }
        /// <summary>
        /// Populate a Vector4 with values from a float array
        /// </summary>
        public static void FromFloatArray(ref Vector4 v, float[] floats)
        {
            if (floats.Length > 0)
                v.X = floats[0];
            if (floats.Length > 1)
                v.Y = floats[1];
            if (floats.Length > 2)
                v.Z = floats[2];
            if (floats.Length > 3)
                v.W = floats[3];
        }
        /// <summary>
        /// Populate a Quaternion with values from a float array
        /// </summary>
        public static void FromFloatArray(ref System.Numerics.Quaternion v, float[] floats)
        {
            if (floats.Length > 0)
                v.X = floats[0];
            if (floats.Length > 1)
                v.Y = floats[1];
            if (floats.Length > 2)
                v.Z = floats[2];
            if (floats.Length > 3)
                v.W = floats[3];
        }
        /// <summary>
        /// Populate a Vector2 with values from a byte array starting at offset
        /// </summary>
        public static void FromByteArray(ref Vector2 v, byte[] byteArray, int offset)
        {
            v.X = BitConverter.ToSingle(byteArray, offset);
            v.Y = BitConverter.ToSingle(byteArray, offset + 4);
        }
        /// <summary>
        /// Populate a Vector3 with values from a byte array starting at offset
        /// </summary>
        public static void FromByteArray(ref Vector3 v, byte[] byteArray, int offset)
        {
            v.X = BitConverter.ToSingle(byteArray, offset);
            v.Y = BitConverter.ToSingle(byteArray, offset + 4);
            v.Z = BitConverter.ToSingle(byteArray, offset + 8);
        }
        /// <summary>
        /// Populate a Vector4 with values from a byte array starting at offset
        /// </summary>
        public static void FromByteArray(ref Vector4 v, byte[] byteArray, int offset)
        {
            v.X = BitConverter.ToSingle(byteArray, offset);
            v.Y = BitConverter.ToSingle(byteArray, offset + 4);
            v.Z = BitConverter.ToSingle(byteArray, offset + 8);
            v.W = BitConverter.ToSingle(byteArray, offset + 12);
        }
        /// <summary>
        /// Populate a Quaternion with values from a byte array starting at offset
        /// </summary>
        public static void FromByteArray(ref System.Numerics.Quaternion v, byte[] byteArray, int offset)
        {
            v.X = BitConverter.ToSingle(byteArray, offset);
            v.Y = BitConverter.ToSingle(byteArray, offset + 4);
            v.Z = BitConverter.ToSingle(byteArray, offset + 8);
            v.W = BitConverter.ToSingle(byteArray, offset + 12);
        }



        public static void ImportFloatArray(this ref Vector3 v, float[] floats)
        {
            if (floats.Length > 0)
                v.X = floats[0];
            if (floats.Length > 1)
                v.Y = floats[1];
            if (floats.Length > 2)
                v.Z = floats[2];
        }
        public static Vector3 Transform(this Vector3 v, ref Matrix4x4 mat, bool translate = false)
        {
            Vector4 v4 = Vector4.Transform(new Vector4(v, translate ? 1f : 0f), mat);
            return new Vector3(v4.X, v4.Y, v4.Z);
        }
        public static Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }

    public class Mesh
    {
        public string Name;
        public List<Primitive> Primitives = new List<Primitive>();
        public BoundingBox bb;

        /// <summary>
        /// add primitive and update mesh bounding box
        /// </summary>
        public void AddPrimitive(Primitive p)
        {
            if (Primitives.Count == 0)
                bb = p.bb;
            else
                bb += p.bb;


            Primitives.Add(p);
        }
    }




    public unsafe class GLTFLoader
    {






        internal Gltf gltf;
        internal string baseDirectory;

        internal byte[][] loadedBuffers;
        internal GCHandle[] bufferHandles;


        internal Device _device;



        public GLTFLoader(Device device, string path)
        {
            _device = device;

            _path = path;

            baseDirectory = Path.GetDirectoryName(path);
            gltf = Interface.LoadModel(path);
            loadedBuffers = new byte[gltf.Buffers.Length][];
            bufferHandles = new GCHandle[gltf.Buffers.Length];


            CreateBuffers();
        }
        public TextureData TextureData { get; set; }


        public Graphics.Buffer VertexBuffer { get; private set; }
        public Graphics.Buffer IndexBuffer { get; private set; }

        public List<Mesh> Meshes { get; set; }

        public IndexType IndexType { get; set; }




        internal void CreateBuffers()
        {
            ulong vertexCount, indexCount;

            GetVertexCount(out vertexCount, out indexCount);

            int vertSize = (int)vertexCount * Marshal.SizeOf<VertexPositionNormal>();
            int idxSize = (int)indexCount * (IndexType == IndexType.Uint16 ? 2 : 4);
            Console.WriteLine(IndexType);

            VertexBuffer = new(_device, new()
            {
                BufferFlags = BufferFlags.VertexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = (int)vertSize,
            });


            IndexBuffer = new(_device, new()
            {
                BufferFlags = BufferFlags.IndexBuffer,
                Usage = GraphicsResourceUsage.Dynamic,
                SizeInBytes = (int)idxSize,
            });



            Meshes = LoadMeshes<VertexPositionNormal>(IndexType, VertexBuffer, 0, IndexBuffer, 0);


        }




        internal byte[] loadDataUri(Schema.Image img)
        {
            int idxComa = img.Uri.IndexOf(",", 5, StringComparison.Ordinal);
            return Convert.FromBase64String(img.Uri.Substring(idxComa + 1));
        }
        internal byte[] loadDataUri(Schema.Buffer buff)
        {
            int idxComa = buff.Uri.IndexOf(",", 5, StringComparison.Ordinal);
            return Convert.FromBase64String(buff.Uri.Substring(idxComa + 1));
        }


        public void GetVertexCount(out ulong vertexCount, out ulong indexCount)
        {
            vertexCount = 0;
            indexCount = 0;
            IndexType = IndexType.Uint16;
            //compute size of stagging buf
            foreach (Schema.Mesh mesh in gltf.Meshes)
            {
                foreach (Schema.MeshPrimitive p in mesh.Primitives)
                {
                    if (p.Attributes.TryGetValue("POSITION", out int accessorIdx))
                    {
                        vertexCount += (ulong)gltf.Accessors[accessorIdx].Count;
                    }

                    if (p.Indices != null)
                    {
                        indexCount += (ulong)gltf.Accessors[(int)p.Indices].Count;
                        if (gltf.Accessors[(int)p.Indices].ComponentType == Accessor.ComponentTypeEnum.UNSIGNED_INT)
                        {
                            IndexType = IndexType.Uint32;
                        }
                    }
                }
            }
        }

        public uint ImageCount => gltf.Images == null ? 0 : (uint)gltf.Images.Length;

        private readonly string _path;

        private void ensureBufferIsLoaded(int bufferIdx)
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
                    loadedBuffers[bufferIdx] = loadDataUri(gltf.Buffers[bufferIdx]);//TODO:check this func=>System.Buffers.Text.Base64.EncodeToUtf8InPlace
                }
                else
                {
                    loadedBuffers[bufferIdx] = File.ReadAllBytes(Path.Combine(baseDirectory, gltf.Buffers[bufferIdx].Uri));
                }

                bufferHandles[bufferIdx] = GCHandle.Alloc(loadedBuffers[bufferIdx], GCHandleType.Pinned);
            }
        }







        List<Mesh> meshes;


        public List<Mesh> LoadMeshes<TVertex>(IndexType indexType, Graphics.Buffer vbo, ulong vboOffset, Graphics.Buffer ibo, ulong iboOffset)
        {
            ulong vCount, iCount;

            GetVertexCount(out vCount, out iCount);

            int vertexByteSize = Marshal.SizeOf<TVertex>();
            ulong vertSize = vCount * (ulong)vertexByteSize;
            ulong idxSize = iCount * (indexType == IndexType.Uint16 ? 2ul : 4ul);
            ulong size = vertSize + idxSize;

            int vertexCount = 0, indexCount = 0;
            int autoNamedMesh = 1;

            meshes = new List<Mesh>();


            vbo.Map();
            ibo.Map();

            {

                unsafe
                {
                    byte* stagVertPtrInit = (byte*)vbo.MappedData.ToPointer();
                    byte* stagIdxPtrInit = (byte*)(ibo.MappedData.ToPointer());
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
                            Schema.Accessor AccPos = null, AccNorm = null, AccUv = null, AccUv1 = null;

                            int accessorIdx;
                            if (p.Attributes.TryGetValue("POSITION", out accessorIdx))
                            {
                                AccPos = gltf.Accessors[accessorIdx];
                                ensureBufferIsLoaded(gltf.BufferViews[(int)AccPos.BufferView].Buffer);
                            }
                            if (p.Attributes.TryGetValue("NORMAL", out accessorIdx))
                            {
                                AccNorm = gltf.Accessors[accessorIdx];
                                ensureBufferIsLoaded(gltf.BufferViews[(int)AccNorm.BufferView].Buffer);
                            }
                            if (p.Attributes.TryGetValue("TEXCOORD_0", out accessorIdx))
                            {
                                AccUv = gltf.Accessors[accessorIdx];
                                ensureBufferIsLoaded(gltf.BufferViews[(int)AccUv.BufferView].Buffer);
                            }
                            if (p.Attributes.TryGetValue("TEXCOORD_1", out accessorIdx))
                            {
                                AccUv1 = gltf.Accessors[accessorIdx];
                                ensureBufferIsLoaded(gltf.BufferViews[(int)AccUv1.BufferView].Buffer);
                            }

                            Primitive prim = new Primitive
                            {
                                indexBase = indexCount,
                                vertexBase = vertexCount,
                                vertexCount = AccPos.Count,
                                material = (p.Material ?? 0)
                            };

                            prim.bb.Min.ImportFloatArray(AccPos.Min);
                            prim.bb.Max.ImportFloatArray(AccPos.Max);
                            prim.bb.IsValid = true;

                            //Interleaving vertices
                            byte* inPosPtr = null, inNormPtr = null, inUvPtr = null, inUv1Ptr = null;

                            Schema.BufferView bv = gltf.BufferViews[(int)AccPos.BufferView];
                            inPosPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                            inPosPtr += AccPos.ByteOffset + bv.ByteOffset;

                            if (AccNorm != null)
                            {
                                bv = gltf.BufferViews[(int)AccNorm.BufferView];
                                inNormPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                                inNormPtr += AccNorm.ByteOffset + bv.ByteOffset;
                            }
                            if (AccUv != null)
                            {
                                bv = gltf.BufferViews[(int)AccUv.BufferView];
                                inUvPtr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                                inUvPtr += AccUv.ByteOffset + bv.ByteOffset;
                            }
                            if (AccUv1 != null)
                            {
                                bv = gltf.BufferViews[(int)AccUv1.BufferView];
                                inUv1Ptr = (byte*)bufferHandles[bv.Buffer].AddrOfPinnedObject().ToPointer();
                                inUv1Ptr += AccUv1.ByteOffset + bv.ByteOffset;
                            }

                            //TODO: use vertex attributes scan for copying data if they exists
                            for (int j = 0; j < prim.vertexCount; j++)
                            {
                                System.Buffer.MemoryCopy(inPosPtr, stagVertPtr, 12, 12);
                                inPosPtr += 12;
                                if (inNormPtr != null)
                                {
                                    System.Buffer.MemoryCopy(inNormPtr, stagVertPtr + 12, 12, 12);
                                    inNormPtr += 12;
                                }
                                if (inUvPtr != null)
                                {
                                    System.Buffer.MemoryCopy(inUvPtr, stagVertPtr + 24, 8, 8);
                                    inUvPtr += 8;
                                }
                                if (inUv1Ptr != null)
                                {
                                    System.Buffer.MemoryCopy(inUv1Ptr, stagVertPtr + 32, 8, 8);
                                    inUv1Ptr += 8;
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
                                if (acc.ComponentType == Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT)
                                {
                                    if (indexType == IndexType.Uint16)
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
                                else if (acc.ComponentType == Schema.Accessor.ComponentTypeEnum.UNSIGNED_INT)
                                {
                                    if (indexType == IndexType.Uint32)
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
                                else if (acc.ComponentType == Schema.Accessor.ComponentTypeEnum.UNSIGNED_BYTE)
                                {
                                    //convert
                                    if (indexType == IndexType.Uint16)
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

                                prim.indexCount = acc.Count;
                                indexCount += acc.Count;
                            }

                            m.AddPrimitive(prim);

                            vertexCount += AccPos.Count;
                        }
                        meshes.Add(m);
                    }
                }

                vbo.Unmap();
                ibo.Unmap();

                //CommandBuffer cmd = new CommandBuffer(_device, CommandBufferType.AsyncTransfer);
                //cmd.Start();

                //stagging.CopyTo(cmd, vbo, vertSize, 0, vboOffset);
                //if (iCount > 0)
                //    stagging.CopyTo(cmd, ibo, idxSize, vertSize, iboOffset);

                //cmd.End();

                //transferQ.Submit(cmd);

                //dev.WaitIdle();
                //cmd.Free();

            }

            return meshes;
        }





        public void Draw(CommandBuffer commandBuffer)
        {
            commandBuffer.SetVertexBuffers(new[] { VertexBuffer });
            commandBuffer.SetIndexBuffer(IndexBuffer, 0, IndexType);


            //commandBuffer.BindDescriptorSets(new DescriptorSet);

            foreach (Mesh m in Meshes)
            {
                foreach (var p in m.Primitives)
                {
                    commandBuffer.DrawIndexed(p.indexCount, 1, p.indexBase, p.vertexBase, 0);
                }
            }
        }



        public Scene[] LoadScenes(out int defaultScene)
        {
            defaultScene = -1;
            if (gltf.Scene == null)
                return new Scene[] { };

            List<Scene> scenes = new List<Scene>();
            defaultScene = (int)gltf.Scene;

            for (int i = 0; i < gltf.Scenes.Length; i++)
            {
                Schema.Scene scene = gltf.Scenes[i];
                //Debug.WriteLine("Loading Scene {0}", scene.Name);

                scenes.Add(new Scene
                {
                    Name = scene.Name,
                });

                if (scene.Nodes.Length == 0)
                    continue;

                scenes[i].Root = new Node
                {
                    LocalMatrix = Matrix4x4.Identity,
                    Children = new List<Node>()
                };

                foreach (int nodeIdx in scene.Nodes)
                    LoadNode(scenes[i].Root, gltf.Nodes[nodeIdx]);
            }
            return scenes.ToArray();
        }




        public void LoadNode(Node parentNode, Schema.Node gltfNode)
        {
            //Debug.WriteLine("Loading node {0}", gltfNode.Name);

            Vector3 translation = new Vector3();
            System.Numerics.Quaternion rotation = System.Numerics.Quaternion.Identity;
            Vector3 scale = new Vector3(1);
            Matrix4x4 localTransform = Matrix4x4.Identity;

            if (gltfNode.Matrix != null)
            {
                float[] M = gltfNode.Matrix;
                localTransform = new Matrix4x4(
                    M[0], M[1], M[2], M[3],
                    M[4], M[5], M[6], M[7],
                    M[8], M[9], M[10], M[11],
                   M[12], M[13], M[14], M[15]);
            }

            if (gltfNode.Translation != null)
                FromFloatArray(ref translation, gltfNode.Translation);
            if (gltfNode.Translation != null)
                FromFloatArray(ref rotation, gltfNode.Rotation);
            if (gltfNode.Translation != null)
                FromFloatArray(ref scale, gltfNode.Scale);

            localTransform *=
                Matrix4x4.CreateScale(scale) *
                Matrix4x4.CreateFromQuaternion(rotation) *
                Matrix4x4.CreateTranslation(translation);

            //localTransform = Matrix4x4.Identity;

            Node node = new Node
            {
                LocalMatrix = localTransform,
                Parent = parentNode,
                Name = gltfNode.Name
            };
            parentNode.Children.Add(node);

            if (gltfNode.Children != null)
            {
                node.Children = new List<Node>();
                for (int i = 0; i < gltfNode.Children.Length; i++)
                    LoadNode(node, gltf.Nodes[gltfNode.Children[i]]);
            }

            if (gltfNode.Mesh != null)
                node.Mesh = meshes[(int)gltfNode.Mesh];
        }

        public static void FromFloatArray(ref Vector3 v, float[] floats)
        {
            if (floats.Length > 0)
                v.X = floats[0];
            if (floats.Length > 1)
                v.Y = floats[1];
            if (floats.Length > 2)
                v.Z = floats[2];
        }


        public static void FromFloatArray(ref System.Numerics.Quaternion v, float[] floats)
        {
            if (floats.Length > 0)
                v.X = floats[0];
            if (floats.Length > 1)
                v.Y = floats[1];
            if (floats.Length > 2)
                v.Z = floats[2];
            if (floats.Length > 3)
                v.W = floats[3];
        }




       


        public void Update()
        {

        }



    }
}
