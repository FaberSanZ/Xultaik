
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class CameraOrthographic
  {
    private float _xmag;
    private float _ymag;
    private float _zfar;
    private float _znear;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("xmag")]
    public float Xmag
    {
      get => this._xmag;
      set => this._xmag = value;
    }

    [JsonPropertyName("ymag")]
    public float Ymag
    {
      get => this._ymag;
      set => this._ymag = value;
    }

    [JsonPropertyName("zfar")]
    public float Zfar
    {
      get => this._zfar;
      set => this._zfar = (double) value > 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (Zfar), (object) value, "Expected value to be greater than 0");
    }

    [JsonPropertyName("znear")]
    public float Znear
    {
      get => this._znear;
      set => this._znear = (double) value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof (Znear), (object) value, "Expected value to be greater than or equal to 0");
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

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
