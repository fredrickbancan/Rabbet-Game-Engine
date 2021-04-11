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
out float vTextureIndex;

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
	vTextureIndex = textureIndex;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
in vec2 vTexCoord;
in vec4 vColor;
in float vTextureIndex;
in float visibility;
out vec4 fragColor;

uniform sampler2D uTextures[8];
uniform int frame = 0;

void main()
{
	int i = int(ceil(vTextureIndex));
	vec4 textureColor = texture(uTextures[i], vTexCoord) * vColor;
	if (textureColor.a < 0.01F)
	{
		discard;//cutout
	}
	fragColor.a = 1.0F;
}