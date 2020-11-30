

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class AccessorSparse
  {
    private int _count;
    private AccessorSparseIndices _indices;
    private AccessorSparseValues _values;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("count")]
    public int Count
    {
      get => this._count;
      set => this._count = (double) value >= 1.0 ? value : throw new ArgumentOutOfRangeException(nameof (Count), (object) value, "Expected value to be greater than or equal to 1");
    }

    [JsonPropertyName("indices")]
    public AccessorSparseIndices Indices
    {
      get => this._indices;
      set => this._indices = value;
    }

    [JsonPropertyName("values")]
    public AccessorSparseValues Values
    {
      get => this._values;
      set => this._values = value;
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

    public bool ShouldSerializeIndices() => this._indices != null;

    public bool ShouldSerializeValues() => this._values != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
