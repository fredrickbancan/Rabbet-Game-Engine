#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 colour;
layout(location = 2) in vec2 texCoord;

out vec4 vcolour;
out vec2 fTexCoord;

uniform mat4 scaleMatrix;

void main()
{
	gl_Position = scaleMatrix * position;
	vcolour = colour;
	fTexCoord = texCoord;
}

#shader fragment
#version 330 core
out vec4 color;
in vec4 vcolour;
in vec2 fTexCoord;

uniform sampler2D uTexture;

void main()
{
	color = vec4(vcolour.rgb, texture(uTexture, fTexCoord).a);
}