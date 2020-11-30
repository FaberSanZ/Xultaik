

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class BufferView
  {
    private int _buffer;
    private int _byteOffset;
    private int _byteLength;
    private int? _byteStride;
    private BufferView.GltfTarget? _target;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("buffer")]
    public int Buffer
    {
      get => this._buffer;
      set => this._buffer = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (Buffer), (object) value, "Expected value to be greater than or equal to 0");
    }

    [JsonPropertyName("byteOffset")]
    public int ByteOffset
    {
      get => this._byteOffset;
      set => this._byteOffset = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (ByteOffset), (object) value, "Expected value to be greater than or equal to 0");
    }

    [JsonPropertyName("byteLength")]
    public int ByteLength
    {
      get => this._byteLength;
      set => this._byteLength = (double) value >= 1.0 ? value : throw new ArgumentOutOfRangeException(nameof (ByteLength), (object) value, "Expected value to be greater than or equal to 1");
    }

    [JsonPropertyName("byteStride")]
    public int? ByteStride
    {
      get => this._byteStride;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num1 = 4f;
        if ((double) nullable2.GetValueOrDefault() < (double) num1 & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (ByteStride), (object) value, "Expected value to be greater than or equal to 4");
        int? nullable3 = value;
        nullable2 = nullable3.HasValue ? new float?((float) nullable3.GetValueOrDefault()) : new float?();
        float num2 = 252f;
        if ((double) nullable2.GetValueOrDefault() > (double) num2 & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (ByteStride), (object) value, "Expected value to be less than or equal to 252");
        this._byteStride = value;
      }
    }

    [JsonPropertyName("target")]
    public BufferView.GltfTarget? Target
    {
      get => this._target;
      set => this._target = value;
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

    public bool ShouldSerializeByteOffset() => (uint) this._byteOffset > 0U;

    public bool ShouldSerializeByteStride() => this._byteStride.HasValue;

    public bool ShouldSerializeTarget() => this._target.HasValue;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;

    public enum GltfTarget
    {
      ArrayBuffer = 34962, // 0x00008892
      ElementArrayBuffer = 34963, // 0x00008893
    }
  }
}
