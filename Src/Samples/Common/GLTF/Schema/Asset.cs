// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Asset.cs
=============================================================================*/

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class Asset
    {

        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }



        [JsonPropertyName("generator")]
        public string Generator { get; set; }



        [JsonPropertyName("version")]
        public string Version { get; set; }



        [JsonPropertyName("minVersion")]
        public string MinVersion { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }
    }
}
