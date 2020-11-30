

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Camera
  {
    private CameraOrthographic _orthographic;
    private CameraPerspective _perspective;
    private Camera.GltfType _type;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("orthographic")]
    public CameraOrthographic Orthographic
    {
      get => this._orthographic;
      set => this._orthographic = value;
    }

    [JsonPropertyName("perspective")]
    public CameraPerspective Perspective
    {
      get => this._perspective;
      set => this._perspective = value;
    }

    [JsonConverter(typeof (JsonStringEnumConverter))]
    [JsonPropertyName("type")]
    public Camera.GltfType Type
    {
      get => this._type;
      set => this._type = value;
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

    public bool ShouldSerializeOrthographic() => this._orthographic != null;

    public bool ShouldSerializePerspective() => this._perspective != null;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;

    public enum GltfType
    {
      Perspective,
      Orthographic,
    }
  }
}
