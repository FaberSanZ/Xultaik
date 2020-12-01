// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Camera.cs
=============================================================================*/


using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Zeckoxe.GLTF.Schema
{
    public class Camera
    {

        [JsonPropertyName("orthographic")]
        public CameraOrthographic Orthographic { get; set; }



        [JsonPropertyName("perspective")]
        public CameraPerspective Perspective { get; set; }



        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("type")]
        public Camera.GltfType Type { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }

        //TODO : Camera Type
        public enum GltfType
        {
            Perspective,

            Orthographic,
        }
    }
}
