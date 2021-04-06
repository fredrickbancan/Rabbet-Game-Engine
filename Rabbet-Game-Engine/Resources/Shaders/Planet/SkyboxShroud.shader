#shader vertex
#version 330 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	gl_Position = projectionMatrix * viewMatrix * position;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
out vec4 fragColor;
uniform vec3 fogColor;
void main()
{
	fragColor = vec4(0,0,0,1);
}