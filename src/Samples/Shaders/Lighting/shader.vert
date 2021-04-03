#version 450

layout(set = 0, binding = 0) uniform UniformBufferObject {
	mat4 model;
    mat4 view;
    mat4 proj;
} ubo;

layout(push_constant) uniform PushConsts 
{
    mat4 model;
} pc;

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inNormCoord;
layout(location = 2) in vec2 inTexCoord;


layout(location = 0) out vec2 fragTexCoord;
layout(location = 1) out vec3 vertPos;
layout(location = 2) out vec3 normalInterp;

void main() 
{
    fragTexCoord = inTexCoord;
	
	
	vertPos =  vec3((ubo.model * pc.model) * vec4(inPosition, 1.0));

    normalInterp = mat3(transpose(inverse(ubo.model * pc.model))) * inNormCoord;
	
	
    gl_Position = ubo.proj * ubo.view * (ubo.model * pc.model) * vec4(inPosition, 1.0);
}
