// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vultaik
{
    public class FramebufferAttachmentDescription
    {

        public Image Target { get; set; }

        public uint ArrayLayer { get; set; }

        public uint MipLevel { get; set; }


        public FramebufferAttachmentDescription(Image target, uint arrayLayer)
            : this(target, arrayLayer, 0)
        { }


        public FramebufferAttachmentDescription(Image target, uint arrayLayer, uint mipLevel)
        {

            Target = target;
            ArrayLayer = arrayLayer;
            MipLevel = mipLevel;
        }
    }
}
