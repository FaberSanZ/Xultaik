// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class Accessor
    {
        [JsonPropertyName("bufferView")]
        public int? BufferView { get; set; }



        [JsonPropertyName("byteOffset")]
        public int ByteOffset { get; set; }



        [JsonPropertyName("componentType")]
        public GltfComponentType ComponentType { get; set; }



        [JsonPropertyName("normalized")]
        public bool Normalized { get; set; }



        [JsonPropertyName("count")]
        public int Count { get; set; }



        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("type")]
        public GltfType Type { get; set; }



        [JsonPropertyName("max")]
        public float[] Max { get; set; }



        [JsonPropertyName("min")]
        public float[] Min { get; set; }



        [JsonPropertyName("sparse")]
        public AccessorSparse Sparse { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }




        public bool ShouldSerializeBufferView() => BufferView.HasValue;

        public bool ShouldSerializeByteOffset() => (uint)ByteOffset > 0U;

        public bool ShouldSerializeNormalized() => Normalized;

        public bool ShouldSerializeMax() => Max is not null;

        public bool ShouldSerializeMin() => Min is not null;

        public bool ShouldSerializeSparse() => Sparse is not null;
        
        public bool ShouldSerializeName() => Name is not null;

        public bool ShouldSerializeExtensions() => Extensions is not null;

        public bool ShouldSerializeExtras() => Extras is not null;
        
    }
}
