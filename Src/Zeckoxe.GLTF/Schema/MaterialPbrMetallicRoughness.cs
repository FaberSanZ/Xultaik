

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class MaterialPbrMetallicRoughness
  {
    private float[] _baseColorFactor = new float[4]
    {
      1f,
      1f,
      1f,
      1f
    };
    private TextureInfo _baseColorTexture;
    private float _metallicFactor = 1f;
    private float _roughnessFactor = 1f;
    private TextureInfo _metallicRoughnessTexture;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("baseColorFactor")]
    public float[] BaseColorFactor
    {
      get => this._baseColorFactor;
      set
      {
        if (value == null)
        {
          this._baseColorFactor = value;
        }
        else
        {
          if (value.Length < 4)
            throw new ArgumentException("Array not long enough");
          if (value.Length > 4)
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
          this._baseColorFactor = value;
        }
      }
    }

    [JsonPropertyName("baseColorTexture")]
    public TextureInfo BaseColorTexture
    {
      get => this._baseColorTexture;
      set => this._baseColorTexture = value;
    }

    [JsonPropertyName("metallicFactor")]
    public float MetallicFactor
    {
      get => this._metallicFactor;
      set
      {
        if ((double) value < 0.0)
          throw new ArgumentOutOfRangeException(nameof (MetallicFactor), (object) value, "Expected value to be greater than or equal to 0");
        this._metallicFactor = (double) value <= 1.0 ? value : throw new ArgumentOutOfRangeException(nameof (MetallicFactor), (object) value, "Expected value to be less than or equal to 1");
      }
    }

    [JsonPropertyName("roughnessFactor")]
    public float RoughnessFactor
    {
      get => this._roughnessFactor;
      set
      {
        if ((double) value < 0.0)
          throw new ArgumentOutOfRangeException(nameof (RoughnessFactor), (object) value, "Expected value to be greater than or equal to 0");
        this._roughnessFactor = (double) value <= 1.0 ? value : throw new ArgumentOutOfRangeException(nameof (RoughnessFactor), (object) value, "Expected value to be less than or equal to 1");
      }
    }

    [JsonPropertyName("metallicRoughnessTexture")]
    public TextureInfo MetallicRoughnessTexture
    {
      get => this._metallicRoughnessTexture;
      set => this._metallicRoughnessTexture = value;
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

    public bool ShouldSerializeBaseColorFactor() => !((IEnumerable<float>) this._baseColorFactor).SequenceEqual<float>((IEnumerable<float>) new float[4]
    {
      1f,
      1f,
      1f,
      1f
    });

    public bool ShouldSerializeBaseColorTexture() => this._baseColorTexture != null;

    public bool ShouldSerializeMetallicFactor() => (double) this._metallicFactor != 1.0;

    public bool ShouldSerializeRoughnessFactor() => (double) this._roughnessFactor != 1.0;

    public bool ShouldSerializeMetallicRoughnessTexture() => this._metallicRoughnessTexture != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
