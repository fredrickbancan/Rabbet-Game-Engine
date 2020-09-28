﻿#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 texCoord;

out vec4 vColor;
out vec2 fTexCoord;

void main()
{
	gl_Position = position;
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
	gl_FragDepth = -1;//render on top
	color = texture(uTexture, fTexCoord) * vColor;
}