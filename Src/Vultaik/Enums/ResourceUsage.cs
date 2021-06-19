// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




namespace Vultaik
{
    public enum ResourceUsage
    {
        Default = unchecked(0),

        Immutable = unchecked(1),

        Dynamic = unchecked(2),

        Staging = unchecked(3),

        DynamicUniform = unchecked(4),


		/// <summary>
		/// No intended memory usage specified.
		/// </summary>
		Unknown = 0,


		/// <summary>
		/// Memory will be used on device only, no need to be mapped on host.
		/// </summary>
		GPU_Only = 1,


		/// <summary>
		/// Memory will be mapped on host. Could be used for transfer to device.
		/// </summary>
		CPU_Only = 2,


		/// <summary>
		/// Memory will be used for frequent (dynamic) updates from host and reads on device.
		/// </summary>
		CPU_To_GPU = 3,


		/// <summary>
		/// Memory will be used for writing on device and readback on host.
		/// </summary>
		GPU_To_CPU = 4,
	}
}