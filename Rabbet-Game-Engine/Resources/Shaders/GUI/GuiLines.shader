#shader vertex
#version 330 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;

out vec4 vColor;
uniform mat4 orthoMatrix;

void main()
{
	gl_Position = orthoMatrix * position;
	vColor = vertexColor;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
in vec4 vColor;
out vec4 fragColor;
void main()
{
	fragColor = vColor;
	fragColor.a = 1.0F;
}