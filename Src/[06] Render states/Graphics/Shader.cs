using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _06__Render_states
{
    public class Shader
    {
        public VertexShader VertexShader { get; set; }
        public PixelShader PixelShader { get; set; }
        public InputLayout Layout { get; set; }
        public Buffer ConstantMatrixB { get; set; }

        public void Initialize(Device device, string vsFileName, string psFileName)
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
                        Format = SharpDX.DXGI.Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement()
                    {
                        SemanticName = "COLOR",
                        SemanticIndex = 0,
                        Format = SharpDX.DXGI.Format.R32G32B32A32_Float,
                        Slot = 0,
                        AlignedByteOffset = InputElement.AppendAligned,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
            };

            // Create the vertex input the layout.
            Layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);

            // Release the vertex and pixel shader buffers, since they are no longer needed.
            vertexShaderByteCode.Dispose();
            pixelShaderByteCode.Dispose();

            // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
            BufferDescription MBufDesc = new BufferDescription();
            MBufDesc.Usage = ResourceUsage.Dynamic;
            MBufDesc.SizeInBytes = SharpDX.Utilities.SizeOf<MatrixB>(); // was Matrix
            MBufDesc.BindFlags = BindFlags.ConstantBuffer;
            MBufDesc.CpuAccessFlags = CpuAccessFlags.Write;
            MBufDesc.OptionFlags = ResourceOptionFlags.None;
            MBufDesc.StructureByteStride = 0;

            // Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
            ConstantMatrixB = new Buffer(device, MBufDesc);
        }

        public void SetParameters(DeviceContext deviceContext, Matrix World, Matrix View, Matrix Projection)
        {

            // Transpose the matrices to prepare them for shader.
            World.Transpose();
            View.Transpose();
            Projection.Transpose();

            // Lock the constant buffer so it can be written to.
            DataStream mappedResource;
            deviceContext.MapSubresource(ConstantMatrixB, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

            // Copy the matrices into the constant buffer.
            MatrixB MatrixBuffer = new MatrixB();
            MatrixBuffer.World = World;
            MatrixBuffer.View = View;
            MatrixBuffer.Projection = Projection;

            mappedResource.Write<MatrixB>(MatrixBuffer);

            // Unlock the constant buffer.
            deviceContext.UnmapSubresource(ConstantMatrixB, 0);

            // Set the position of the constant buffer in the vertex shader.
            int Slot = 0;

            // Finally set the constant buffer in the vertex shader with the updated values.
            deviceContext.VertexShader.SetConstantBuffer(Slot, ConstantMatrixB);
        }

        public void Render(DeviceContext deviceContext, int indexCount)
        {
            deviceContext.InputAssembler.InputLayout = Layout;

            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);

            // Draw the triangle
            deviceContext.DrawIndexed(indexCount, 0, 0);
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
