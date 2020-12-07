#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 texCoord;

out vec4 vColor;
uniform mat4 orthoMatrix;

void main()
{
	gl_Position = orthoMatrix * position;
	vColor = Color;
}

#shader fragment
#version 330 core
out vec4 color;
in vec4 vColor;

void main()
{
	color = vColor;
}