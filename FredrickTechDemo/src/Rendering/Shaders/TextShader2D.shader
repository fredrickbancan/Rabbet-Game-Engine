#shader vertex
#version 330 core
//location is the location of the value in the vertex atrib array
//for vec4 position, the gpu automatically fills in the 4th component with a 1.0F. This means you can treat position as a vec4 no problem. (no need for messy conversions)
layout(location = 0) in vec4 position;
layout(location = 1) in vec2 texCoord;

out vec2 fTexCoord;
uniform vec2 translation;

void main()
{
	gl_Position = vec4(position + translation * vec2(2.0, -2.0), 0.0, 1.0);
	fTexCoord = texCoord;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
in vec2 fTexCoord;
out vec4 color;

uniform vec3 textColour;
uniform sampler2D uTexture;

void main()
{
	color = vec4(textColour, texture(uTexture, fTexCoord).a)
}
