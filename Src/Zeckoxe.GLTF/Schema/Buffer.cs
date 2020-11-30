

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Buffer
  {
    private string _uri;
    private int _byteLength;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("uri")]
    public string Uri
    {
      get => this._uri;
      set => this._uri = value;
    }

    [JsonPropertyName("byteLength")]
    public int ByteLength
    {
      get => this._byteLength;
      set => this._byteLength = (double) value >= 1.0 ? value : throw new ArgumentOutOfRangeException(nameof (ByteLength), (object) value, "Expected value to be greater than or equal to 1");
    }

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

    public bool ShouldSerializeUri() => this._uri != null;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
