

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class MeshPrimitive
  {
    private Dictionary<string, int> _attributes;
    private int? _indices;
    private int? _material;
    private MeshPrimitive.GltfMode _mode = MeshPrimitive.GltfMode.Triangles;
    private Dictionary<string, int>[] _targets;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("attributes")]
    public Dictionary<string, int> Attributes
    {
      get => this._attributes;
      set => this._attributes = value;
    }

    [JsonPropertyName("indices")]
    public int? Indices
    {
      get => this._indices;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num = 0.0f;
        if ((double) nullable2.GetValueOrDefault() < (double) num & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (Indices), (object) value, "Expected value to be greater than or equal to 0");
        this._indices = value;
      }
    }

    [JsonPropertyName("material")]
    public int? Material
    {
      get => this._material;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num = 0.0f;
        if ((double) nullable2.GetValueOrDefault() < (double) num & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (Material), (object) value, "Expected value to be greater than or equal to 0");
        this._material = value;
      }
    }

    [JsonPropertyName("mode")]
    public MeshPrimitive.GltfMode Mode
    {
      get => this._mode;
      set => this._mode = value;
    }

    [JsonPropertyName("targets")]
    public Dictionary<string, int>[] Targets
    {
      get => this._targets;
      set
      {
        if (value == null)
          this._targets = value;
        else
          this._targets = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
      }
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

    public bool ShouldSerializeAttributes() => this._attributes != null;

    public bool ShouldSerializeIndices() => this._indices.HasValue;

    public bool ShouldSerializeMaterial() => this._material.HasValue;

    public bool ShouldSerializeMode() => this._mode != MeshPrimitive.GltfMode.Triangles;

    public bool ShouldSerializeTargets() => this._targets != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;

    public enum GltfMode
    {
      Points,
      Lines,
      LineLoop,
      LineStrip,
      Triangles,
      TriangleStrip,
      TriangleFan,
    }
  }
}
