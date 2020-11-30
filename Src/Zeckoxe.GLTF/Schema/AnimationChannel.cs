

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class AnimationChannel
  {
    private int _sampler;
    private AnimationChannelTarget _target;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("sampler")]
    public int Sampler
    {
      get => this._sampler;
      set => this._sampler = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (Sampler), (object) value, "Expected value to be greater than or equal to 0");
    }

    [JsonPropertyName("target")]
    public AnimationChannelTarget Target
    {
      get => this._target;
      set => this._target = value;
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

    public bool ShouldSerializeTarget() => this._target != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
