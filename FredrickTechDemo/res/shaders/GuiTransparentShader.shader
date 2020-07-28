/*This shader is for rendering transparent gui components with colour. For example, text, crosshair etc*/

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
	fcolour = colour;
	fTexCoord = texCoord;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
in vec4 vcolour;
in vec2 fTexCoord;
out vec4 color;

uniform sampler2D uTexture;

void main()
{
	color = vec4(fcolour, texture(uTexture, fTexCoord).a)
}