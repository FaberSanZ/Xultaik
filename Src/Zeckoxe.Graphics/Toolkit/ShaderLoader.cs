// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	ShaderLoader.cs
=============================================================================*/



using Zeckoxe.ShaderCompiler;

namespace Zeckoxe.Graphics.Toolkit
{
    public class ShaderLoader
    {
        public static ShaderBytecode LoadFromFile(string path, Stage stage)
        {
            return new ShaderBytecode(Compiler.LoadFromFile(path, stage))
            {

            };
        }
    }
}
