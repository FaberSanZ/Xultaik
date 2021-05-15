// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


/*=============================================================================
	Utils.cs
=============================================================================*/


using System.Numerics;

namespace Vultaik.GLTF
{

    public static partial class Utils
    {
        public static void ImportFloatArray(this Vector3 v, float[] floats)
        {
            if (floats.Length > 0)
            {
                v.X = floats[0];
            }

            if (floats.Length > 1)
            {
                v.Y = floats[1];
            }

            if (floats.Length > 2)
            {
                v.Z = floats[2];
            }
        }

    }
}
