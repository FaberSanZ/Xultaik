
<h1 align="center">
   Zeckoxe
  
  ##               Zeckoxe is a 2D/3D renderer implemented in [Vulkan®](https://www.khronos.org/vulkan/)
  
</h1>

<hr>



### Zecoxe can be used to produce render layers for custom engines or next-gen games and provides building blocks for writing your own engine or game quickly

<br>

## Low-level rendering backend

The rendering backend focuses entirely on Vulkan, so it reuses Vulkan enums and data structures where appropriate. However, the API greatly simplifies the more painful points of writing straight Vulkan. Modern Vulkan extensions and features are aggressively made use of to improve performance


<br>
<br>

> :warning: The Zeckoxe.Audio librarie are optional and you will need to download the FMOD native binaries from https://www.fmod.com/download


<br>
<br>

## Features and extensions:

- [ ] Ray Tracing (VK_KHR_acceleration_structure)
- [ ] Ray Tracing (VK_KHR_ray_tracing_pipeline)
- [ ] Ray Tracing (VK_KHR_ray_query)
- [ ] Ray Tracing (VK_KHR_pipeline_library
- [ ] Conservative rasterization (VK_EXT_conservative_rasterization)
- [ ] Inline uniform blocks (VK_EXT_inline_uniform_block)
- [ ] Multiview rendering (VK_KHR_multiview)
- [ ] Conditional rendering (VK_EXT_conditional_rendering)
- [ ] Shading rate (VK_KHR_fragment_shading_rate) 
- [ ] Descriptor indexing (VK_EXT_descriptor_indexing) 
- [ ] Timeline semaphore (VK_KHR_timeline_semaphore) 
- [ ] Buffer device address (VK_KHR_buffer_device_address) 
- [ ] Synchronization2 (VK_KHR_synchronization2)
- [x] VK_KHR_uniform_buffer_standard_layout  (Note: Not functional-Experimental)
- [ ] VK_EXT_robustness2 
- [ ] VK_EXT_extended_dynamic_state 
- [ ] VK_KHR_deferred_host_operations
- [ ] VK_KHR_device_group_creation 
- [ ] VK_KHR_device_group 
- [ ] VK_KHR_external_fence 
- [ ] VK_KHR_external_memory 
- [ ] VK_KHR_external_semaphore 
- [ ] VK_KHR_sampler_ycbcr_conversion 
- [ ] VK_EXT_memory_priority 
- [ ] VK_KHR_draw_indirect_count
- [ ] VK_EXT_inline_uniform_block 
- [ ] VK_KHR_8bit_storage 
- [ ] VK_KHR_16bit_storage 
- [ ] Memory allocator
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


## Examples

| Example   | Screenshot  | Description          |
|---------------|-------------|----------------------|
| [Clear Screen](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/ClearScreen.cs) | <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/ClearScreen.PNG" width=350> | This example shows how to configure the device and clear the color. |
| [Triangle](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Triangle.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Triangle.PNG" width=350> | This example shows how to render simple triangle.  |
| [Transformations](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Transformations.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Transformations.PNG" width=350> | We will transform the world space for each object (the two cubes) using transformation matrices.  |
| [LoadGLTF](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadGLTF.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadGLTF.PNG" width=350> | Load GLTF 3D Model.  |
| [LoadTexture](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadTexture.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadTexture.PNG" width=350> | Loads a 2D texture from disk (including all mip levels).  |
| [Lighting](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Lighting.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Lighting.PNG" width=350> | Basic Lighting.  |


<hr>
<br>


## The project is inspired by and based on:
Granite (<https://github.com/Themaister/Granite>)



<br>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/FakzSoft/Zeckoxe/blob/master/LICENSE)

The code is licensed under MIT. Feel free to use it for whatever purpose.
If you have any commercial use for this project which needs further development to accomplish, I might be available for contracting work. Contact me by [Twitter](https://twitter.com/FaberSan_Z).

<hr>

