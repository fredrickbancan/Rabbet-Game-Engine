//base shader for dynamically rendering world objects with fog and linear interpolation.
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float textureIndex;
layout(location = 4) in mat4 modelMatrix;
layout(location = 8) in mat4 prevTickModelMatrix;

out vec2 vTexCoord;
out vec4 vColor;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform float percentageToNextTick;

void main()
{
	//lerp matrix between ticks
	mat4 lerpMatrix = prevTickModelMatrix + (modelMatrix - prevTickModelMatrix) * percentageToNextTick;

	gl_Position = projectionMatrix * viewMatrix * lerpMatrix * position;
	vColor = vertexColor;
	vTexCoord = texCoord;
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