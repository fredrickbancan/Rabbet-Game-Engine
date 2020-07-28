/*This shader is for rendering 2d quads on screen with texture and colour. Can be used for most gui components e.g buttons, menu, overlays*/

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
in vec4 vcolour;
in vec2 fTexCoord;
out vec4 color;

uniform sampler2D uTexture;
void main()
{
	color = texture(uTexture, fTexCoord) * vcolour;// *texture(uTexture, fTexCoord); // mixes colour and textures
}