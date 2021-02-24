
<h1 align="center">
   Zeckoxe Engine
  
  ##               Zeckoxe is a 2D/3D renderer implemented in [Vulkan®](https://www.khronos.org/vulkan/)
  
</h1>

<hr>


<br>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/LICENSE)

The code is licensed under MIT. Feel free to use it for whatever purpose.

<hr>
<br>



## Low-level rendering backend

The rendering backend focuses entirely on Vulkan, so it reuses Vulkan enums and data structures where appropriate. However, the API greatly simplifies the more painful points of writing straight Vulkan. It's not designed to be the fastest renderer ever made, it's likely a happy middle ground between "perfect" Vulkan and OpenGL/D3D11 w.r.t. CPU overhead.
<br>


## Features:

- [ ] Ray Tracing  VK_KHR_acceleration_structure (VK_KHR_ray_tracing_pipeline, VK_KHR_ray_query, VK_KHR_pipeline_library)
- [ ] Conservative rasterization (VK_EXT_conservative_rasterization)
- [ ] Inline uniform blocks (VK_EXT_inline_uniform_block)
- [ ] Multiview rendering (VK_KHR_multiview)
- [ ] Conditional rendering (VK_EXT_conditional_rendering)
- [ ] Shading rate (VK_KHR_fragment_shading_rate) 
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
- [ ] VK_EXT_memory_priority 
- [ ] VK_KHR_draw_indirect_count
- [ ] VK_EXT_inline_uniform_block 
- [ ] VK_KHR_8bit_storage 
- [ ] Memory allocator (VMA)
- [ ] Compute Shader
- [ ] Geometry Shader
- [ ] Tessellation Shader (Phong, PN Triangles, and Displacement Mapping)
- [ ] G-Buffer
- [ ] Physically-Based Rendering
- [ ] Instancing, Indirect drawing
- [ ] Post-AA (FXAA, SMAA and TAA)
- [ ] Multithreaded rendering
- [x] Automatic descriptor set management
- [x] Vulkan GLSL for shaders, shaders are compiled in runtime with shaderc
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
| [EntityComponentSystem ](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/ECS.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/ECS.PNG" width=350> | Basic EntityComponentSystem .  |


<hr>
<br>


## Research or code used:
- Granite (<https://github.com/Themaister/Granite>)
- Vulkan examples from Sascha Willems (<https://github.com/SaschaWillems/Vulkan>)
- Vortice.Vulkan (<https://github.com/amerkoleci/Vortice.Vulkan>)
- Silk.NET (<https://github.com/Ultz/Silk.NET>)




