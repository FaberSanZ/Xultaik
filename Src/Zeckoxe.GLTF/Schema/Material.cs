

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Material
  {
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;
    private MaterialPbrMetallicRoughness _pbrMetallicRoughness;
    private MaterialNormalTextureInfo _normalTexture;
    private MaterialOcclusionTextureInfo _occlusionTexture;
    private TextureInfo _emissiveTexture;
    private float[] _emissiveFactor = new float[3];
    private Material.GltfAlphaMode _alphaMode;
    private float _alphaCutoff = 0.5f;
    private bool _doubleSided;

    [JsonPropertyName("name")]
    public string Name
    {
      get => this._name;
      set => this._name = value;
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

    [JsonPropertyName("pbrMetallicRoughness")]
    public MaterialPbrMetallicRoughness PbrMetallicRoughness
    {
      get => this._pbrMetallicRoughness;
      set => this._pbrMetallicRoughness = value;
    }

    [JsonPropertyName("normalTexture")]
    public MaterialNormalTextureInfo NormalTexture
    {
      get => this._normalTexture;
      set => this._normalTexture = value;
    }

    [JsonPropertyName("occlusionTexture")]
    public MaterialOcclusionTextureInfo OcclusionTexture
    {
      get => this._occlusionTexture;
      set => this._occlusionTexture = value;
    }

    [JsonPropertyName("emissiveTexture")]
    public TextureInfo EmissiveTexture
    {
      get => this._emissiveTexture;
      set => this._emissiveTexture = value;
    }

    [JsonPropertyName("emissiveFactor")]
    public float[] EmissiveFactor
    {
      get => this._emissiveFactor;
      set
      {
        if (value == null)
        {
          this._emissiveFactor = value;
        }
        else
        {
          if (value.Length < 3)
            throw new ArgumentException("Array not long enough");
          if (value.Length > 3)
            throw new ArgumentException("Array too long");
          for (int index = 0; index < value.Length; ++index)
          {
            if ((double) value[index] < 0.0)
              throw new ArgumentOutOfRangeException();
          }
          for (int index = 0; index < value.Length; ++index)
          {
            if ((double) value[index] > 1.0)
              throw new ArgumentOutOfRangeException();
          }
          this._emissiveFactor = value;
        }
      }
    }

    [JsonConverter(typeof (JsonStringEnumConverter))]
    [JsonPropertyName("alphaMode")]
    public Material.GltfAlphaMode AlphaMode
    {
      get => this._alphaMode;
      set => this._alphaMode = value;
    }

    [JsonPropertyName("alphaCutoff")]
    public float AlphaCutoff
    {
      get => this._alphaCutoff;
      set => this._alphaCutoff = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (AlphaCutoff), (object) value, "Expected value to be greater than or equal to 0");
    }

    [JsonPropertyName("doubleSided")]
    public bool DoubleSided
    {
      get => this._doubleSided;
      set => this._doubleSided = value;
    }

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;

    public bool ShouldSerializePbrMetallicRoughness() => this._pbrMetallicRoughness != null;

    public bool ShouldSerializeNormalTexture() => this._normalTexture != null;

    public bool ShouldSerializeOcclusionTexture() => this._occlusionTexture != null;

    public bool ShouldSerializeEmissiveTexture() => this._emissiveTexture != null;

    public bool ShouldSerializeEmissiveFactor() => !((IEnumerable<float>) this._emissiveFactor).SequenceEqual<float>((IEnumerable<float>) new float[3]);

    public bool ShouldSerializeAlphaMode() => (uint) this._alphaMode > 0U;

    public bool ShouldSerializeAlphaCutoff() => (double) this._alphaCutoff != 0.5;

    public bool ShouldSerializeDoubleSided() => this._doubleSided;

    public enum GltfAlphaMode
    {
      Opaque,
      Mask,
      Blend,
    }
  }
}
