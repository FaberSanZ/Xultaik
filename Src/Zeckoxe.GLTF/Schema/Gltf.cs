
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
    public class Gltf
    {
        private string[] _extensionsUsed;
        private string[] _extensionsRequired;
        private Accessor[] _accessors;
        private Animation[] _animations;
        private Asset _asset;
        private Buffer[] _buffers;
        private BufferView[] _bufferViews;
        private Camera[] _cameras;
        private Image[] _images;
        private Material[] _materials;
        private Mesh[] _meshes;
        private Node[] _nodes;
        private Sampler[] _samplers;
        private int? _scene;
        private GltfLoader.Schema.Scene[] _scenes;
        private Skin[] _skins;
        private Texture[] _textures;
        private Dictionary<string, object> _extensions;
        private Extras _extras;

        [JsonPropertyName("extensionsUsed")]
        public string[] ExtensionsUsed
        {
            get => this._extensionsUsed;
            set
            {
                if (value == null)
                {
                    this._extensionsUsed = value;
                }
                else
                {
                    this._extensionsUsed = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("extensionsRequired")]
        public string[] ExtensionsRequired
        {
            get => this._extensionsRequired;
            set
            {
                if (value == null)
                {
                    this._extensionsRequired = value;
                }
                else
                {
                    this._extensionsRequired = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("accessors")]
        public Accessor[] Accessors
        {
            get => this._accessors;
            set
            {
                if (value == null)
                {
                    this._accessors = value;
                }
                else
                {
                    this._accessors = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("animations")]
        public Animation[] Animations
        {
            get => this._animations;
            set
            {
                if (value == null)
                {
                    this._animations = value;
                }
                else
                {
                    this._animations = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("asset")]
        public Asset Asset
        {
            get => this._asset;
            set => this._asset = value;
        }

        [JsonPropertyName("buffers")]
        public Buffer[] Buffers
        {
            get => this._buffers;
            set
            {
                if (value == null)
                {
                    this._buffers = value;
                }
                else
                {
                    this._buffers = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("bufferViews")]
        public BufferView[] BufferViews
        {
            get => this._bufferViews;
            set
            {
                if (value == null)
                {
                    this._bufferViews = value;
                }
                else
                {
                    this._bufferViews = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("cameras")]
        public Camera[] Cameras
        {
            get => this._cameras;
            set
            {
                if (value == null)
                {
                    this._cameras = value;
                }
                else
                {
                    this._cameras = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("images")]
        public Image[] Images
        {
            get => this._images;
            set
            {
                if (value == null)
                {
                    this._images = value;
                }
                else
                {
                    this._images = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("materials")]
        public Material[] Materials
        {
            get => this._materials;
            set
            {
                if (value == null)
                {
                    this._materials = value;
                }
                else
                {
                    this._materials = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("meshes")]
        public Mesh[] Meshes
        {
            get => this._meshes;
            set
            {
                if (value == null)
                {
                    this._meshes = value;
                }
                else
                {
                    this._meshes = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("nodes")]
        public Node[] Nodes
        {
            get => this._nodes;
            set
            {
                if (value == null)
                {
                    this._nodes = value;
                }
                else
                {
                    this._nodes = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("samplers")]
        public Sampler[] Samplers
        {
            get => this._samplers;
            set
            {
                if (value == null)
                {
                    this._samplers = value;
                }
                else
                {
                    this._samplers = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("scene")]
        public int? Scene
        {
            get => this._scene;
            set
            {
                int? nullable1 = value;
                float? nullable2 = nullable1.HasValue ? new float?(nullable1.GetValueOrDefault()) : new float?();
                float num = 0.0f;
                if (nullable2.GetValueOrDefault() < (double)num & nullable2.HasValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(Scene), value, "Expected value to be greater than or equal to 0");
                }

                this._scene = value;
            }
        }

        [JsonPropertyName("scenes")]
        public GltfLoader.Schema.Scene[] Scenes
        {
            get => this._scenes;
            set
            {
                if (value == null)
                {
                    this._scenes = value;
                }
                else
                {
                    this._scenes = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("skins")]
        public Skin[] Skins
        {
            get => this._skins;
            set
            {
                if (value == null)
                {
                    this._skins = value;
                }
                else
                {
                    this._skins = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("textures")]
        public Texture[] Textures
        {
            get => this._textures;
            set
            {
                if (value == null)
                {
                    this._textures = value;
                }
                else
                {
                    this._textures = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
                }
            }
        }

        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions
        {
            get => this._extensions;
            set => this._extensions = value;
        }

        [JsonPropertyName("extras")]
        public Extras Extras
        {
            get => this._extras;
            set => this._extras = value;
        }

        public bool ShouldSerializeExtensionsUsed()
        {
            return this._extensionsUsed != null;
        }

        public bool ShouldSerializeExtensionsRequired()
        {
            return this._extensionsRequired != null;
        }

        public bool ShouldSerializeAccessors()
        {
            return this._accessors != null;
        }

        public bool ShouldSerializeAnimations()
        {
            return this._animations != null;
        }

        public bool ShouldSerializeAsset()
        {
            return this._asset != null;
        }

        public bool ShouldSerializeBuffers()
        {
            return this._buffers != null;
        }

        public bool ShouldSerializeBufferViews()
        {
            return this._bufferViews != null;
        }

        public bool ShouldSerializeCameras()
        {
            return this._cameras != null;
        }

        public bool ShouldSerializeImages()
        {
            return this._images != null;
        }

        public bool ShouldSerializeMaterials()
        {
            return this._materials != null;
        }

        public bool ShouldSerializeMeshes()
        {
            return this._meshes != null;
        }

        public bool ShouldSerializeNodes()
        {
            return this._nodes != null;
        }

        public bool ShouldSerializeSamplers()
        {
            return this._samplers != null;
        }

        public bool ShouldSerializeScene()
        {
            return this._scene.HasValue;
        }

        public bool ShouldSerializeScenes()
        {
            return this._scenes != null;
        }

        public bool ShouldSerializeSkins()
        {
            return this._skins != null;
        }

        public bool ShouldSerializeTextures()
        {
            return this._textures != null;
        }

        public bool ShouldSerializeExtensions()
        {
            return this._extensions != null;
        }

        public bool ShouldSerializeExtras()
        {
            return this._extras != null;
        }
    }
}
