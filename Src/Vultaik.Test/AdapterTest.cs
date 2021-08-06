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

            bool IsSupported = adapter.IsSupported;

            Assert.True(IsSupported, "Vulkan is not supported");
        }

        [Fact]
        public void Dispose_Adapter()
        {
            AdapterConfig config = new();
            Adapter adapter = new(config);

            Assert.False(adapter.IsNull, "Adapter is not disposed");

            adapter.Dispose();
            Assert.True(adapter.IsNull, "Adapter is disposed");

        }
    }
}
