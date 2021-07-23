// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Vortice.Vulkan;


namespace Vultaik
{


    public class GraphicsPipelineDescription
    {
        internal int VertexAttributeLocation = 0;
        internal int VertexAttributeOffset = 0;

        public GraphicsPipelineDescription()
        {
            SetPrimitiveType(PrimitiveType.TriangleList);
            SetFillMode(FillMode.Solid);
            SetCullMode(CullMode.None);
        }


        public Framebuffer Framebuffer { get; set; }

        public InputAssemblyState InputAssemblyState { get; set; } = new();

        public RasterizationState RasterizationState { get; set; } = new();

        public MultisampleState MultisampleState { get; set; } = new();

        public PipelineVertexInput PipelineVertexInput { get; set; } = new();

        public List<ShaderBytecode> Shaders { get; set; } = new();




        internal ShaderStage stage_from_path(string path)
        {
            string ext = Path.GetExtension(path);

            if (path.EndsWith("vert"))
                return ShaderStage.Vertex;

            else if (ext == ".frag")
                return ShaderStage.Fragment;

            if (ext == ".vert")
                return ShaderStage.Vertex;

            else if (ext == ".tesc")
                return ShaderStage.TessellationControl;

            else if (ext == ".tese")
                return ShaderStage.TessellationEvaluation;

            else if (ext == ".geom")
                return ShaderStage.Geometry;

            else if (ext == ".comp")
                return ShaderStage.Compute;


            return ShaderStage.All;
        }

        public void SetProgram(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                SetShader(new(path, stage_from_path(path)));
            }
        }

        public void AddVertexAttribute<TVertex>()
        {
            IEnumerable<PropertyInfo> propertyInfos = typeof(TVertex).GetTypeInfo().GetRuntimeProperties();

            foreach(PropertyInfo info in propertyInfos)
            { 
                VertexAttribute? attribute = info.GetCustomAttribute<VertexAttribute>();

                if (attribute?.Type == VertexType.Position)
                    SetVertexAttribute(VertexType.Position);


                if (attribute?.Type == VertexType.Color)
                    SetVertexAttribute(VertexType.Color);


                if (attribute?.Type == VertexType.TextureCoordinate)
                    SetVertexAttribute(VertexType.TextureCoordinate);


                if (attribute?.Type == VertexType.Normal)
                    SetVertexAttribute(VertexType.Normal);
            }
        }

        public void SetFramebuffer(Framebuffer framebuffer)
        {
            Framebuffer = framebuffer;
        }

        public void SetCullMode(CullMode mode)
        {
            RasterizationState.CullMode = mode;
        }

        public void SetFillMode(FillMode mode)
        {
            RasterizationState.FillMode = mode;
        }

        public void SetPrimitiveType(PrimitiveType type)
        {
            InputAssemblyState.PrimitiveType = type;
        }

        public void SetShader(ShaderBytecode bytecode)
        {
            if (bytecode.Data.Any())
                Shaders.Add(bytecode);
            else
                Shaders.Add(new(new byte[] { 0, 255 }, ShaderStage.None, ShaderBackend.None));
        }


        public void SetVertexBinding(VertexInputRate rate, int stride, int binding = 0)
        {
            PipelineVertexInput.VertexBindingDescriptions.Add(new()
            {
                Binding = binding,
                InputRate = rate,
                Stride = stride,
            });
        }
        public void SetVertexAttribute(VertexType type, int binding = 0)
        {
            PipelineVertexInput.VertexAttributeDescriptions.Add(new()
            {
                Binding = binding,
                Location = VertexAttributeLocation,
                Format = type.ToPixelFormat(),
                Offset = VertexAttributeOffset,
            });

            VertexAttributeLocation++;
            VertexAttributeOffset += type.Size();

            //if(VertexAttributeLocation > Adapter.MaxVertexInputAttributes)
            //if(VertexAttributeOffset > Adapter.MaxVertexInputAttributeOffset)
            
        }


    }
}
