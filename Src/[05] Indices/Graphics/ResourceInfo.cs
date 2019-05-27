using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public class ResourceInfo
    {
        public BindFlags BindFlags { get; internal set; }
        public CpuAccessFlags AccessFlag { get; internal set; }
        public ResourceUsage UsageType { get; internal set; }


        public ResourceInfo(BindFlags bindFlags, CpuAccessFlags accessFlag, ResourceUsage usageType)
        {
            BindFlags = bindFlags;
            AccessFlag = accessFlag;
            UsageType = usageType;
        }


        public static ResourceInfo VertexBuffer => new ResourceInfo(BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceUsage.Default);

        public static ResourceInfo IndexBuffer => new ResourceInfo(BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceUsage.Default);

    }
}
