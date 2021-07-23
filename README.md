



<h1 align="center">
   <img src="Logo/vultaik-logo.png" width=410>

  
  ##               Vultaik is a 2D/3D renderer implemented in [Vulkan®](https://www.khronos.org/vulkan/)
  
</h1>


[![Build status](https://github.com/FaberSanZ/Zeckoxe/workflows/ci/badge.svg)](https://github.com/FaberSanZ/Vultaik/actions)
[![NuGet](https://img.shields.io/nuget/v/Vultaik.svg)](https://www.nuget.org/packages?q=Tags%3A%22Vultaik%22)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/FaberSanZ/Vultaik/blob/master/LICENSE) 

<br>

## Overview
Vultaik can be used to produce render layers for custom engines or next-gen games and provides building blocks for writing your own engine or game quickly.
Due to the parallel nature of GPUs, Vultaik can use [GPGPU](https://en.wikipedia.org/wiki/General-purpose_computing_on_graphics_processing_units) to exploit a GPU for computational tasks. Supports ComputePipeline and more, allowing to be used for general computations


<br>

## Low-level rendering backend

The rendering backend focuses entirely on Vulkan and HLSL, however, the API greatly simplifies the more painful points of writing straight Vulkan. Modern Vulkan [extensions](https://github.com/FaberSanZ/Vultaik/wiki/Extension) and [features](#Features) are aggressively made use of to improve performance

Some notable extensions that **should** be supported for optimal or correct behavior.
These extensions will likely become mandatory later.

- `VK_EXT_descriptor_indexing`
- `VK_KHR_timeline_semaphore`

<br>

## Features:

- [ ] Ray Tracing
- [ ] Conservative rasterization
- [ ] Multiview rendering
- [ ] Conditional rendering 
- [ ] Shading rate
- [x] Descriptor indexing
- [ ] Timeline semaphore
- [ ] Synchronization2
- [x] Memory allocator
- [x] GPU-Assisted Validation
- [x] Compute Shader
- [x] Geometry Shader
- [x] Tessellation Shader
- [ ] Instancing, Indirect drawing
- [ ] Post-AA (FXAA, SMAA and TAA)
- [ ] Multithreaded rendering
- [x] Automatic descriptor set management
- [x] Shader reflection with [SPIRV-Cross](https://github.com/KhronosGroup/SPIRV-Cross)
- [x] Vulkan HLSL for shaders, shaders are compiled in runtime with [DirectXShaderCompiler](https://github.com/microsoft/DirectXShaderCompiler)
- [x] GLTF 2.0 for samples

<br>



> The examples not only show how to use the API, but also show things specific to HLSL and its mapping with Vultaik, (Variable Rate Shading, [AMD effects](https://gpuopen.com/effects/), Ray Tracing) and can be used as a guide on how to use techniques independent of Vulkan.


## Examples


Example | Details
---------|--------
<img src="Screenshots/ClearScreen.PNG" width=380> | [Clear Screen](Src/Samples/Samples/ClearScreen/ClearScreen.cs)<br> This example shows how to configure the device and clear the color.
<img src="Screenshots/Triangle.PNG" width=380> | [Triangle](Src/Samples/Samples/Triangle/Triangle.cs)<br> This example shows how to render simple triangle.
<img src="Screenshots/Transformations.PNG" width=380> | [Transformations](Src/Samples/Samples/Transformations/Transformations.cs)<br> This example how to transform the world space for each object.
<img src="Screenshots/PushConstant.PNG" width=380> | [Push Constant](Src/Samples/Samples/PushConstant/PushConstant.cs)<br> This example use push constants, small blocks of uniform data stored within a command buffer, to pass data to a shader without the need for uniform buffers.
<img src="Screenshots/LoadGLTF.PNG" width=380> | [Load GLTF](Src/Samples/Samples/LoadGLTF/LoadGLTF.cs)<br> This example shows how to load GLTF models.
<img src="Screenshots/LoadTexture.PNG" width=380> | [Load Texture](Src/Samples/Samples/LoadTexture/LoadTexture.cs)<br> This example shows how to load 2D texture from disk (including all mip levels). 
<img src="Screenshots/Bindless.PNG" width=380> | [Bindless](Src/Samples/Samples/Bindless/Bindless.cs)<br> This example demonstrates the use of VK_EXT_descriptor_indexing for creating descriptor sets with a variable size that can be dynamically indexed in a shader using `SPV_EXT_descriptor_indexing`.
<img src="Screenshots/DiffuseLighting.PNG" width=380> | [Diffuse Lighting](Src/Samples/Samples/Lighting/Lighting.cs)<br> This example shows how to create diffuse lighting. 
<img src="Screenshots/AmbientLighting.PNG" width=380> | [Ambient Lighting](Src/Samples/Samples/AmbientLighting/AmbientLighting.cs)<br> This example shows how to create ambient lighting. 
<img src="Screenshots/SpecularLighting.PNG" width=380> | [Specular Lighting](Src/Samples/Samples/SpecularLighting/SpecularLighting.cs)<br> This example shows how to create specular lighting. 
<img src="Screenshots/ComputeTexture.PNG" width=380> | [Compute Texture](Src/Samples/Samples/SpecularLighting/ComputeTexture.cs)<br> Use a calculation shader in conjunction with a separate calculation queue to modify a full screen image.
<hr>
<br>




Additionally, **Vultaik** uses NuGet packages or code from the following repositories:

- [Vortice.Vulkan](https://github.com/amerkoleci/Vortice.Vulkan)
- [Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows)
- [Silk.NET](https://github.com/dotnet/Silk.NET)
- [VMASharp](https://github.com/sunkin351/VMASharp)


