

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Sampler
  {
    private Sampler.GltfMagFilter? _magFilter;
    private Sampler.GltfMinFilter? _minFilter;
    private Sampler.GltfWrapS _wrapS = Sampler.GltfWrapS.Repeat;
    private Sampler.GltfWrapT _wrapT = Sampler.GltfWrapT.Repeat;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("magFilter")]
    public Sampler.GltfMagFilter? MagFilter
    {
      get => this._magFilter;
      set => this._magFilter = value;
    }

    [JsonPropertyName("minFilter")]
    public Sampler.GltfMinFilter? MinFilter
    {
      get => this._minFilter;
      set => this._minFilter = value;
    }

    [JsonPropertyName("wrapS")]
    public Sampler.GltfWrapS WrapS
    {
      get => this._wrapS;
      set => this._wrapS = value;
    }

    [JsonPropertyName("wrapT")]
    public Sampler.GltfWrapT WrapT
    {
      get => this._wrapT;
      set => this._wrapT = value;
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

    public bool ShouldSerializeMagFilter() => this._magFilter.HasValue;

    public bool ShouldSerializeMinFilter() => this._minFilter.HasValue;

    public bool ShouldSerializeWrapS() => this._wrapS != Sampler.GltfWrapS.Repeat;

    public bool ShouldSerializeWrapT() => this._wrapT != Sampler.GltfWrapT.Repeat;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;

    public enum GltfMagFilter
    {
      Nearest = 9728, // 0x00002600
      Linear = 9729, // 0x00002601
    }

    public enum GltfMinFilter
    {
      Nearest = 9728, // 0x00002600
      Linear = 9729, // 0x00002601
      NearestMipmapNearest = 9984, // 0x00002700
      LinearMipmapNearest = 9985, // 0x00002701
      NearestMipmapLinear = 9986, // 0x00002702
      LinearMipmapLinear = 9987, // 0x00002703
    }

    public enum GltfWrapS
    {
      Repeat = 10497, // 0x00002901
      ClampToEdge = 33071, // 0x0000812F
      MirroredRepeat = 33648, // 0x00008370
    }

    public enum GltfWrapT
    {
      Repeat = 10497, // 0x00002901
      ClampToEdge = 33071, // 0x0000812F
      MirroredRepeat = 33648, // 0x00008370
    }
  }
}
