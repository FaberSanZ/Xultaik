#version 450

#extension GL_EXT_scalar_block_layout : require



layout(set = 0, binding = 1) uniform sampler2D texSampler;

layout(set = 0, binding = 2, std430) uniform UBO  
{
   vec3 pos;
   vec3 viewPos;
   vec3 lightColor;
   vec3 pad;
} light;


layout(location = 0) in vec2 fragTexCoord;
layout(location = 1) in vec3 vertPos;
layout(location = 2) in vec3 normalInterp;

layout(location = 0) out vec4 outColor;






const float specularStrength  = 0.5;
const float ambientStrength = 0.1;


void main() 
{
	vec3 ambient = ambientStrength * light.lightColor;
 
	vec3 norm = normalize(normalInterp);
	vec3 lightDir = normalize(light.pos - vertPos);
	float diff = max(dot(norm,lightDir),0.0);
	vec3 diffuse = diff * light.lightColor;

	vec3 specular = vec3(1);

	if(diff > 0)
	{
		vec3 viewDir = normalize(light.viewPos - vertPos);
		vec3 reflectDir = reflect(-lightDir, norm) * light.pos.xyz;
		float spec = pow(max(dot(viewDir,reflectDir),0.0), 128);
		specular = specularStrength * spec * light.lightColor;
	}
 
	vec4 tex = texture(texSampler, fragTexCoord);
	vec4 light = vec4((ambient + diffuse + specular), 1.0);
 
	outColor = light * tex;
 
}
