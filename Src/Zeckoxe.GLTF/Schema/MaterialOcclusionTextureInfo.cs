

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class MaterialOcclusionTextureInfo
  {
    private int _index;
    private int _texCoord;
    private float _strength = 1f;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("index")]
    public int Index
    {
      get => this._index;
      set => this._index = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (Index), (object) value, "Expected value to be greater than or equal to 0");
    }

    [JsonPropertyName("texCoord")]
    public int TexCoord
    {
      get => this._texCoord;
      set => this._texCoord = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (TexCoord), (object) value, "Expected value to be greater than or equal to 0");
    }

    [JsonPropertyName("strength")]
    public float Strength
    {
      get => this._strength;
      set
      {
        if ((double) value < 0.0)
          throw new ArgumentOutOfRangeException(nameof (Strength), (object) value, "Expected value to be greater than or equal to 0");
        this._strength = (double) value <= 1.0 ? value : throw new ArgumentOutOfRangeException(nameof (Strength), (object) value, "Expected value to be less than or equal to 1");
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

    public bool ShouldSerializeTexCoord() => (uint) this._texCoord > 0U;

    public bool ShouldSerializeStrength() => (double) this._strength != 1.0;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
