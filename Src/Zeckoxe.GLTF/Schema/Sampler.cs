// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Sampler.cs
=============================================================================*/

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zeckoxe.GLTF.Schema
{
    // TODO: Sampler
    public class Sampler
    {

        [JsonPropertyName("magFilter")]
        public Sampler.GltfMagFilter? MagFilter { get; set; }



        [JsonPropertyName("minFilter")]
        public Sampler.GltfMinFilter? MinFilter { get; set; }



        [JsonPropertyName("wrapS")]
        public Sampler.GltfWrapS WrapS { get; set; }



        [JsonPropertyName("wrapT")]
        public Sampler.GltfWrapT WrapT { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }
       

        public enum GltfMagFilter
        {
            Nearest = 0x00002600,

            Linear = 0x00002601
        }

        public enum GltfMinFilter
        {
            Nearest = 0x00002600,

            Linear = 0x00002601,

            NearestMipmapNearest =0x00002700,

            LinearMipmapNearest = 0x00002701,

            NearestMipmapLinear = 0x00002702,

            LinearMipmapLinear = 0x00002703,
        }

        public enum GltfWrapS
        {
            Repeat = 0x00002901,

            ClampToEdge = 0x0000812F,

            MirroredRepeat = 0x00008370
        }

        public enum GltfWrapT
        {
            Repeat = 0x00002901,

            ClampToEdge = 0x0000812F,

            MirroredRepeat = 0x00008370
        }
    }
}
