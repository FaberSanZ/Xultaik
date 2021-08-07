using Xunit;

namespace Vultaik.Test
{
    public class AdapterTest
    {
        [Fact]
        public void Create()
        {
            AdapterConfig config = new();
            using Adapter adapter = new(config);
            bool supported = adapter.IsSupported;

            Assert.True(supported, "Vulkan is not supported.");
        }

        [Fact]
        public void Dispose()
        {
            AdapterConfig config = new();
            Adapter adapter = new(config);

            Assert.False(adapter.IsNull, "Adapter is not disposed.");

            adapter.Dispose();
            Assert.True(adapter.IsNull, "Adapter is not disposed.");
        }

        [Fact]
        public void SwapChainSupport()
        {
            AdapterConfig config = new();
            using Adapter adapter = new(config);
            bool support = adapter.SwapChain.Support;
            string name = adapter.SwapChain.Name;

            Assert.True(support, $"SwapChain is not supported. {name}");
        }


        //[Fact]
        //public void MultisampleQualityLevels()
        //{
        //    AdapterConfig config = new();
        //    using Adapter adapter = new(config);
        //    var sample = adapter.MultisampleCount;

        //    Assert.True(sample != Vortice.Vulkan.VkSampleCountFlags.None, "Multisample is not supported.");
        //}
    }
}
