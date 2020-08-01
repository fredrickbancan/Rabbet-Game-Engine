﻿#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 colour;
layout(location = 2) in vec2 texCoord;

out vec4 vcolour;
out vec2 fTexCoord;

void main()
{
	gl_Position =  position;
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
	gl_FragDepth = -1;
	color = vec4(vcolour.rgb, textureColor.a);
}