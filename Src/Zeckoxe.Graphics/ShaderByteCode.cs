// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	ShaderBytecode.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Zeckoxe.Graphics
{
    public class ShaderBytecode
    {

        public byte[] Data { get; private set; }

        public ShaderBytecode(string path)
        {
            Data = File.ReadAllBytes(path);
        }

        public ShaderBytecode(byte[] buffer)
        {
            Data = buffer;
        }



        public static byte[] LoadFromFile(string path) => new ShaderBytecode(path);


        public static implicit operator byte[](ShaderBytecode shaderBytecode) => shaderBytecode.Data;

    }
}
