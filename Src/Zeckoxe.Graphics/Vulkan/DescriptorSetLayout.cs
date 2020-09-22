// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

/*===================================================================================
	DescriptorSetLayout.cs
====================================================================================*/



namespace Zeckoxe.Graphics
{
    public class DescriptorSetLayout
    {
        public ShaderStage Stage { get; set; }
        public DescriptorType Type { get; set; }
        public int Binding { get; set; }

    }
}
