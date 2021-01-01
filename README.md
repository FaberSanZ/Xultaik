
<h1 align="center">
   Zeckoxe Engine
  
  ##               Zeckoxe Engine is a 2D/3D renderer implemented in [Vulkan®](https://www.khronos.org/vulkan/)
  
</h1>

<hr>


<br>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/LICENSE)

The code is licensed under MIT. Feel free to use it for whatever purpose.

<hr>
<br>



## Low-level rendering backend

The rendering backend is completely focused on Vulkan, however the Engine greatly simplifies the most painful points of writing Vulkan. 
<br>

The engine is designed to test and investigate graphics, physics and GPGPU techniques, including games. 
Games is not the real motivation behind this project.


## Features:

- [ ] Ray Tracing -><br>
		- VK_KHR_acceleration_structure <br>
		- VK_KHR_ray_tracing_pipeline <br>
		- VK_KHR_ray_query <br>
		- VK_KHR_pipeline_library <br>
- [ ] Conservative rasterization (VK_EXT_conservative_rasterization)
- [ ] Push descriptors (VK_KHR_push_descriptor)
- [ ] Inline uniform blocks (VK_EXT_inline_uniform_block)
- [ ] Multiview rendering (VK_KHR_multiview)
- [ ] Conditional rendering (VK_EXT_conditional_rendering)
- [ ] Variable rate shading (VK_NV_shading_rate_image)
- [ ] VK_EXT_descriptor_indexing 
- [ ] VK_KHR_timeline_semaphore 
- [ ] VK_EXT_robustness2 
- [ ] VK_KHR_buffer_device_address 
- [ ] VK_EXT_extended_dynamic_state 
- [ ] VK_KHR_deferred_host_operations
- [ ] VK_KHR_device_group 
- [ ] VK_KHR_device_group_creation 
- [ ] VK_KHR_external_fence 
- [ ] VK_KHR_external_memory 
- [ ] VK_KHR_external_semaphore 
- [ ] VK_KHR_sampler_ycbcr_conversion 
- [ ] VK_KHR_imageless_framebuffer 
- [ ] VK_EXT_memory_priority 
- [ ] VK_KHR_draw_indirect_count
- [ ] VK_EXT_inline_uniform_block 
- [ ] Memory allocator (VMA)
- [ ] Compute Shader
- [ ] Geometry Shader
- [ ] G-Buffer
- [ ] Physically-Based Rendering
- [ ] Instancing, Indirect drawing
- [ ] Tessellation (Phong, PN Triangles, and Displacement Mapping)
- [ ] Post-AA (FXAA, SMAA and TAA)
- [x] Vulkan GLSL for shaders, shaders are compiled in runtime with shaderc
- [ ] Multithreaded rendering
- [x] GLTF 2.0


<hr>
<br>


## Low Level API Examples

| Example   | Screenshot  | Description          |
|---------------|-------------|----------------------|
| [Clear Screen](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/ClearScreen.cs) | <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/ClearScreen.PNG" width=350> | This example shows how to configure the device and clear the color. |
| [Triangle](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Triangle.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Triangle.PNG" width=350> | This example shows how to render simple triangle.  |
| [Transformations](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Transformations.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Transformations.PNG" width=350> | We will transform the world space for each object (the two cubes) using transformation matrices.  |
| [LoadGLTF](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadGLTF.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadGLTF.PNG" width=350> | Load GLTF 3D Model.  |
| [LoadTexture](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadTexture.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadTexture.PNG" width=350> | Loads a 2D texture from disk (including all mip levels), uses staging to upload it into video memory.  |
| [Lighting](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Lighting.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Lighting.PNG" width=350> | Basic Lighting.  |


<hr>
<br>


## Research or code used:
- Granite (<https://github.com/Themaister/Granite>)
- Vulkan examples from Sascha Willems (<https://github.com/SaschaWillems/Vulkan>)
- Vortice.Vulkan (<https://github.com/amerkoleci/Vortice.Vulkan>)
- DefaultEcs (<https://github.com/Doraku/DefaultEcs>)
- Silk.NET (<https://github.com/Ultz/Silk.NET>)




