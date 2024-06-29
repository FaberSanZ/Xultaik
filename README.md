



<h1 align="center">
   <img src="Logo/XULTAIK-2.png" width=410>

  
  ##               Xultaik is a 2D/3D Game Engine implemented in [Vulkan®](https://www.khronos.org/vulkan/)
  
</h1>

NOTE: The engine is being written from scratch and the previous project was obsolete and discarded

[![Build status](https://github.com/FaberSanZ/Zeckoxe/workflows/ci/badge.svg)](https://github.com/FaberSanZ/Xultaik/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/FaberSanZ/Xultaik/blob/master/LICENSE) 

<br>

## Overview
Xultaik is an open-source C# game engine for realistic rendering. The engine is highly modular and aims at giving game makers more flexibility in their development.

<br>

## Low-level rendering backend

The rendering backend focuses entirely on Vulkan and HLSL, however, the API greatly simplifies the more painful points of writing straight Vulkan. Modern Vulkan [extensions](https://github.com/FaberSanZ/Vultaik/wiki/Extension) and [features](#Features) are aggressively made use of to improve performance



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



Additionally, **Xultaik** uses NuGet packages or code from the following repositories:

- [Vortice.Vulkan](https://github.com/amerkoleci/Vortice.Vulkan)
- [Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows)
- [Silk.NET](https://github.com/dotnet/Silk.NET)
- [VMASharp](https://github.com/sunkin351/VMASharp)


