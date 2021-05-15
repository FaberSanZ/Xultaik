// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	AccessorSparseIndices.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class AccessorSparseIndices
    {

        [JsonPropertyName("bufferView")]
        public int BufferView { get; set; }



        [JsonPropertyName("byteOffset")]
        public int ByteOffset { get; set; }



        [JsonPropertyName("componentType")]
        public AccessorSparseIndices.GltfComponentType ComponentType { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }




        public bool ShouldSerializeByteOffset() => (uint)ByteOffset > 0U;

        public bool ShouldSerializeExtensions() => Extensions is not null;

        public bool ShouldSerializeExtras() => Extras is not null;
        


        public enum GltfComponentType
        {
            UnsignedByte =  0x00001401,

            UnsignedShort = 0x00001403,

            UnsignedInt = 0x00001405,
        }
    }
}
