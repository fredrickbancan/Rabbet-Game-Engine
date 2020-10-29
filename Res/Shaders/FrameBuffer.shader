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

uniform sampler2D frameTexture0;
uniform sampler2D frameTexture1;
uniform sampler2D frameTexture2;
uniform sampler2D frameTexture3;
uniform sampler2D frameTexture4;
uniform sampler2D frameTexture5;
uniform sampler2D frameTexture6;
uniform sampler2D frameTexture7;

void main()
{
	vec4 frameColor0 = texture(frameTexture0, vTexCoord);
	vec4 frameColor1 = texture(frameTexture1, vTexCoord);
	vec4 frameColor2 = texture(frameTexture2, vTexCoord);
	vec4 frameColor3 = texture(frameTexture3, vTexCoord);
	vec4 frameColor4 = texture(frameTexture4, vTexCoord);
	vec4 frameColor5 = texture(frameTexture5, vTexCoord);
	vec4 frameColor6 = texture(frameTexture6, vTexCoord);
	vec4 frameColor7 = texture(frameTexture7, vTexCoord);

//	color = (frameColor0 * 0.125F) + (frameColor1 * 0.125F) + (frameColor2 * 0.125F) + (frameColor3 * 0.125F) + (frameColor4 * 0.125F) + (frameColor5 * 0.125F) + (frameColor6 * 0.125F) + (frameColor7 * 0.125F);
	color = (frameColor0 * 0.5F) + (frameColor1 * 0.5F);// +(frameColor2 * 0.125F) + (frameColor3 * 0.125F) + (frameColor4 * 0.125F) + (frameColor5 * 0.125F) + (frameColor6 * 0.125F) + (frameColor7 * 0.125F);
}