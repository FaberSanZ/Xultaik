

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class AccessorSparseIndices
  {
    private int _bufferView;
    private int _byteOffset;
    private AccessorSparseIndices.GltfComponentType _componentType;
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

    [JsonPropertyName("componentType")]
    public AccessorSparseIndices.GltfComponentType ComponentType
    {
      get => this._componentType;
      set => this._componentType = value;
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

    public enum GltfComponentType
    {
      UnsignedByte = 5121, // 0x00001401
      UnsignedShort = 5123, // 0x00001403
      UnsignedInt = 5125, // 0x00001405
    }
  }
}
