using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Graphics
{
    public class Shaders
    {
        public VertexShader VertexShader { get; set; }

        public PixelShader PixelShader { get; set; }

        public InputLayout Layout { get; set; }




        public Shaders(GraphicsDevice Device, string VSFileName, string PSFileName)
        {
            // Compile the vertex shader code.
            byte[] vertexShaderByteCode = ShaderBytecode.CompileFromFile(VSFileName, "VS", "vs_5_0", ShaderFlags.None, EffectFlags.None);

            // Compile the pixel shader code.
            byte[] pixelShaderByteCode = ShaderBytecode.CompileFromFile(PSFileName, "PS", "ps_5_0", ShaderFlags.None, EffectFlags.None);

            // Create the vertex shader from the buffer.
            VertexShader = new VertexShader(Device.NativeDevice, vertexShaderByteCode);

            // Create the pixel shader from the buffer.
            PixelShader = new PixelShader(Device.NativeDevice, pixelShaderByteCode);

            // Now setup the layout of the data that goes into the shader.
            // This setup needs to match the VertexType structure in the Model and in the shader.
            InputElement[] inputElements = new InputElement[]
            {
                new InputElement()
                {
                    SemanticName = "POSITION",
                    SemanticIndex = 0,
                    Format = Format.R32G32B32_Float,
                    Slot = 0,
                    AlignedByteOffset = 0,
                    Classification = InputClassification.PerVertexData,
                    InstanceDataStepRate = 0
                }
            };

            // Create the vertex input the layout.
            Layout = new InputLayout(Device.NativeDevice, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);

        }
    }
}
