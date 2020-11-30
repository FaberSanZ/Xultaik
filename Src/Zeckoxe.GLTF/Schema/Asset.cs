

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Asset
  {
    private string _copyright;
    private string _generator;
    private string _version;
    private string _minVersion;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("copyright")]
    public string Copyright
    {
      get => this._copyright;
      set => this._copyright = value;
    }

    [JsonPropertyName("generator")]
    public string Generator
    {
      get => this._generator;
      set => this._generator = value;
    }

    [JsonPropertyName("version")]
    public string Version
    {
      get => this._version;
      set => this._version = value;
    }

    [JsonPropertyName("minVersion")]
    public string MinVersion
    {
      get => this._minVersion;
      set => this._minVersion = value;
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

    public bool ShouldSerializeCopyright() => this._copyright != null;

    public bool ShouldSerializeGenerator() => this._generator != null;

    public bool ShouldSerializeVersion() => this._version != null;

    public bool ShouldSerializeMinVersion() => this._minVersion != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
