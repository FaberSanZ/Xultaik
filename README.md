
<h1 align="center">
   Zeckoxe
  
  ##               Zeckoxe is a 2D/3D renderer implemented in [Vulkan®](https://www.khronos.org/vulkan/)
  
</h1>

<hr>



### Zecoxe can be used to produce render layers for custom engines or next-gen games and provides building blocks for writing your own engine or game quickly

<br>

## Low-level rendering backend

The rendering backend focuses entirely on Vulkan, so it reuses Vulkan enums where appropriate. However, the API greatly simplifies the more painful points of writing straight Vulkan. Modern Vulkan [extensions](https://github.com/FaberSanZ/Zeckoxe/wiki/Extension) and features are aggressively made use of to improve performance


<br>
<br>

> :warning: The Zeckoxe.Audio librarie are optional and you will need to download the FMOD native binaries from https://www.fmod.com/download


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
| [Transformations](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Transformations.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Transformations.PNG" width=350> | This example how to transform the world space for each object.  |
| [LoadGLTF](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadGLTF.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadGLTF.PNG" width=350> | This example shows how to load GLTF models.  |
| [LoadTexture](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadTexture.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadTexture.PNG" width=350> | This example shows how to load 2D texture from disk (including all mip levels).  |
| [Lighting](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Lighting.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Lighting.PNG" width=350> | This example shows how to create basic lighting.  |


<hr>
<br>


## The project is inspired by and based on:
The-Forge (<https://github.com/ConfettiFX/The-Forge>)



<br>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/FakzSoft/Zeckoxe/blob/master/LICENSE)

The code is licensed under MIT. Feel free to use it for whatever purpose.
If you have any commercial use for this project which needs further development to accomplish, I might be available for contracting work. Contact me by [Twitter](https://twitter.com/FaberSan_Z).

<hr>

