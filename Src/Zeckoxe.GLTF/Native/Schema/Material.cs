
namespace glTFLoader.Schema
{
    using System.Linq;


    public class Material
    {

        /// <summary>
        /// Backing field for Name.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Backing field for Extensions.
        /// </summary>
        private System.Collections.Generic.Dictionary<string, object> m_extensions;

        /// <summary>
        /// Backing field for Extras.
        /// </summary>
        private Extras m_extras;

        /// <summary>
        /// Backing field for PbrMetallicRoughness.
        /// </summary>
        private MaterialPbrMetallicRoughness m_pbrMetallicRoughness;

        /// <summary>
        /// Backing field for NormalTexture.
        /// </summary>
        private MaterialNormalTextureInfo m_normalTexture;

        /// <summary>
        /// Backing field for OcclusionTexture.
        /// </summary>
        private MaterialOcclusionTextureInfo m_occlusionTexture;

        /// <summary>
        /// Backing field for EmissiveTexture.
        /// </summary>
        private TextureInfo m_emissiveTexture;

        /// <summary>
        /// Backing field for EmissiveFactor.
        /// </summary>
        private float[] m_emissiveFactor = new[] {
                0F,
                0F,
                0F};

        /// <summary>
        /// Backing field for AlphaMode.
        /// </summary>
        private AlphaModeEnum m_alphaMode = AlphaModeEnum.OPAQUE;

        /// <summary>
        /// Backing field for AlphaCutoff.
        /// </summary>
        private float m_alphaCutoff = 0.5F;

        /// <summary>
        /// Backing field for DoubleSided.
        /// </summary>
        private bool m_doubleSided = false;

        /// <summary>
        /// The user-defined name of this object.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("name")]
        public string Name {
            get => this.m_name;
            set => this.m_name = value;
        }

        /// <summary>
        /// Dictionary object with extension-specific objects.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("extensions")]
        public System.Collections.Generic.Dictionary<string, object> Extensions {
            get => this.m_extensions;
            set => this.m_extensions = value;
        }

        /// <summary>
        /// Application-specific data.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("extras")]
        public Extras Extras {
            get => this.m_extras;
            set => this.m_extras = value;
        }

        /// <summary>
        /// A set of parameter values that are used to define the metallic-roughness material model from Physically-Based Rendering (PBR) methodology. When not specified, all the default values of `pbrMetallicRoughness` apply.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("pbrMetallicRoughness")]
        public MaterialPbrMetallicRoughness PbrMetallicRoughness {
            get => this.m_pbrMetallicRoughness;
            set => this.m_pbrMetallicRoughness = value;
        }

        /// <summary>
        /// The normal map texture.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("normalTexture")]
        public MaterialNormalTextureInfo NormalTexture {
            get => this.m_normalTexture;
            set => this.m_normalTexture = value;
        }

        /// <summary>
        /// The occlusion map texture.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("occlusionTexture")]
        public MaterialOcclusionTextureInfo OcclusionTexture {
            get => this.m_occlusionTexture;
            set => this.m_occlusionTexture = value;
        }

        /// <summary>
        /// The emissive map texture.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("emissiveTexture")]
        public TextureInfo EmissiveTexture {
            get => this.m_emissiveTexture;
            set => this.m_emissiveTexture = value;
        }

        /// <summary>
        /// The emissive color of the material.
        /// </summary>
        [Newtonsoft.Json.JsonConverterAttribute(typeof(glTFLoader.Shared.ArrayConverter))]
        [Newtonsoft.Json.JsonPropertyAttribute("emissiveFactor")]
        public float[] EmissiveFactor
        {
            get => this.m_emissiveFactor;
            set
            {
                if ((value.Length < 3u))
                {
                    throw new System.ArgumentException("Array not long enough");
                }
                if ((value.Length > 3u))
                {
                    throw new System.ArgumentException("Array too long");
                }
                int index = 0;
                for (index = 0; (index < value.Length); index = (index + 1))
                {
                    if ((value[index] < 0D))
                    {
                        throw new System.ArgumentOutOfRangeException();
                    }
                }
                for (index = 0; (index < value.Length); index = (index + 1))
                {
                    if ((value[index] > 1D))
                    {
                        throw new System.ArgumentOutOfRangeException();
                    }
                }
                this.m_emissiveFactor = value;
            }
        }

        /// <summary>
        /// The alpha rendering mode of the material.
        /// </summary>
        [Newtonsoft.Json.JsonConverterAttribute(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [Newtonsoft.Json.JsonPropertyAttribute("alphaMode")]
        public AlphaModeEnum AlphaMode {
            get => this.m_alphaMode;
            set => this.m_alphaMode = value;
        }

        /// <summary>
        /// The alpha cutoff value of the material.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("alphaCutoff")]
        public float AlphaCutoff
        {
            get => this.m_alphaCutoff;
            set
            {
                if ((value < 0D))
                {
                    throw new System.ArgumentOutOfRangeException("AlphaCutoff", value, "Expected value to be greater than or equal to 0");
                }
                this.m_alphaCutoff = value;
            }
        }

        /// <summary>
        /// Specifies whether the material is double sided.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("doubleSided")]
        public bool DoubleSided {
            get => this.m_doubleSided;
            set => this.m_doubleSided = value;
        }

        public bool ShouldSerializeName()
        {
            return ((m_name == null)
                        == false);
        }

        public bool ShouldSerializeExtensions()
        {
            return ((m_extensions == null)
                        == false);
        }

        public bool ShouldSerializeExtras()
        {
            return ((m_extras == null)
                        == false);
        }

        public bool ShouldSerializePbrMetallicRoughness()
        {
            return ((m_pbrMetallicRoughness == null)
                        == false);
        }

        public bool ShouldSerializeNormalTexture()
        {
            return ((m_normalTexture == null)
                        == false);
        }

        public bool ShouldSerializeOcclusionTexture()
        {
            return ((m_occlusionTexture == null)
                        == false);
        }

        public bool ShouldSerializeEmissiveTexture()
        {
            return ((m_emissiveTexture == null)
                        == false);
        }

        public bool ShouldSerializeEmissiveFactor()
        {
            return (m_emissiveFactor.SequenceEqual(new float[] {
                        0F,
                        0F,
                        0F}) == false);
        }

        public bool ShouldSerializeAlphaMode()
        {
            return ((m_alphaMode == AlphaModeEnum.OPAQUE)
                        == false);
        }

        public bool ShouldSerializeAlphaCutoff()
        {
            return ((m_alphaCutoff == 0.5F)
                        == false);
        }

        public bool ShouldSerializeDoubleSided()
        {
            return ((m_doubleSided == false)
                        == false);
        }

        public enum AlphaModeEnum
        {

            OPAQUE,

            MASK,

            BLEND,
        }
    }
}
