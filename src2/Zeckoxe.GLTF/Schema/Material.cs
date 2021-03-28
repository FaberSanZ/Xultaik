// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Material.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Zeckoxe.GLTF.Schema
{
    public class Material
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }



        [JsonPropertyName("pbrMetallicRoughness")]
        public MaterialPbrMetallicRoughness PbrMetallicRoughness { get; set; }



        [JsonPropertyName("normalTexture")]
        public MaterialNormalTextureInfo NormalTexture { get; set; }



        [JsonPropertyName("occlusionTexture")]
        public MaterialOcclusionTextureInfo OcclusionTexture { get; set; }



        [JsonPropertyName("emissiveTexture")]
        public TextureInfo EmissiveTexture { get; set; }



        [JsonPropertyName("emissiveFactor")]
        public float[] EmissiveFactor { get; set; }



        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("alphaMode")]
        public Material.GltfAlphaMode AlphaMode { get; set; }



        [JsonPropertyName("alphaCutoff")]
        public float AlphaCutoff { get; set; }



        [JsonPropertyName("doubleSided")]
        public bool DoubleSided { get; set; }
        


        public enum GltfAlphaMode
        {
            Opaque,

            Mask,

            Blend,
        }
    }
}
