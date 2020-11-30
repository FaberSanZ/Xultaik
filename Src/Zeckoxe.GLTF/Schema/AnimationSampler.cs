

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class AnimationSampler
  {
    private int _input;
    private AnimationSampler.GltfInterpolation _interpolation;
    private int _output;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("input")]
    public int Input
    {
      get => this._input;
      set => this._input = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (Input), (object) value, "Expected value to be greater than or equal to 0");
    }

    [JsonConverter(typeof (JsonStringEnumConverter))]
    [JsonPropertyName("interpolation")]
    public AnimationSampler.GltfInterpolation Interpolation
    {
      get => this._interpolation;
      set => this._interpolation = value;
    }

    [JsonPropertyName("output")]
    public int Output
    {
      get => this._output;
      set => this._output = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (Output), (object) value, "Expected value to be greater than or equal to 0");
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

    public bool ShouldSerializeInterpolation() => (uint) this._interpolation > 0U;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;

    public enum GltfInterpolation
    {
      Linear,
      Step,
      Cubicspline,
    }
  }
}
