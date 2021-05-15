
<h1 align="center">
   Zeckoxe
  
  ##               Vultaik is a 2D/3D renderer implemented in [Vulkan®](https://www.khronos.org/vulkan/)
  
</h1>

<hr>

[![Build status](https://github.com/FaberSanZ/Zeckoxe/workflows/ci/badge.svg)](https://github.com/FaberSanZ/Vultaik/actions)
[![NuGet](https://img.shields.io/nuget/v/Zeckoxe.Vulkan.svg)](https://www.nuget.org/profiles/FaberSanZ)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/FaberSanZ/Vultaik/blob/master/LICENSE) 

The code is licensed under MIT. Feel free to use it for whatever purpose.

<hr>

## Overview
Vultaik can be used to produce render layers for custom engines or next-gen games and provides building blocks for writing your own engine or game quickly.
Due to the parallel nature of GPUs, Vultaik can use [GPGPU](https://en.wikipedia.org/wiki/General-purpose_computing_on_graphics_processing_units) to exploit a GPU for computational tasks. Supports ComputePipeline and more, allowing to be used for general computations


<br>

## Low-level rendering backend

The rendering backend focuses entirely on Vulkan, so it reuses Vulkan enums where appropriate. However, the API greatly simplifies the more painful points of writing straight Vulkan. Modern Vulkan [extensions](https://github.com/FaberSanZ/Zeckoxe/wiki/Extension) and [features](#Features) are aggressively made use of to improve performance


<br>
<br>

## Features:

- [ ] KHR Ray Tracing
- [ ] Conservative rasterization
- [ ] Multiview rendering
- [ ] Conditional rendering 
- [ ] Shading rate
- [ ] Descriptor indexing
- [ ] Timeline semaphore
- [ ] Buffer device address 
- [ ] Synchronization
- [x] Memory allocator
- [x] GPU-Assisted Validation
- [ ] Compute Shader
- [ ] Geometry Shader
- [ ] Tessellation Shader (Phong, PN Triangles, and Displacement Mapping)
- [ ] G-Buffer
- [ ] Physically-Based Rendering
- [ ] Instancing, Indirect drawing
- [ ] Post-AA (FXAA, SMAA and TAA)
- [ ] Multithreaded rendering
- [x] Automatic descriptor set management
- [x] SPIRV reflection with [SPIRV-Cross](https://github.com/KhronosGroup/SPIRV-Cross)
- [x] Vulkan GLSL for shaders, shaders are compiled in runtime with [Shaderc](https://github.com/google/shaderc)
- [x] Vulkan HLSL for shaders, shaders are compiled in runtime with [DirectXShaderCompiler](https://github.com/microsoft/DirectXShaderCompiler)
- [x] GLTF 2.0

<hr>
<br>


## Examples


Example | Details
---------|--------
<img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/ClearScreen.PNG" width=350> | [ClearScreen](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/ClearScreen.cs)<br> This example shows how to configure the device and clear the color.
<img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Triangle.PNG" width=350> | [Triangle](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Triangle.cs)<br> This example shows how to render simple triangle.
<img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Transformations.PNG" width=350> | [Transformations](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Transformations.cs)<br> This example how to transform the world space for each object.
<img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadGLTF.PNG" width=350> | [LoadGLTF](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadGLTF.cs)<br> This example shows how to load GLTF models.
<img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadTexture.PNG" width=350> | [LoadTexture](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadTexture.cs)<br> This example shows how to load 2D texture from disk (including all mip levels). 
<img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Lighting.PNG" width=350> | [Lighting](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Lighting.cs)<br> This example shows how to create basic lighting. 



<hr>
<br>


## The project is inspired by and based on:
The-Forge (<https://github.com/ConfettiFX/The-Forge>)



<br>


Additionally, **Zeckoxe** uses NuGet packages from the following repositories:

- [Vortice.Vulkan](https://github.com/amerkoleci/Vortice.Vulkan)
- [Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows)
- [Silk.NET](https://github.com/dotnet/Silk.NET)


