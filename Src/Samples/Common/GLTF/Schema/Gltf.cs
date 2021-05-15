// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Gltf.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class Gltf
    {

        [JsonPropertyName("extensionsUsed")]
        public string[] ExtensionsUsed { get; set; }



        [JsonPropertyName("extensionsRequired")]
        public string[] ExtensionsRequired { get; set; }



        [JsonPropertyName("accessors")]
        public Accessor[] Accessors { get; set; }



        [JsonPropertyName("animations")]
        public Animation[] Animations { get; set; }



        [JsonPropertyName("asset")]
        public Asset Asset { get; set; }



        [JsonPropertyName("buffers")]
        public Buffer[] Buffers { get; set; }



        [JsonPropertyName("bufferViews")]
        public BufferView[] BufferViews { get; set; }



        [JsonPropertyName("cameras")]
        public Camera[] Cameras { get; set; }



        [JsonPropertyName("images")]
        public Image[] Images { get; set; }



        [JsonPropertyName("materials")]
        public Material[] Materials { get; set; }



        [JsonPropertyName("meshes")]
        public Mesh[] Meshes { get; set; }



        [JsonPropertyName("nodes")]
        public Node[] Nodes { get; set; }



        [JsonPropertyName("samplers")]
        public Sampler[] Samplers { get; set; }



        [JsonPropertyName("scene")]
        public int? Scene { get; set; }



        [JsonPropertyName("scenes")]
        public Schema.Scene[] Scenes { get; set; }



        [JsonPropertyName("skins")]
        public Skin[] Skins { get; set; }



        [JsonPropertyName("textures")]
        public Texture[] Textures { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

       
    }
}
