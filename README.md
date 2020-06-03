
<h1 align="center">
   Zeckoxe Engine
  <br>
  
  ##               Zeckoxe Engine is a 2D/3D renderer implemented in [Vulkan®](https://www.khronos.org/vulkan/)
  
</h1>

<hr>


<br>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/LICENSE)

The code is licensed under MIT. Feel free to use it for whatever purpose.

<hr>
<br>



## Low-level rendering backend

The rendering backend is completely focused on Vulkan, however the API greatly simplifies the most painful points of writing Vulkan. It is likely to be a happy medium ground between "perfect" Vulkan and OpenGL / D3D11 w.r.t. CPU overload in .NET

TODO:

- [ ] KHR Ray Tracing
- [ ] Memory manager (VMA)
- [ ] Deferred destruction and release of API objects and memory
- [ ] Automatic descriptor set management
- [ ] Linear allocators for vertex/index/uniform/staging data
- [x] Vulkan GLSL for shaders, shaders are compiled in runtime with shaderc
- [ ] Automatic pipeline creation
- [ ] Multithreaded rendering
- [ ] GLTF 2.0
- [ ] KTX

<hr>
<br>

## Research or code used:
- Granite (<https://github.com/Themaister/Granite>)
- Vulkan examples from Sascha Willems (<https://github.com/SaschaWillems/Vulkan>)
- Vortice.Vulkan from Amer Koleci (<https://github.com/amerkoleci/Vortice.Vulkan>)

<hr>
<br>

## Examples

| Example   | Screenshot  | Description          |
|---------------|-------------|----------------------|
| [Clear Screen](https://github.com/Zeckoxe/Zeckoxe-Engine/tree/master/Src/01-ClearScreen) | <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/01.PNG" width=180> | This example shows how to configure the device and clear the color. |
| [Triangle](https://github.com/Zeckoxe/Zeckoxe-Engine/tree/master/Src/01-ClearScreen) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/02.PNG" width=180> | This example shows how to render simple triangle.  |






