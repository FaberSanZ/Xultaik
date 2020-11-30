

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GltfLoader.Schema
{
  public class Scene
  {
    private int[] _nodes;
    private string _name;
    private Dictionary<string, object> _extensions;
    private Extras _extras;

    [JsonPropertyName("nodes")]
    public int[] Nodes
    {
      get => this._nodes;
      set
      {
        if (value == null)
        {
          this._nodes = value;
        }
        else
        {
          if (value.Length < 1)
            throw new ArgumentException("Array not long enough");
          for (int index = 0; index < value.Length; ++index)
          {
            if ((double) value[index] < 0.0)
              throw new ArgumentOutOfRangeException();
          }
          this._nodes = value;
        }
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

    public bool ShouldSerializeNodes() => this._nodes != null;

    public bool ShouldSerializeName() => this._name != null;

    public bool ShouldSerializeExtensions() => this._extensions != null;

    public bool ShouldSerializeExtras() => this._extras != null;
  }
}
