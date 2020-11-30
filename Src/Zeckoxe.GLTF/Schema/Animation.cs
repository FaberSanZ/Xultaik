

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Animation
  {
    private AnimationChannel[] _channels;
    private AnimationSampler[] _samplers;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("channels")]
    public AnimationChannel[] Channels
    {
      get => this._channels;
      set
      {
        if (value == null)
          this._channels = value;
        else
          this._channels = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
      }
    }

    [JsonPropertyName("samplers")]
    public AnimationSampler[] Samplers
    {
      get => this._samplers;
      set
      {
        if (value == null)
          this._samplers = value;
        else
          this._samplers = value.Length >= 1 ? value : throw new ArgumentException("Array not long enough");
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

    public bool ShouldSerializeChannels() => this._channels != null;

    public bool ShouldSerializeSamplers() => this._samplers != null;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
