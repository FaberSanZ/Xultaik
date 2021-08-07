using System;
using Xunit;

namespace Vultaik.Test
{
    public class AdapterTest
    {
        [Fact]
        public void CreateAdapter()
        {
            AdapterConfig config = new();
            using Adapter adapter = new(config);
            bool supported = adapter.IsSupported;

            Assert.True(supported, "Vulkan is not supported.");
        }

        [Fact]
        public void DisposeAdapter()
        {
            AdapterConfig config = new();
            Adapter adapter = new(config);

            Assert.False(adapter.IsNull, "Adapter is not disposed.");

            adapter.Dispose();
            Assert.True(adapter.IsNull, "Adapter is disposed.");

        }


        [Fact]
        public void SwapChainSupport()
        {
            AdapterConfig config = new();
            using Adapter adapter = new(config);
            bool support = adapter.SwapChain.Support;

            Assert.True(support, "SwapChain is not supported.");
        }
    }
}
