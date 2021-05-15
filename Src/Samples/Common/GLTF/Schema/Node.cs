// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Node.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{

    public class Node
    {
        private float[] _matrix = new float[16]
        {

            1f, 0.0f, 0.0f, 0.0f,
      
            0.0f, 1f, 0.0f, 0.0f,
      
            0.0f, 0.0f, 1f, 0.0f,

            0.0f, 0.0f, 0.0f, 1f
        };

        private float[] _rotation = new float[4] { 0.0f, 0.0f, 0.0f, 1f };
        private float[] _scale = new float[3] { 1f, 1f, 1f };
        private float[] _translation = new float[3];





        [JsonPropertyName("camera")]
        public int? Camera { get; set; }



        [JsonPropertyName("children")]
        public int[] Children { get; set; }



        [JsonPropertyName("skin")]
        public int? Skin { get; set; }



        [JsonPropertyName("matrix")]
        public float[] Matrix
        {
            get => _matrix;
            set
            {
                if (value is null)
                    _matrix = value;
                else
                {
                    if (value.Length < 16)
                        throw new ArgumentException("Array not long enough");

                    _matrix = value.Length <= 16 ? value : throw new ArgumentException("Array too long");
                }
            }
        }

        [JsonPropertyName("mesh")]
        public int? Mesh { get; set; }



        [JsonPropertyName("rotation")]
        public float[] Rotation
        {
            get => _rotation;
            set
            {
                if (value is null)
                    _rotation = value;
                else
                {
                    if (value.Length < 4)
                        throw new ArgumentException("Array not long enough");

                    if (value.Length > 4)
                        throw new ArgumentException("Array too long");

                    for (int index = 0; index < value.Length; ++index)
                        if (value[index] < -1.0)
                            throw new ArgumentOutOfRangeException();

                    for (int index = 0; index < value.Length; ++index)
                        if (value[index] > 1.0)
                            throw new ArgumentOutOfRangeException();

                    _rotation = value;
                }
            }
        }



        [JsonPropertyName("scale")]
        public float[] Scale
        {
            get => _scale;
            set
            {
                if (value is null)
                    _scale = value;
                else
                {
                    if (value.Length < 3)
                        throw new ArgumentException("Array not long enough");

                    _scale = value.Length <= 3 ? value : throw new ArgumentException("Array too long");
                }
            }
        }



        [JsonPropertyName("translation")]
        public float[] Translation
        {
            get => _translation;
            set
            {
                if (value is null)
                    _translation = value;
                else
                {
                    if (value.Length < 3)
                        throw new ArgumentException("Array not long enough");

                    _translation = value.Length <= 3 ? value : throw new ArgumentException("Array too long");
                }
            }
        }



        [JsonPropertyName("weights")]
        public float[] Weights { get; set; }



        [JsonPropertyName("name")]
        public string Name { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }
       
    }


}
