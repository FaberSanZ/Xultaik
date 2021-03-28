// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Skin.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zeckoxe.GLTF.Schema
{
    public class Skin
    {

        [JsonPropertyName("inverseBindMatrices")]
        public int? InverseBindMatrices { get; set; }



        [JsonPropertyName("skeleton")]
        public int? Skeleton { get; set; }



        [JsonPropertyName("joints")]
        public int[] Joints { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }
        
    }
}
