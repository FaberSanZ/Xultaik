
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


## Features:

- [ ] KHR Ray Tracing
- [x] Entity Component System
- [ ] Memory allocator (VMA)
- [ ] Physically-Based Rendering
- [ ] Instancing, Indirect drawing
- [ ] Tessellation (Phong, PN Triangles, and Displacement Mapping)
- [ ] Post-AA (FXAA, SMAA and TAA)
- [x] Vulkan GLSL for shaders, shaders are compiled in runtime with shaderc
- [ ] Multithreaded rendering
- [x] GLTF 2.0


<hr>
<br>


## Examples

| Example   | Screenshot  | Description          |
|---------------|-------------|----------------------|
| [Clear Screen](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/ClearScreen.cs) | <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/ClearScreen.PNG" width=300> | This example shows how to configure the device and clear the color. |
| [Triangle](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Triangle.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Triangle.PNG" width=300> | This example shows how to render simple triangle.  |
| [Transformations](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/Transformations.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/Transformations.PNG" width=300> | We will transform the world space for each object (the two cubes) using transformation matrices.  |
| [LoadGLTF](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadGLTF.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadGLTF.PNG" width=300> | Load GLTF 3D Model.  |
| [LoadTexture](https://github.com/FaberSanZ/Zeckoxe-Engine/blob/master/Src/Samples/Samples/LoadTexture.cs) |  <img src="https://github.com/Zeckoxe/Zeckoxe-Engine/blob/master/Screenshots/LoadTexture.PNG" width=300> | Loads a 2D texture from disk (including all mip levels), uses staging to upload it into video memory.  |


<hr>
<br>


## Research or code used:
- Granite (<https://github.com/Themaister/Granite>)
- Vulkan examples from Sascha Willems (<https://github.com/SaschaWillems/Vulkan>)
- Vortice.Vulkan (<https://github.com/amerkoleci/Vortice.Vulkan>)
- SharpGLTF (<https://github.com/vpenades/SharpGLTF>)
- DefaultEcs (<https://github.com/Doraku/DefaultEcs>)



