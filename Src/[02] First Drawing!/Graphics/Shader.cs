using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Device = SharpDX.Direct3D11.Device;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace _02__First_Drawing_
{
    public class Shader
    {
        public VertexShader VertexShader { get; set; }
        public PixelShader PixelShader { get; set; }
        public InputLayout Layout { get; set; }


        public bool Initialize(Device device, string vsFileName, string psFileName)
        {
            // Compile the vertex shader code.
            ShaderBytecode vertexShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName, "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);

            // Compile the pixel shader code.
            ShaderBytecode pixelShaderByteCode = ShaderBytecode.CompileFromFile(psFileName, "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);

            // Create the vertex shader from the buffer.
            VertexShader = new VertexShader(device, vertexShaderByteCode);
            // Create the pixel shader from the buffer.
            PixelShader = new PixelShader(device, pixelShaderByteCode);

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
            Layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);

            // Release the vertex and pixel shader buffers, since they are no longer needed.
            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();
            return true;

        }


        public void Render(DeviceContext deviceContext, int indexCount)
        {
            deviceContext.InputAssembler.InputLayout = Layout;

            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);

            // Draw the triangle
            deviceContext.Draw(indexCount, 0);
        }


        public void ShutDown()
        {
            // Release the layout.
            Layout?.Dispose();
            Layout = null;
            // Release the pixel shader.
            PixelShader?.Dispose();
            PixelShader = null;
            // Release the vertex shader.
            VertexShader?.Dispose();
            VertexShader = null;
        }
    }
}
