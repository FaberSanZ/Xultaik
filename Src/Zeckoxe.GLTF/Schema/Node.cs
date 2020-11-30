

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Node
  {
    private int? _camera;
    private int[] _children;
    private int? _skin;
    private float[] _matrix = new float[16]
    {
      1f,
      0.0f,
      0.0f,
      0.0f,
      0.0f,
      1f,
      0.0f,
      0.0f,
      0.0f,
      0.0f,
      1f,
      0.0f,
      0.0f,
      0.0f,
      0.0f,
      1f
    };
    private int? _mesh;
    private float[] _rotation = new float[4]
    {
      0.0f,
      0.0f,
      0.0f,
      1f
    };
    private float[] _scale = new float[3]{ 1f, 1f, 1f };
    private float[] _translation = new float[3];
    private float[] _weights;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("camera")]
    public int? Camera
    {
      get => this._camera;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num = 0.0f;
        if ((double) nullable2.GetValueOrDefault() < (double) num & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (Camera), (object) value, "Expected value to be greater than or equal to 0");
        this._camera = value;
      }
    }

    [JsonPropertyName("children")]
    public int[] Children
    {
      get => this._children;
      set
      {
        if (value == null)
        {
          this._children = value;
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
          this._children = value;
        }
      }
    }

    [JsonPropertyName("skin")]
    public int? Skin
    {
      get => this._skin;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num = 0.0f;
        if ((double) nullable2.GetValueOrDefault() < (double) num & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (Skin), (object) value, "Expected value to be greater than or equal to 0");
        this._skin = value;
      }
    }

    [JsonPropertyName("matrix")]
    public float[] Matrix
    {
      get => this._matrix;
      set
      {
        if (value == null)
        {
          this._matrix = value;
        }
        else
        {
          if (value.Length < 16)
            throw new ArgumentException("Array not long enough");
          this._matrix = value.Length <= 16 ? value : throw new ArgumentException("Array too long");
        }
      }
    }

    [JsonPropertyName("mesh")]
    public int? Mesh
    {
      get => this._mesh;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num = 0.0f;
        if ((double) nullable2.GetValueOrDefault() < (double) num & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (Mesh), (object) value, "Expected value to be greater than or equal to 0");
        this._mesh = value;
      }
    }

    [JsonPropertyName("rotation")]
    public float[] Rotation
    {
      get => this._rotation;
      set
      {
        if (value == null)
        {
          this._rotation = value;
        }
        else
        {
          if (value.Length < 4)
            throw new ArgumentException("Array not long enough");
          if (value.Length > 4)
            throw new ArgumentException("Array too long");
          for (int index = 0; index < value.Length; ++index)
          {
            if ((double) value[index] < -1.0)
              throw new ArgumentOutOfRangeException();
          }
          for (int index = 0; index < value.Length; ++index)
          {
            if ((double) value[index] > 1.0)
              throw new ArgumentOutOfRangeException();
          }
          this._rotation = value;
        }
      }
    }

    [JsonPropertyName("scale")]
    public float[] Scale
    {
      get => this._scale;
      set
      {
        if (value == null)
        {
          this._scale = value;
        }
        else
        {
          if (value.Length < 3)
            throw new ArgumentException("Array not long enough");
          this._scale = value.Length <= 3 ? value : throw new ArgumentException("Array too long");
        }
      }
    }

    [JsonPropertyName("translation")]
    public float[] Translation
    {
      get => this._translation;
      set
      {
        if (value == null)
        {
          this._translation = value;
        }
        else
        {
          if (value.Length < 3)
            throw new ArgumentException("Array not long enough");
          this._translation = value.Length <= 3 ? value : throw new ArgumentException("Array too long");
        }
      }
    }

    [JsonPropertyName("weights")]
    public float[] Weights
    {
      get => this._weights;
      set
      {
        if (value == null)
          this._weights = value;
        else
          this._weights = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
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

    public bool ShouldSerializeCamera() => this._camera.HasValue;

    public bool ShouldSerializeChildren() => this._children != null;

    public bool ShouldSerializeSkin() => this._skin.HasValue;

    public bool ShouldSerializeMatrix() => !((IEnumerable<float>) this._matrix).SequenceEqual<float>((IEnumerable<float>) new float[16]
    {
      1f,
      0.0f,
      0.0f,
      0.0f,
      0.0f,
      1f,
      0.0f,
      0.0f,
      0.0f,
      0.0f,
      1f,
      0.0f,
      0.0f,
      0.0f,
      0.0f,
      1f
    });

    public bool ShouldSerializeMesh() => this._mesh.HasValue;

    public bool ShouldSerializeRotation() => !((IEnumerable<float>) this._rotation).SequenceEqual<float>((IEnumerable<float>) new float[4]
    {
      0.0f,
      0.0f,
      0.0f,
      1f
    });

    public bool ShouldSerializeScale() => !((IEnumerable<float>) this._scale).SequenceEqual<float>((IEnumerable<float>) new float[3]
    {
      1f,
      1f,
      1f
    });

    public bool ShouldSerializeTranslation() => !((IEnumerable<float>) this._translation).SequenceEqual<float>((IEnumerable<float>) new float[3]);

    public bool ShouldSerializeWeights() => this._weights != null;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
