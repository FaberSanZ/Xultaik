

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class CameraPerspective
  {
    private float? _aspectRatio;
    private float _yfov;
    private float? _zfar;
    private float _znear;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("aspectRatio")]
    public float? AspectRatio
    {
      get => this._aspectRatio;
      set
      {
        float? nullable = value;
        float num = 0.0f;
        if ((double) nullable.GetValueOrDefault() <= (double) num & nullable.HasValue)
          throw new ArgumentOutOfRangeException(nameof (AspectRatio), (object) value, "Expected value to be greater than 0");
        this._aspectRatio = value;
      }
    }

    [JsonPropertyName("yfov")]
    public float Yfov
    {
      get => this._yfov;
      set => this._yfov = (double) value > 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (Yfov), (object) value, "Expected value to be greater than 0");
    }

    [JsonPropertyName("zfar")]
    public float? Zfar
    {
      get => this._zfar;
      set
      {
        float? nullable = value;
        float num = 0.0f;
        if ((double) nullable.GetValueOrDefault() <= (double) num & nullable.HasValue)
          throw new ArgumentOutOfRangeException(nameof (Zfar), (object) value, "Expected value to be greater than 0");
        this._zfar = value;
      }
    }

    [JsonPropertyName("znear")]
    public float Znear
    {
      get => this._znear;
      set => this._znear = (double) value > 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (Znear), (object) value, "Expected value to be greater than 0");
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

    public bool ShouldSerializeAspectRatio() => this._aspectRatio.HasValue;

    public bool ShouldSerializeZfar() => this._zfar.HasValue;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
