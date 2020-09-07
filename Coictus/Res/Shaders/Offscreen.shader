#shader vertex
#version 330 core
//location is the location of the value in the vertex atrib array
//for vec4 position, the gpu automatically fills in the 4th component with a 1.0F. This means you can treat position as a vec4 no problem. (no need for messy conversions)
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 texCoord;

out vec2 vTexCoord;
void main()
{
	gl_Position = vec4(position.x, position.y, 0.0, 1.0);
	vTexCoord = texCoord;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core

in vec2 vTexCoord;
layout(location = 0) out vec4 color;

uniform sampler2D renderedTexture;

void main()
{
	color = vec4(texture(renderedTexture, vTexCoord).rgb, 1.0);
}