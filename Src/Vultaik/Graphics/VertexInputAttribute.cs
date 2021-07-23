// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using Vortice.Vulkan;

namespace Vultaik
{
    /// <summary>
    /// Specifying vertex input attribute.
    /// </summary>
    public class VertexInputAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexInputAttribute"/> attribute.
        /// </summary>
        public VertexInputAttribute()
        {
                
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexInputAttribute"/> attribute.
        /// </summary>
        /// <param name="location">The shader binding location number for this attribute.</param>
        /// <param name="binding">The binding number which this attribute takes its data from.</param>
        /// <param name="format">The size and type of the vertex attribute data.</param>
        /// <param name="offset">
        /// A byte offset of this attribute relative to the start of an element in the vertex input binding.
        /// </param>
        public VertexInputAttribute(int location, int binding, VkFormat format, int offset)
        {
            Location = location;
            Binding = binding;
            Format = format;
            Offset = offset;
        }


        /// <summary>
        /// The shader binding location number for this attribute.
        /// <para>Must be less than <see cref="Adapter.MaxVertexInputAttributes"/>.</para>
        /// </summary>
        public int Location { get; set; }


        /// <summary>
        /// The binding number which this attribute takes its data from.
        /// <para>Must be less than <see cref="Adapter.MaxVertexInputBindings"/>.</para>
        /// </summary>
        public int Binding { get; set; }



        public VkFormat Format { get; set; }

        /// <summary>
        /// A byte offset of this attribute relative to the start of an element in the vertex input binding.
        /// <para>Must be less than or equal to <see cref="Adapter.MaxVertexInputAttributeOffset"/>.</para>
        /// </summary>
        public int Offset { get; set; }
    }
}
