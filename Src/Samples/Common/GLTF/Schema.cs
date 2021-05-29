// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vultaik.GLTF.Schema
{
    public enum GltfType
    {
        Scalar,

        Vec2,

        Vec3,

        Vec4,

        Mat2,

        Mat3,

        Mat4,
    }
    public enum GltfComponentType
    {
        Byte = 0x00001400,

        UnsignedByte = 0x00001401,

        Short = 0x00001402,

        UnsignedShort = 0x00001403,

        UnsignedInt = 0x00001405,

        Float = 0x00001406,
    }

    public class Gltf
    {

        [JsonPropertyName("extensionsUsed")]
        public string[] ExtensionsUsed { get; set; }



        [JsonPropertyName("extensionsRequired")]
        public string[] ExtensionsRequired { get; set; }



        [JsonPropertyName("accessors")]
        public Accessor[] Accessors { get; set; }



        [JsonPropertyName("animations")]
        public Animation[] Animations { get; set; }



        [JsonPropertyName("asset")]
        public Asset Asset { get; set; }



        [JsonPropertyName("buffers")]
        public Buffer[] Buffers { get; set; }



        [JsonPropertyName("bufferViews")]
        public BufferView[] BufferViews { get; set; }



        [JsonPropertyName("cameras")]
        public Camera[] Cameras { get; set; }



        [JsonPropertyName("images")]
        public Image[] Images { get; set; }



        [JsonPropertyName("materials")]
        public Material[] Materials { get; set; }



        [JsonPropertyName("meshes")]
        public Mesh[] Meshes { get; set; }



        [JsonPropertyName("nodes")]
        public Node[] Nodes { get; set; }



        [JsonPropertyName("samplers")]
        public Sampler[] Samplers { get; set; }



        [JsonPropertyName("scene")]
        public int? Scene { get; set; }



        [JsonPropertyName("scenes")]
        public Schema.Scene[] Scenes { get; set; }



        [JsonPropertyName("skins")]
        public Skin[] Skins { get; set; }



        [JsonPropertyName("textures")]
        public Texture[] Textures { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }


    }
    public class Accessor
    {
        [JsonPropertyName("bufferView")]
        public int? BufferView { get; set; }


        [JsonPropertyName("byteOffset")]
        public int ByteOffset { get; set; }


        [JsonPropertyName("componentType")]
        public GltfComponentType ComponentType { get; set; }


        [JsonPropertyName("normalized")]
        public bool Normalized { get; set; }


        [JsonPropertyName("count")]
        public int Count { get; set; }


        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("type")]
        public GltfType Type { get; set; }


        [JsonPropertyName("max")]
        public float[] Max { get; set; }


        [JsonPropertyName("min")]
        public float[] Min { get; set; }


        [JsonPropertyName("sparse")]
        public AccessorSparse Sparse { get; set; }


        [JsonPropertyName("name")]
        public string Name { get; set; }


        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }


        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }
    }


    public class Mesh
    {
        [JsonPropertyName("primitives")]
        public MeshPrimitive[] Primitives { get; set; }



        [JsonPropertyName("weights")]
        public float[] Weights { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }


    }

    public class AccessorSparse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }



        [JsonPropertyName("indices")]
        public AccessorSparseIndices Indices { get; set; }



        [JsonPropertyName("values")]
        public AccessorSparseValues Values { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    
    public class AccessorSparseIndices
    {

        [JsonPropertyName("bufferView")]
        public int BufferView { get; set; }



        [JsonPropertyName("byteOffset")]
        public int ByteOffset { get; set; }



        [JsonPropertyName("componentType")]
        public AccessorSparseIndices.GltfComponentType ComponentType { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }




        public enum GltfComponentType
        {
            UnsignedByte = 0x00001401,

            UnsignedShort = 0x00001403,

            UnsignedInt = 0x00001405,
        }
    }

    public class AccessorSparseValues
    {
        [JsonPropertyName("bufferView")]
        public int BufferView { get; set; }



        [JsonPropertyName("byteOffset")]
        public int ByteOffset { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }


    }

    public class Animation
    {
        [JsonPropertyName("channels")]
        public AnimationChannel[] Channels { get; set; }



        [JsonPropertyName("samplers")]
        public AnimationSampler[] Samplers { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    public class AnimationChannel
    {
        [JsonPropertyName("sampler")]
        public int Sampler { get; set; }



        [JsonPropertyName("target")]
        public AnimationChannelTarget Target { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    public class AnimationChannelTarget
    {
        [JsonPropertyName("node")]
        public int? Node { get; set; }



        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("path")]
        public AnimationChannelTarget.GltfPath Path { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }



        public enum GltfPath
        {
            Translation,
            Rotation,
            Scale,
            Weights,
        }
    }

    public class AnimationSampler
    {

        [JsonPropertyName("input")]
        public int Input { get; set; }



        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("interpolation")]
        public AnimationSampler.GltfInterpolation Interpolation { get; set; }



        [JsonPropertyName("output")]
        public int Output { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

        public enum GltfInterpolation
        {
            Linear,
            Step,
            Cubicspline,
        }
    }

    public class Asset
    {

        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }



        [JsonPropertyName("generator")]
        public string Generator { get; set; }



        [JsonPropertyName("version")]
        public string Version { get; set; }



        [JsonPropertyName("minVersion")]
        public string MinVersion { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }
    }

    public class Buffer
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }



        [JsonPropertyName("byteLength")]
        public int ByteLength { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }


    }

    public class Texture
    {

        [JsonPropertyName("sampler")]
        public int? Sampler { get; set; }



        [JsonPropertyName("source")]
        public int? Source { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }


    }

    public class Skin
    {

        [JsonPropertyName("inverseBindMatrices")]
        public int? InverseBindMatrices { get; set; }



        [JsonPropertyName("skeleton")]
        public int? Skeleton { get; set; }



        [JsonPropertyName("joints")]
        public int[] Joints { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    public class BufferView
    {

        [JsonPropertyName("buffer")]
        public int Buffer { get; set; }



        [JsonPropertyName("byteOffset")]
        public int ByteOffset { get; set; }



        [JsonPropertyName("byteLength")]
        public int ByteLength { get; set; }



        [JsonPropertyName("byteStride")]
        public int? ByteStride { get; set; }



        [JsonPropertyName("target")]
        public BufferView.GltfTarget? Target { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }



        public enum GltfTarget
        {
            ArrayBuffer = 0x00008892,

            ElementArrayBuffer = 0x00008893
        }
    }


    public class Camera
    {

        [JsonPropertyName("orthographic")]
        public CameraOrthographic Orthographic { get; set; }



        [JsonPropertyName("perspective")]
        public CameraPerspective Perspective { get; set; }



        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("type")]
        public Camera.GltfType Type { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

        //TODO : Camera Type
        public enum GltfType
        {
            Perspective,

            Orthographic,
        }
    }


    public class CameraOrthographic
    {

        [JsonPropertyName("xmag")]
        public float Xmag { get; set; }



        [JsonPropertyName("ymag")]
        public float Ymag { get; set; }



        [JsonPropertyName("zfar")]
        public float Zfar { get; set; }



        [JsonPropertyName("znear")]
        public float Znear { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }


    public class CameraPerspective
    {

        [JsonPropertyName("aspectRatio")]
        public float? AspectRatio { get; set; }



        [JsonPropertyName("yfov")]
        public float Yfov { get; set; }



        [JsonPropertyName("zfar")]
        public float? Zfar { get; set; }



        [JsonPropertyName("znear")]
        public float Znear { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    public class GltfChildOfRootProperty
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    public class GltfProperty
    {

        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    public class Scene
    {

        [JsonPropertyName("nodes")]
        public int[] Nodes { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }
    }

    public class Image
    {

        [JsonPropertyName("uri")]
        public string Uri { get; set; }



        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }



        [JsonPropertyName("bufferView")]
        public int? BufferView { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    public class Material
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }



        [JsonPropertyName("pbrMetallicRoughness")]
        public MaterialPbrMetallicRoughness PbrMetallicRoughness { get; set; }



        [JsonPropertyName("normalTexture")]
        public MaterialNormalTextureInfo NormalTexture { get; set; }



        [JsonPropertyName("occlusionTexture")]
        public MaterialOcclusionTextureInfo OcclusionTexture { get; set; }



        [JsonPropertyName("emissiveTexture")]
        public TextureInfo EmissiveTexture { get; set; }



        [JsonPropertyName("emissiveFactor")]
        public float[] EmissiveFactor { get; set; }



        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("alphaMode")]
        public Material.GltfAlphaMode AlphaMode { get; set; }



        [JsonPropertyName("alphaCutoff")]
        public float AlphaCutoff { get; set; }



        [JsonPropertyName("doubleSided")]
        public bool DoubleSided { get; set; }



        public enum GltfAlphaMode
        {
            Opaque,

            Mask,

            Blend,
        }
    }

    public class MaterialNormalTextureInfo
    {

        [JsonPropertyName("index")]
        public int Index { get; set; }



        [JsonPropertyName("texCoord")]
        public int TexCoord { get; set; }



        [JsonPropertyName("scale")]
        public float Scale { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }


    }

    public class MaterialOcclusionTextureInfo
    {

        [JsonPropertyName("index")]
        public int Index { get; set; }



        [JsonPropertyName("texCoord")]
        public int TexCoord { get; set; }



        [JsonPropertyName("strength")]
        public float Strength { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }


    public class MaterialPbrMetallicRoughness
    {
        private float[] _baseColorFactor = new[]
        {
            1f,1f,1f,1f
        };


        [JsonPropertyName("baseColorFactor")]
        public float[] BaseColorFactor
        {
            get => _baseColorFactor;
            set
            {
                if (value is null)
                    _baseColorFactor = value;
                else
                {
                    if (value.Length < 4)
                        throw new ArgumentException("Array not long enough");

                    if (value.Length > 4)
                        throw new ArgumentException("Array too long");

                    for (int index = 0; index < value.Length; ++index)
                        if (value[index] < 0.0)
                            throw new ArgumentOutOfRangeException();

                    for (int index = 0; index < value.Length; ++index)
                        if (value[index] > 1.0)
                            throw new ArgumentOutOfRangeException();

                    _baseColorFactor = value;
                }
            }
        }



        [JsonPropertyName("baseColorTexture")]
        public TextureInfo BaseColorTexture { get; set; }



        [JsonPropertyName("metallicFactor")]
        public float MetallicFactor { get; set; }



        [JsonPropertyName("roughnessFactor")]
        public float RoughnessFactor { get; set; }



        [JsonPropertyName("metallicRoughnessTexture")]
        public TextureInfo MetallicRoughnessTexture { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    public class MeshPrimitive
    {

        [JsonPropertyName("attributes")]
        public Dictionary<string, int> Attributes { get; set; }



        [JsonPropertyName("indices")]
        public int? Indices { get; set; }



        [JsonPropertyName("material")]
        public int? Material { get; set; }



        [JsonPropertyName("mode")]
        public MeshPrimitive.GltfMode Mode { get; set; }



        [JsonPropertyName("targets")]
        public Dictionary<string, int>[] Targets { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }




        // TODO: MeshPrimitive GltfMode
        public enum GltfMode
        {
            Points,
            Lines,
            LineLoop,
            LineStrip,
            Triangles,
            TriangleStrip,
            TriangleFan,
        }
    }

    public class Node
    {
        private float[] _matrix = new float[16]
        {

            1f, 0.0f, 0.0f, 0.0f,

            0.0f, 1f, 0.0f, 0.0f,

            0.0f, 0.0f, 1f, 0.0f,

            0.0f, 0.0f, 0.0f, 1f
        };

        private float[] _rotation = new float[4] { 0.0f, 0.0f, 0.0f, 1f };
        private float[] _scale = new float[3] { 1f, 1f, 1f };
        private float[] _translation = new float[3];





        [JsonPropertyName("camera")]
        public int? Camera { get; set; }



        [JsonPropertyName("children")]
        public int[] Children { get; set; }



        [JsonPropertyName("skin")]
        public int? Skin { get; set; }



        [JsonPropertyName("matrix")]
        public float[] Matrix
        {
            get => _matrix;
            set
            {
                if (value is null)
                    _matrix = value;
                else
                {
                    if (value.Length < 16)
                        throw new ArgumentException("Array not long enough");

                    _matrix = value.Length <= 16 ? value : throw new ArgumentException("Array too long");
                }
            }
        }

        [JsonPropertyName("mesh")]
        public int? Mesh { get; set; }



        [JsonPropertyName("rotation")]
        public float[] Rotation
        {
            get => _rotation;
            set
            {
                if (value is null)
                    _rotation = value;
                else
                {
                    if (value.Length < 4)
                        throw new ArgumentException("Array not long enough");

                    if (value.Length > 4)
                        throw new ArgumentException("Array too long");

                    for (int index = 0; index < value.Length; ++index)
                        if (value[index] < -1.0)
                            throw new ArgumentOutOfRangeException();

                    for (int index = 0; index < value.Length; ++index)
                        if (value[index] > 1.0)
                            throw new ArgumentOutOfRangeException();

                    _rotation = value;
                }
            }
        }



        [JsonPropertyName("scale")]
        public float[] Scale
        {
            get => _scale;
            set
            {
                if (value is null)
                    _scale = value;
                else
                {
                    if (value.Length < 3)
                        throw new ArgumentException("Array not long enough");

                    _scale = value.Length <= 3 ? value : throw new ArgumentException("Array too long");
                }
            }
        }



        [JsonPropertyName("translation")]
        public float[] Translation
        {
            get => _translation;
            set
            {
                if (value is null)
                    _translation = value;
                else
                {
                    if (value.Length < 3)
                        throw new ArgumentException("Array not long enough");

                    _translation = value.Length <= 3 ? value : throw new ArgumentException("Array too long");
                }
            }
        }



        [JsonPropertyName("weights")]
        public float[] Weights { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }

    public class Sampler
    {

        [JsonPropertyName("magFilter")]
        public Sampler.GltfMagFilter? MagFilter { get; set; }



        [JsonPropertyName("minFilter")]
        public Sampler.GltfMinFilter? MinFilter { get; set; }



        [JsonPropertyName("wrapS")]
        public Sampler.GltfWrapS WrapS { get; set; }



        [JsonPropertyName("wrapT")]
        public Sampler.GltfWrapT WrapT { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }


        public enum GltfMagFilter
        {
            Nearest = 0x00002600,

            Linear = 0x00002601
        }

        public enum GltfMinFilter
        {
            Nearest = 0x00002600,

            Linear = 0x00002601,

            NearestMipmapNearest = 0x00002700,

            LinearMipmapNearest = 0x00002701,

            NearestMipmapLinear = 0x00002702,

            LinearMipmapLinear = 0x00002703,
        }

        public enum GltfWrapS
        {
            Repeat = 0x00002901,

            ClampToEdge = 0x0000812F,

            MirroredRepeat = 0x00008370
        }

        public enum GltfWrapT
        {
            Repeat = 0x00002901,

            ClampToEdge = 0x0000812F,

            MirroredRepeat = 0x00008370
        }
    }

    public class TextureInfo
    {

        [JsonPropertyName("index")]
        public int Index { get; set; }



        [JsonPropertyName("texCoord")]
        public int TexCoord { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

    }
}
