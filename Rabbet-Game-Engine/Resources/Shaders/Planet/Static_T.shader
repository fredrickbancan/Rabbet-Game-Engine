//base shader for rendering objects statically with fog and transparency
#shader vertex
#version 330 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float textureIndex;
out vec2 vTexCoord;
out vec4 vColor;

uniform float percentageToNextTick;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	gl_Position = projectionMatrix * viewMatrix * position;
	vTexCoord = texCoord;
	vColor = vertexColor;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
in vec2 vTexCoord;
in vec4 vColor;
out vec4 fragColor;

uniform sampler2D uTexture;


void main()
{
	vec4 textureColor = texture(uTexture, vTexCoord) * vColor;
	//this avoids alpha sorting issues with fully transparent surfaces
	if (fragColor.a < 0.01)
	{
		discard;
	}
}