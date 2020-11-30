

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Accessor
  {
    private int? _bufferView;
    private int _byteOffset;
    private Accessor.GltfComponentType _componentType;
    private bool _normalized;
    private int _count;
    private Accessor.GltfType _type;
    private float[] _max;
    private float[] _min;
    private AccessorSparse _sparse;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

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

    [JsonPropertyName("byteOffset")]
    public int ByteOffset
    {
      get => this._byteOffset;
      set => this._byteOffset = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (ByteOffset), (object) value, "Expected value to be greater than or equal to 0");
    }

    [JsonPropertyName("componentType")]
    public Accessor.GltfComponentType ComponentType
    {
      get => this._componentType;
      set => this._componentType = value;
    }

    [JsonPropertyName("normalized")]
    public bool Normalized
    {
      get => this._normalized;
      set => this._normalized = value;
    }

    [JsonPropertyName("count")]
    public int Count
    {
      get => this._count;
      set => this._count = (double) value >= 1.0 ? value : throw new ArgumentOutOfRangeException(nameof (Count), (object) value, "Expected value to be greater than or equal to 1");
    }

    [JsonConverter(typeof (JsonStringEnumConverter))]
    [JsonPropertyName("type")]
    public Accessor.GltfType Type
    {
      get => this._type;
      set => this._type = value;
    }

    [JsonPropertyName("max")]
    public float[] Max
    {
      get => this._max;
      set
      {
        if (value == null)
        {
          this._max = value;
        }
        else
        {
          if (value.Length < 1)
            throw new ArgumentException("Array not long enough");
          this._max = value.Length <= 16 ? value : throw new ArgumentException("Array too long");
        }
      }
    }

    [JsonPropertyName("min")]
    public float[] Min
    {
      get => this._min;
      set
      {
        if (value == null)
        {
          this._min = value;
        }
        else
        {
          if (value.Length < 1)
            throw new ArgumentException("Array not long enough");
          this._min = value.Length <= 16 ? value : throw new ArgumentException("Array too long");
        }
      }
    }

    [JsonPropertyName("sparse")]
    public AccessorSparse Sparse
    {
      get => this._sparse;
      set => this._sparse = value;
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

    public bool ShouldSerializeBufferView() => this._bufferView.HasValue;

    public bool ShouldSerializeByteOffset() => (uint) this._byteOffset > 0U;

    public bool ShouldSerializeNormalized() => this._normalized;

    public bool ShouldSerializeMax() => this._max != null;

    public bool ShouldSerializeMin() => this._min != null;

    public bool ShouldSerializeSparse() => this._sparse != null;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;

    public enum GltfComponentType
    {
      Byte = 5120, // 0x00001400
      UnsignedByte = 5121, // 0x00001401
      Short = 5122, // 0x00001402
      UnsignedShort = 5123, // 0x00001403
      UnsignedInt = 5125, // 0x00001405
      Float = 5126, // 0x00001406
    }

    public enum GltfType
    {
      Scalar,
      Vec2,
      Vec3,
      Vec4,
      Mat2,
      Mat3,
      Mat4,
    }
  }
}
