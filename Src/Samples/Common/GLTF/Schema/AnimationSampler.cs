// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	AnimationSampler.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class AnimationSampler
    {

        [JsonPropertyName("input")]
        public int Input { get; set; }



        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("interpolation")]
        public AnimationSampler.GltfInterpolation Interpolation { get; set; }



        [JsonPropertyName("output")]
        public int Output { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }




        public bool ShouldSerializeInterpolation() => (uint)Interpolation > 0U;

        public bool ShouldSerializeExtensions() => Extensions is not null;

        public bool ShouldSerializeExtras() => Extras is not null;
        

        public enum GltfInterpolation
        {
            Linear,
            Step,
            Cubicspline,
        }
    }
}
