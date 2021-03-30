#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float textureIndex;

out vec4 vColor;
out vec2 fTexCoord;
out float vTextureIndex;
uniform mat4 orthoMatrix;

void main()
{
	gl_Position = orthoMatrix * position;
	vColor = vertexColor;
	fTexCoord = texCoord;
	vTextureIndex = textureIndex;
}

#shader fragment
#version 330 core
out vec4 color;
in vec4 vColor;
in vec2 fTexCoord;
in float vTextureIndex;
uniform sampler2D uTextures[8];

void main()
{

	int i = int(ceil(vTextureIndex));
	color = texture(uTextures[i], fTexCoord) * vColor;
	if (color.a < 0.01)
	{
		discard;
	}
}