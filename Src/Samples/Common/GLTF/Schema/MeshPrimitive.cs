// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	MeshPrimitive.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class MeshPrimitive
    {

        [JsonPropertyName("attributes")]
        public Dictionary<string, int> Attributes { get; set; }



        [JsonPropertyName("indices")]
        public int? Indices { get; set; }



        [JsonPropertyName("material")]
        public int? Material { get; set; }



        [JsonPropertyName("mode")]
        public MeshPrimitive.GltfMode Mode { get; set; }



        [JsonPropertyName("targets")]
        public Dictionary<string, int>[] Targets { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }




        // TODO: MeshPrimitive GltfMode
        public enum GltfMode
        {
            Points,
            Lines,
            LineLoop,
            LineStrip,
            Triangles,
            TriangleStrip,
            TriangleFan,
        }
    }
}
