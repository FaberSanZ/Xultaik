// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	AnimationChannel.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zeckoxe.GLTF.Schema
{
    public class AnimationChannel
    {
        [JsonPropertyName("sampler")]
        public int Sampler { get; set; }



        [JsonPropertyName("target")]
        public AnimationChannelTarget Target { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }





        public bool ShouldSerializeTarget() => Target is not null;

        public bool ShouldSerializeExtensions() => Extensions is not null;

        public bool ShouldSerializeExtras() => Extras is not null;
        
    }
}
