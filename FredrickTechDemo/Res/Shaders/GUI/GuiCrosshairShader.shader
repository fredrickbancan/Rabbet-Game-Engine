#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 colour;
layout(location = 2) in vec2 texCoord;

out vec4 vcolour;
out vec2 fTexCoord;

uniform mat4 modelMatrix;
uniform mat4 projectionMatrix;

void main()
{
	gl_Position = projectionMatrix * modelMatrix * position;
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
	vec4 textureColor = texture(uTexture, fTexCoord);
	if (textureColor.a < 1.0)
		discard;
	gl_FragDepth = -0.9999999;//render on top
	color = vec4(vcolour.rgb, 1.0);
}