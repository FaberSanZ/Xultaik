

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class AnimationChannelTarget
  {
    private int? _node;
    private AnimationChannelTarget.GltfPath _path;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("node")]
    public int? Node
    {
      get => this._node;
      set
      {
        int? nullable1 = value;
        float? nullable2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
        float num = 0.0f;
        if ((double) nullable2.GetValueOrDefault() < (double) num & nullable2.HasValue)
          throw new ArgumentOutOfRangeException(nameof (Node), (object) value, "Expected value to be greater than or equal to 0");
        this._node = value;
      }
    }

    [JsonConverter(typeof (JsonStringEnumConverter))]
    [JsonPropertyName("path")]
    public AnimationChannelTarget.GltfPath Path
    {
      get => this._path;
      set => this._path = value;
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

    public bool ShouldSerializeNode() => this._node.HasValue;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;

    public enum GltfPath
    {
      Translation,
      Rotation,
      Scale,
      Weights,
    }
  }
}
