//this shader is for rendering 2d quads with a color. the texture provided is only to assign alpha. any less than 1 alpha pixels will be discarded.
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 texCoord;

out vec4 vColor;
out vec2 fTexCoord;

uniform mat4 orthoMatrix;

void main()
{
	gl_Position = orthoMatrix * position;
	vColor = Color;
	fTexCoord = texCoord;
}

#shader fragment
#version 330 core
out vec4 color;
in vec4 vColor;
in vec2 fTexCoord;

uniform sampler2D uTexture;

void main()
{
	//gl_FragDepth = 0;
	vec4 textureColor = texture(uTexture, fTexCoord);
	if (textureColor.a < 1.0)
		discard;
	color = vec4(vColor.rgb, 1.0);
}