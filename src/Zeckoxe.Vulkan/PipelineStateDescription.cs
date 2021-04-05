// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	PipelineStateDescription.cs
=============================================================================*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Vortice.Vulkan;
using Zeckoxe.Core;

namespace Zeckoxe.Vulkan
{
    public static class VertexElementExt
    {
        public static int Size(this VertexType element)
        {
            switch (element)
            {
                case VertexType.Position: return 12;

                case VertexType.Normal: return 12;

                case VertexType.TextureCoordinate: return 8;

                case VertexType.Color :return 12;


                default: return 0;
            }
        }


        public static VkFormat ToPixelFormat(this VertexType element)
        {
            switch (element)
            {
                case VertexType.Position: return VkFormat.R32G32B32SFloat;

                case VertexType.Normal: return VkFormat.R32G32B32SFloat;

                case VertexType.TextureCoordinate: return VkFormat.R32G32SFloat;

                case VertexType.Color: return VkFormat.R32G32B32SFloat;

                default: return 0;
            }
        }


        
    }
    public class InputAssemblyState
    {
        public VkPrimitiveTopology PrimitiveType { get; set; }
        public bool PrimitiveRestartEnable { get; set; }

        public InputAssemblyState(VkPrimitiveTopology Type, bool RestartEnable = false)
        {
            PrimitiveType = Type;
            PrimitiveRestartEnable = RestartEnable;
        }

        public InputAssemblyState()
        {
            PrimitiveType = VkPrimitiveTopology.TriangleList;
            PrimitiveRestartEnable = false;
        }


        public static InputAssemblyState Default() => new InputAssemblyState()
        {
            PrimitiveRestartEnable = false,
            PrimitiveType = VkPrimitiveTopology.TriangleList
        };

    }

    public class MultisampleState
    {
        public VkSampleCountFlags MultisampleCount { get; set; }
        public bool SampleShadingEnable { get; set; }
        public float MinSampleShading { get; set; }
        public bool AlphaToCoverageEnable { get; set; }
        public bool AlphaToOneEnable { get; set; }


        public MultisampleState()
        {
            MultisampleCount = VkSampleCountFlags.Count1;
        }

    }


    public class RasterizationState
    {
        public bool DepthClampEnable { get; set; }
        public bool RasterizerDiscardEnable { get; set; }
        public VkPolygonMode FillMode { get; set; }
        public VkCullModeFlags CullMode { get; set; }
        public VkFrontFace FrontFace { get; set; }
        public bool DepthBiasEnable { get; set; }
        public float DepthBiasConstantFactor { get; set; }
        public float DepthBiasClamp { get; set; }
        public float DepthBiasSlopeFactor { get; set; }
        public float LineWidth { get; set; } = 1.0F;


        public static RasterizationState Default() => new RasterizationState()
        {
            FillMode = VkPolygonMode.Fill,
            CullMode = VkCullModeFlags.None,
            FrontFace = VkFrontFace.Clockwise,
        };
    }


    public class PipelineStateDescription
    {
        internal int VertexAttributeLocation = 0;
        internal int VertexAttributeOffset = 0;

        public PipelineStateDescription()
        {
            SetPrimitiveType(VkPrimitiveTopology.TriangleList);
            SetFillMode(VkPolygonMode.Fill);
            SetCullMode(VkCullModeFlags.None);
        }


        public Framebuffer Framebuffer { get; set; }

        public InputAssemblyState InputAssemblyState { get; set; } = new();

        public RasterizationState RasterizationState { get; set; } = new();

        public MultisampleState MultisampleState { get; set; } = new();

        public PipelineVertexInput PipelineVertexInput { get; set; } = new();

        public List<ShaderBytecode> Shaders { get; set; } = new();

        public List<DescriptorSetLayout> Layouts { get; set; } = new();



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
                VertexAttribute attribute = info.GetCustomAttribute<VertexAttribute>();

                if (attribute.Type is VertexType.Position)
                    SetVertexAttribute(VertexType.Position);


                if (attribute.Type is VertexType.Color)
                    SetVertexAttribute(VertexType.Color);


                if (attribute.Type is VertexType.TextureCoordinate)
                    SetVertexAttribute(VertexType.TextureCoordinate);


                if (attribute.Type is VertexType.Normal)
                    SetVertexAttribute(VertexType.Normal);
            }
        }

        public void SetFramebuffer(Framebuffer framebuffer)
        {
            Framebuffer = framebuffer;
        }

        public void SetCullMode(VkCullModeFlags mode)
        {
            RasterizationState.CullMode = mode;
        }

        public void SetFillMode(VkPolygonMode mode)
        {
            RasterizationState.FillMode = mode;
        }

        public void SetPrimitiveType(VkPrimitiveTopology type)
        {
            InputAssemblyState.PrimitiveType = type;
        }

        public void SetShader(ShaderBytecode bytecode)
        {
            if (bytecode.Data.Any())
            {
                Shaders.Add(bytecode);
            }
        }


        public void SetVertexBinding(VkVertexInputRate rate, int stride, int binding = 0)
        {
            PipelineVertexInput.VertexBindingDescriptions.Add(new()
            {
                Binding = binding,
                InputRate = rate,
                Stride = stride,
            });
        }
        public void SetVertexAttribute(VertexType element, int binding = 0)
        {
            PipelineVertexInput.VertexAttributeDescriptions.Add(new()
            {
                Binding = binding,
                Location = VertexAttributeLocation,
                Format = element.ToPixelFormat(),
                Offset = VertexAttributeOffset,
            });

            VertexAttributeLocation++;
            VertexAttributeOffset += element.Size();
        }


    }
}
