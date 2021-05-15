// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vultaik
{
    public class FramebufferDescription
    {

        public FramebufferAttachmentDescription? DepthTarget { get; set; }

        public FramebufferAttachmentDescription[] ColorTargets { get; set; }





        public FramebufferDescription(Image? depthTarget, params Image[] colorTargets)
        {
            if (depthTarget is not null)
                DepthTarget = new FramebufferAttachmentDescription(depthTarget, 0);
            else
                DepthTarget = null;

            ColorTargets = new FramebufferAttachmentDescription[colorTargets.Length];

            for (int i = 0; i < colorTargets.Length; i++)
                ColorTargets[i] = new FramebufferAttachmentDescription(colorTargets[i], 0);
        }




        public FramebufferDescription(FramebufferAttachmentDescription? depthTarget, FramebufferAttachmentDescription[] colorTargets)
        {
            DepthTarget = depthTarget;
            ColorTargets = colorTargets;
        }
    }
}
