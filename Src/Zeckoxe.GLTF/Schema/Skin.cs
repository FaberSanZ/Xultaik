

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Skin
  {
    private int? _inverseBindMatrices;
    private int? _skeleton;
    private int[] _joints;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("inverseBindMatrices")]
    public int? InverseBindMatrices
    {
      get => this._inverseBindMatrices;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num = 0.0f;
        if ((double) nullable2.GetValueOrDefault() < (double) num & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (InverseBindMatrices), (object) value, "Expected value to be greater than or equal to 0");
        this._inverseBindMatrices = value;
      }
    }

    [JsonPropertyName("skeleton")]
    public int? Skeleton
    {
      get => this._skeleton;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num = 0.0f;
        if ((double) nullable2.GetValueOrDefault() < (double) num & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (Skeleton), (object) value, "Expected value to be greater than or equal to 0");
        this._skeleton = value;
      }
    }

    [JsonPropertyName("joints")]
    public int[] Joints
    {
      get => this._joints;
      set
      {
        if (value == null)
        {
          this._joints = value;
        }
        else
        {
          if (value.Length < 1)
            throw new ArgumentException("Array not long enough");
          for (int index = 0; index < value.Length; ++index)
          {
            if ((double) value[index] < 0.0)
              throw new ArgumentOutOfRangeException();
          }
          this._joints = value;
        }
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

    public bool ShouldSerializeInverseBindMatrices() => this._inverseBindMatrices.HasValue;

    public bool ShouldSerializeSkeleton() => this._skeleton.HasValue;

    public bool ShouldSerializeJoints() => this._joints != null;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
