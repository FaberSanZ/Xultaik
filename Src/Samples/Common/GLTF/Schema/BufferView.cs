// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	BufferView.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class BufferView
    {

        [JsonPropertyName("buffer")]
        public int Buffer { get; set; }



        [JsonPropertyName("byteOffset")]
        public int ByteOffset { get; set; }



        [JsonPropertyName("byteLength")]
        public int ByteLength { get; set; }



        [JsonPropertyName("byteStride")]
        public int? ByteStride { get; set; }



        [JsonPropertyName("target")]
        public BufferView.GltfTarget? Target { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

        

        public enum GltfTarget
        {
            ArrayBuffer = 0x00008892,

            ElementArrayBuffer = 0x00008893
        }
    }
}
