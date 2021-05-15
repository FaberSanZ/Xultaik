// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	CameraPerspective.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class CameraPerspective
    {

        [JsonPropertyName("aspectRatio")]
        public float? AspectRatio { get; set; }



        [JsonPropertyName("yfov")]
        public float Yfov { get; set; }



        [JsonPropertyName("zfar")]
        public float? Zfar { get; set; }



        [JsonPropertyName("znear")]
        public float Znear { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }
       
    }
}
