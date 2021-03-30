
//this shader is for rendering 2d quads with a color. the texture provided is only to assign alpha. any less than 1 alpha pixels will be discarded.
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float textureIndex;

out vec4 vColor;
out vec2 fTexCoord;
out float vTextureIndex;
uniform mat4 orthoMatrix;

void main()
{
	gl_Position = orthoMatrix * position;
	vColor = Color;
	fTexCoord = texCoord;
	vTextureIndex = textureIndex;
}

#shader fragment
#version 330 core
out vec4 color;
in vec4 vColor;
in float vTextureIndex;
in vec2 fTexCoord;

uniform sampler2D uTextures[8];

void main()
{
	int i = int(ceil(vTextureIndex));
	vec4 textureColor = texture(uTextures[i], fTexCoord) * vColor;
	if (textureColor.a < 1.0)
		discard;
	color = vec4(vColor.rgb, 1.0);
}