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

        /// <summary>
        /// Gets the buffer pointer.
        /// </summary>
        public byte[] Data { get; private set; }




        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderBytecode"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public ShaderBytecode(string path)
        {
            Data = File.ReadAllBytes(path);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderBytecode"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public ShaderBytecode(byte[] buffer)
        {
            Data = buffer;
        }



        public static byte[] LoadFromFile(string path) => new ShaderBytecode(path);



        /// <summary>
        /// Cast this <see cref="ShaderBytecode"/> to the underlying byte buffer.
        /// </summary>
        /// <param name="shaderBytecode"></param>
        /// <returns>A byte buffer</returns>
        public static implicit operator byte[](ShaderBytecode shaderBytecode) => shaderBytecode.Data;

    }
}
