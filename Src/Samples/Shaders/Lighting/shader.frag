#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable


layout (binding = 1) uniform sampler2D samplerColor;

layout (location = 0) in vec2 inUV;
layout (location = 1) in vec3 inN;
layout (location = 2) in vec3 inV;//ViewDir
layout (location = 3) in vec3 inL;

layout (location = 0) out vec4 outFragColor;




void main() 
{
    vec4 color = texture(samplerColor, inUV);

    vec3 N = normalize(inN);
    vec3 L = normalize(inL);
    vec3 V = normalize(inV);
    vec3 R = reflect(-L, N);
    vec3 diffuse = max(dot(N, L), 0.0) * vec3(0.9);
    vec3 specular = pow(max(dot(R, V), 0.0), 16.0) * vec3(0.75);
    outFragColor = vec4(diffuse * color.rgb + specular, 1.0);       
}
