// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	AccessorSparse.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zeckoxe.GLTF.Schema
{
    public class AccessorSparse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }



        [JsonPropertyName("indices")]
        public AccessorSparseIndices Indices { get; set; }



        [JsonPropertyName("values")]
        public AccessorSparseValues Values { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }





        public bool ShouldSerializeIndices() => Indices is not null;

        public bool ShouldSerializeValues() => Values is not null;

        public bool ShouldSerializeExtensions() => Extensions is not null;

        public bool ShouldSerializeExtras() => Extras is not null;
        
    }
}
