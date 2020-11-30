

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Image
  {
    private string _uri;
    private string _mimeType;
    private int? _bufferView;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("uri")]
    public string Uri
    {
      get => this._uri;
      set => this._uri = value;
    }

    [JsonPropertyName("mimeType")]
    public string MimeType
    {
      get => this._mimeType;
      set => this._mimeType = value;
    }

    [JsonPropertyName("bufferView")]
    public int? BufferView
    {
      get => this._bufferView;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num = 0.0f;
        if ((double) nullable2.GetValueOrDefault() < (double) num & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (BufferView), (object) value, "Expected value to be greater than or equal to 0");
        this._bufferView = value;
      }
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

    public bool ShouldSerializeMimeType() => this._mimeType != null;

    public bool ShouldSerializeBufferView() => this._bufferView.HasValue;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
