// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	MaterialPbrMetallicRoughness.cs
=============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF.Schema
{
    public class MaterialPbrMetallicRoughness
    {
        private float[] _baseColorFactor = new[]
        {
            1f,1f,1f,1f
        };


        [JsonPropertyName("baseColorFactor")]
        public float[] BaseColorFactor
        {
            get => _baseColorFactor;
            set
            {
                if (value is null)
                    _baseColorFactor = value;
                else
                {
                    if (value.Length < 4)
                        throw new ArgumentException("Array not long enough");

                    if (value.Length > 4)
                        throw new ArgumentException("Array too long");

                    for (int index = 0; index < value.Length; ++index)
                        if (value[index] < 0.0)
                            throw new ArgumentOutOfRangeException();

                    for (int index = 0; index < value.Length; ++index)
                        if (value[index] > 1.0)
                            throw new ArgumentOutOfRangeException();

                    _baseColorFactor = value;
                }
            }
        }



        [JsonPropertyName("baseColorTexture")]
        public TextureInfo BaseColorTexture { get; set; }



        [JsonPropertyName("metallicFactor")]
        public float MetallicFactor { get; set; }



        [JsonPropertyName("roughnessFactor")]
        public float RoughnessFactor { get; set; }



        [JsonPropertyName("metallicRoughnessTexture")]
        public TextureInfo MetallicRoughnessTexture { get; set; }



        [JsonPropertyName("extensions")]
        public Dictionary<string, object> Extensions { get; set; }



        [JsonPropertyName("extras")]
        public Extras Extras { get; set; }




        public bool ShouldSerializeBaseColorFactor() => !((IEnumerable<float>)_baseColorFactor).SequenceEqual<float>(new[] { 1f, 1f, 1f, 1f });
        

    }
}
