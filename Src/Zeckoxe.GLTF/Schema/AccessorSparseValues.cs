// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	AccessorSparseValues.cs
=============================================================================*/

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zeckoxe.GLTF.Schema
{
    public class AccessorSparseValues
    {
        [JsonPropertyName("bufferView")]
        public int BufferView { get; set; }



        [JsonPropertyName("byteOffset")]
        public int ByteOffset { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }




        public bool ShouldSerializeByteOffset() => (uint)ByteOffset > 0U;

        public bool ShouldSerializeExtensions() => Extensions is not null;

        public bool ShouldSerializeExtras() => Extras is not null;
        
    }
}
