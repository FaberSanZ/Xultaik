// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vortice.Dxc;

namespace Xultaik.Graphics
{
    public class ShaderByteCode
    {

        public byte[] Data = Array.Empty<byte>();


        public ShaderStage ShaderStage { get; set; }

        public ShaderModel ShaderModel { get; set; }

        public string Source { get; set; }

        public string EntryPoint { get; set; }

        public ShaderByteCode()
        {

        }


        public ShaderByteCode(string source, ShaderStage stage, string entrypoint, ShaderModel shaderModel)
        {
            Source = source;
            ShaderStage = stage;
            EntryPoint = entrypoint;
            ShaderModel = shaderModel;

            Recreate();
        }


        internal void Recreate()
        {
            DxcCompilerOptions options = new DxcCompilerOptions()
            {
                ShaderModel = ConvertExtensions.ToDxcShaderModel(ShaderModel),
                
            };


            //IDxcOperationResult result = DxcCompiler.Compile(ConvertExtensions.ToDxcShaderStage(ShaderStage), Source, EntryPoint, "", options);

            //Data = Dxc.GetBytesFromBlob(result.GetResult());
        }


        public static ShaderByteCode CompileFromFile(string fileName, ShaderStage stage, string entrypoint = "", ShaderModel shaderModel = ShaderModel.Model6_0)
        {
            if (string.IsNullOrEmpty(entrypoint))
                entrypoint = ConvertExtensions.GetDefaultEntryPoint(stage);


            return new ShaderByteCode(File.ReadAllText(fileName), stage, entrypoint, shaderModel);
        }



        public static implicit operator byte[](ShaderByteCode value) => value.Data;

    }
}
