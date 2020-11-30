
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Mesh
  {
    private MeshPrimitive[] _primitives;
    private float[] _weights;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("primitives")]
    public MeshPrimitive[] Primitives
    {
      get => this._primitives;
      set
      {
        if (value == null)
          this._primitives = value;
        else
          this._primitives = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
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

    public bool ShouldSerializePrimitives() => this._primitives != null;

    public bool ShouldSerializeWeights() => this._weights != null;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
