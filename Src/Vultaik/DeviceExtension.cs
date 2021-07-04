// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

namespace Vultaik
{
    public class DeviceExtension
    {
        internal bool adapter_config;
        internal bool implement => adapter_config && Support;

        public DeviceExtension()
        {

        }

        internal DeviceExtension(string name, bool support, bool config)
        {
            Name = name;
            Support = support;
            adapter_config = config;
        }

        public DeviceExtension(string name, bool support)
        {
            Name = name;
            Support = support;
        }

        public string Name { get; set; }
        public bool Support { get; set; }


    }
}
