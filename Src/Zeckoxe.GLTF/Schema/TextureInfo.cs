

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class TextureInfo
  {
    private int _index;
    private int _texCoord;
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

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
