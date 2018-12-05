using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D11;
using System.Collections.Generic;

namespace _12___Iluminación_Difusa
{
    public class Shaders
    {
        // Structures.
        [StructLayout(LayoutKind.Sequential)]
        public struct MatrixBuffer
        {
            public Matrix WVP;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LightBuffer
        {
            public Vector4 diffuseColor;
            public Vector3 lightDirection;
            public float padding; // Added extra padding so structure is a multiple of 16.
        }

        // Properties.
        public InputLayout Layout { get; set; }
        public Buffer ConstantMatrixBuffer { get; set; }
        public SamplerState SamplerState { get; set; }
        public Buffer ConstantLightBuffer { get; private set; }
        public SamplerState SampleState { get; set; }

        public Dictionary<string, ShaderBytecode> ShaderByte = new Dictionary<string, ShaderBytecode>();



        // Constructor
        public Shaders() { }

        // Methods.
        public bool Initialize(Device device)
        {
            // Initialize the vertex and pixel shaders.
            return InitializeShader(device, @"Shaders/VertexShader.hlsl", @"Shaders/PixelShader.hlsl");
        }
        private bool InitializeShader(Device device, string vsFileName, string psFileName)
        {
            try
            {

                ShaderByte["VS"] = ShadersUtil.CompileShader("Shaders/VertexShader.hlsl", "VS", "vs_4_0");
                ShaderByte["PS"] = ShadersUtil.CompileShader("Shaders/PixelShader.hlsl", "PS", "ps_4_0");


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
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 0,
                        Format = SharpDX.DXGI.Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = InputElement.AppendAligned,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement()
                    {
                        SemanticName = "NORMAL",
                        SemanticIndex = 0,
                        Format = SharpDX.DXGI.Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = InputElement.AppendAligned,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };

                // Create the vertex input the layout. Kin dof like a Vertex Declaration.
                Layout = new InputLayout(device, ShaderSignature.GetInputSignature(ShaderByte["VS"].Data), inputElements);

                // Release the vertex and pixel shader buffers, since they are no longer needed.
                ShaderByte["VS"].Dispose();
                ShaderByte["PS"].Dispose();

                // Setup the description of the dynamic matrix constant Matrix buffer that is in the vertex shader.
                BufferDescription matrixBufferDescription = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<MatrixBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
                ConstantMatrixBuffer = new Buffer(device, matrixBufferDescription);

                // Create a texture sampler state description.
                SamplerStateDescription samplerDesc = new SamplerStateDescription()
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    MipLodBias = 0,
                    MaximumAnisotropy = 1,
                    ComparisonFunction = Comparison.Always,
                    BorderColor = new Color4(0, 0, 0, 0),  // Black Border.
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue
                };

                // Create the texture sampler state.
                SamplerState = new SamplerState(device, samplerDesc);


                // Setup the description of the light dynamic constant bufffer that is in the pixel shader.
                // Note that ByteWidth alwalys needs to be a multiple of the 16 if using D3D11_BIND_CONSTANT_BUFFER or CreateBuffer will fail.
                BufferDescription lightBufferDesc = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<LightBuffer>(), // Must be divisable by 16 bytes, so this is equated to 32.
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
                ConstantLightBuffer = new SharpDX.Direct3D11.Buffer(device, lightBufferDesc);

                return true;
            }
            catch (Exception ex)
            {
               //MessageBox.Show("Error initializing shader. Error is " + ex.Message);
                return false;
            }
        }
        public void ShutDown()
        {
            // Shutdown the vertex and pixel shaders as well as the related objects.
            ShuddownShader();
        }
        private void ShuddownShader()
        {
            // Release the matrix constant buffer.
            ConstantMatrixBuffer?.Dispose();
            ConstantMatrixBuffer = null;
            // Release the layout.
            Layout?.Dispose();
            Layout = null;
        }

        public int bufferPositionNumber = 0;


        public bool SetShaderParameters(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture)
        {
            try
            {
                // Transpose the matrices to prepare them for shader.

                // Lock the constant buffer so it can be written to.
                DataStream mappedResource;
                deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

                // Copy the matrices into the constant buffer.
                MatrixBuffer matrixBuffer = new MatrixBuffer()
                {
                    WVP = Matrix.Transpose(worldMatrix * viewMatrix * projectionMatrix)
                };
                mappedResource.Write(matrixBuffer);

                // Unlock the constant buffer.
                deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

                // Set the position of the constant buffer in the vertex shader.

                // Finally set the constant buffer in the vertex shader with the updated values.
                deviceContext.VertexShader.SetConstantBuffer(bufferPositionNumber, ConstantMatrixBuffer);

                // Set shader resource in the pixel shader.
                deviceContext.PixelShader.SetShaderResource(0, texture);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CreateLight(DeviceContext deviceContext, Vector3 lightDirection, Vector4 diffuseColour)
        {
            DataStream mappedResource;

            // Lock the light constant buffer so it can be written to.
            deviceContext.MapSubresource(ConstantLightBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

            // Copy the lighting variables into the constant buffer.
            LightBuffer lightBuffer = new LightBuffer()
            {
                diffuseColor = diffuseColour,
                lightDirection = lightDirection,
                padding = 0
            };

            mappedResource.Write(lightBuffer);

            // Unlock the constant buffer.
            deviceContext.UnmapSubresource(ConstantLightBuffer, 0);

            // Set the position of the light constant buffer in the pixel shader.
            bufferPositionNumber = 0;

            // Finally set the light constant buffer in the pixel shader with the updated values.
            deviceContext.PixelShader.SetConstantBuffer(bufferPositionNumber, ConstantLightBuffer);
        }

        public void RenderShader(DeviceContext deviceContext, int indexCount)
        {
            // Set the vertex input layout.
            deviceContext.InputAssembler.InputLayout = Layout;

            // Set the vertex and pixel shaders that will be used to render this triangle.
            deviceContext.VertexShader.Set(new VertexShader(deviceContext.Device, ShaderByte["VS"]));
            deviceContext.PixelShader.Set(new PixelShader(deviceContext.Device, ShaderByte["PS"]));

            // Set the sampler state in the pixel shader.
            deviceContext.PixelShader.SetSampler(0, SampleState);


            // Render the triangle.
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
    }




    public static class ShadersUtil
    {
        public static ShaderBytecode CompileShader(string FileName, string EntryPoint, string Profile, ShaderMacro[] defines = null)
        {
            ShaderFlags shaderFlags = ShaderFlags.None;

            shaderFlags |= ShaderFlags.Debug | ShaderFlags.SkipOptimization;

            CompilationResult result = ShaderBytecode.CompileFromFile(FileName, EntryPoint, Profile, shaderFlags, defines: defines);

            return new ShaderBytecode(result);
        }

    }
}
