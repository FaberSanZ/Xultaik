// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	AnimationChannelTarget.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class AnimationChannelTarget
    {
        [JsonPropertyName("node")]
        public int? Node { get; set; }



        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("path")]
        public AnimationChannelTarget.GltfPath Path { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }




        public bool ShouldSerializeNode() => Node.HasValue;

        public bool ShouldSerializeExtensions() => Extensions is not null;

        public bool ShouldSerializeExtras() => Extras is not null;
        

        public enum GltfPath
        {
            Translation,
            Rotation,
            Scale,
            Weights,
        }
    }
}
