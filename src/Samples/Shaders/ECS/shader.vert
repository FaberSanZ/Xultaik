#version 450

#extension GL_EXT_scalar_block_layout : require

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;

layout (binding = 0) uniform UBO 
{
	mat4 modelMatrix;
    mat4 viewMatrix;
	mat4 projectionMatrix;
} ubo;

layout (location = 0) out vec2 outUV;
layout (location = 1) out vec3 outN;
layout (location = 2) out vec3 outV;//ViewDir
layout (location = 3) out vec3 outL;

out gl_PerVertex 
{
    vec4 gl_Position;   
};

layout(push_constant) uniform PushConsts 
{
    mat4 model;
} pc;

vec3 light = vec3(-220.0,-600.0, -4.0);

void main() 
{
    outUV = inUV;
    
    mat4 mod = ubo.modelMatrix * pc.model;
    vec4 pos = mod * vec4(inPos.xyz, 1.0);
    vec3 lPos = mat3(mod) * light;
    
    //outN = normalize(transpose(inverse(mat3(mod))) * inNormal);    
    outN = mat3(mod)* inNormal;    
    
    //mat4 viewInv = inverse(ubo.viewMatrix);
    
    outV = -pos.xyz;//normalize(vec3(viewInv * vec4(0.0, 0.0, 0.0, 1.0) - pos));
    outL = lPos - pos.xyz;
    
	gl_Position = ubo.projectionMatrix * ubo.viewMatrix * pos;    
}