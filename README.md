



<h1 align="center">
   <img src="Logo/XULTAIK-2.png" width=410>

  
  ##              🎮 Xultaik is a 2D/3D Game Engine implemented in [Vulkan®](https://www.khronos.org/vulkan/)

  
</h1>


[![Build status](https://github.com/FaberSanZ/Zeckoxe/workflows/ci/badge.svg)](https://github.com/FaberSanZ/Xultaik/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/FaberSanZ/Xultaik/blob/master/LICENSE) 

<br>

> [!IMPORTANT]
> The engine is being written from scratch and is being completely moved to vulkan.


<br>

## 📝 Overview
Welcome to Xultaik, the game engine designed to deliver AAA experiences with cutting-edge performance and advanced technologies. Centrally based on Vulkan, Xultaik harnesses the full capabilities of this low-level graphics API to provide direct control over the GPU and exceptional performance. It integrates Steam Audio for immersive audio effects and JoltPhysics for realistic physical simulations, ensuring a powerful and comprehensive platform for next-generation game creation.

<br>

## Key Features 🚀

**Cutting-Edge Graphics with Vulkan**:
Xultaik maximizes Vulkan's capabilities, a low-level graphics API that offers direct GPU control. Built exclusively for Vulkan, Xultaik delivers superior performance and full access to advanced API features. It implements the following extensions and key features:

- **Multi-Threading**: Vulkan is designed to leverage multi-core processors, allowing simultaneous execution of multiple graphics tasks. This translates to significantly improved performance and more efficient utilization of system resources.
- **Multi-GPU**: Vulkan natively supports multi-GPU configurations, enabling developers to distribute graphics workload across multiple graphics cards. This not only enhances performance but also allows for the creation of more complex and detailed graphics.
- **Ray Tracing**: Enables advanced lighting and shading effects for realistic graphics and immersive visual experiences. Vulkan Ray Tracing supports both hardware acceleration and software-based implementation, providing flexibility and performance.
- **Variable Rate Shading**: Enhances performance efficiency by adjusting shading rate based on scene complexity, maintaining visual quality. This technique allows developers to optimize GPU usage and improve frame rates without sacrificing important details.
- **Mesh Shaders**: Facilitates efficient rendering of complex models by allowing geometry manipulation at the shader level. Vulkan Mesh Shaders provide greater flexibility and control in geometry rendering, enhancing both performance and visual quality.
- **Vulkan Video**: Allows playback and manipulation of high-quality video content directly on the GPU, seamlessly integrating with the engine's graphic capabilities. This is ideal for games incorporating cinematics or interactive videos.
- **Advanced Vulkan Extensions**: Xultaik implements a variety of advanced Vulkan extensions such as VK_EXT_descriptor_indexing, VK_EXT_buffer_device_address, and VK_EXT_memory_budget, providing more granular control over graphics resources and optimizing application performance.

<br>

**Immersive Audio with Steam Audio and OpenAL**:
Xultaik utilizes Steam Audio for advanced spatialized audio effects. Steam Audio offers a range of features that significantly enhance the game's auditory experience:

- **Spatialized Audio**: Allows precise placement of sound effects in three-dimensional space, enhancing player immersion.
- **Occlusion**: Simulates how objects in the environment block or dampen sounds, providing a more realistic auditory experience. For example, a sound coming from an adjacent room will sound more muffled if there's a wall between the player and the sound source.
- **Reverberation**: Emulates how sound reflects off different surfaces and spaces, creating an authentic sonic environment. This includes everything from reverberation in large cathedrals to echo in small rooms.
- **HRTF (Head-Related Transfer Function)**: Enhances 3D sound perception, allowing players to accurately locate where sounds are coming from.
- **Sound Reflections**: Calculates how sounds bounce off surfaces in the environment, adding an additional layer of realism. For example, the sound of a gunshot in a narrow alley will have multiple reflections and echoes.
- **Support for Virtual Reality**: Optimizes the audio experience for virtual reality devices, ensuring that sounds react accurately to the player's head movements.

<br>

**Intuitive and Customizable Editor**:
Xultaik's editor provides an intuitive graphical interface for real-time manipulation of graphics, audio, physics, and more. It facilitates adjustment and optimization of all aspects of the game during development.

- **Real-Time Manipulation**
- **Advanced Audio Tools**
- **Physics Manipulation**
- **Shader Editor**
- **Advanced Scripting System**
- **User Interface Manipulation**

<br>

**Realistic Physics with JoltPhysics**:
Integrated to deliver dynamic and precise real-time physics simulations, JoltPhysics ensures realistic interactions between objects in the game, from collisions to complex behaviors. This physics engine is highly optimized to work efficiently alongside Vulkan.

- **Advanced Collision Simulation**: JoltPhysics offers precise and fast collision detection, ensuring that physical interactions in the game are realistic and error-free. This includes the ability to handle complex collisions between multiple dynamic and static objects.
- **Rigid and Soft Body Dynamics**: Supports both rigid and soft bodies, allowing simulation of a wide range of materials and physical behaviors. This includes simulation of objects such as rocks, liquids, fabrics, and more.
- **Multi-Threading Optimization**: JoltPhysics is designed to leverage multi-core architectures to the fullest, allowing concurrent physics simulations that significantly enhance performance. This ensures that complex physics do not compromise overall game performance.
- **Support for Special Effects**: JoltPhysics facilitates creation of realistic special effects, such as explosions, object destruction, and particle simulation, adding an additional layer of immersion and dynamism to the game.

<br>

**Advanced Texture Optimization with KTX**:
Xultaik optimizes loading and manipulation of textures using formats like KTX, specifically designed to maximize graphic performance without compromising visual quality. KTX is ideal for handling compressed textures essential for high-quality real-time rendering.

<br>

# Dependencies: 📦 **Xultaik** uses NuGet packages or code from the following repositories:
- [Vortice.Windows](https://github.com/amerkoleci/Vortice.Windows)
- [Stride Engine](https://github.com/stride3d/stride)
- [Prowl](https://github.com/ProwlEngine/Prowl)


