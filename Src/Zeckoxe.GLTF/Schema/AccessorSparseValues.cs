

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class AccessorSparseValues
  {
    private int _bufferView;
    private int _byteOffset;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("bufferView")]
    public int BufferView
    {
      get => this._bufferView;
      set => this._bufferView = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (BufferView), (object) value, "Expected value to be greater than or equal to 0");
    }

    [JsonPropertyName("byteOffset")]
    public int ByteOffset
    {
      get => this._byteOffset;
      set => this._byteOffset = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (ByteOffset), (object) value, "Expected value to be greater than or equal to 0");
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

    public bool ShouldSerializeByteOffset() => (uint) this._byteOffset > 0U;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
