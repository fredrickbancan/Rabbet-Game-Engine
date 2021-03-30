//base shader for dynamically rendering world objects with fog and linear interpolation.
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float textureIndex;
layout(location = 4) in mat4 modelMatrix;
layout(location = 8) in mat4 prevTickModelMatrix;

uniform float fogStart = 1000;
uniform float fogEnd = 1000;

out vec2 vTexCoord;
out vec4 vColor;
out float vTextureIndex;
out float visibility;//for fog

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform float percentageToNextTick;

void main()
{
	//lerp matrix between ticks
	mat4 lerpMatrix = prevTickModelMatrix + (modelMatrix - prevTickModelMatrix) * percentageToNextTick;
	
	vec4 positionRelativeToCam = viewMatrix * lerpMatrix * position;

	gl_Position = projectionMatrix * positionRelativeToCam;

	float distanceFromCam = length(positionRelativeToCam.xyz);
	visibility = (distanceFromCam - fogStart) / (fogEnd - fogStart);
	visibility = clamp(visibility, 0.0, 1.0);
	visibility = 1.0 - visibility;
	visibility *= visibility;

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
uniform vec3 fogColor;

void main()
{
	int i = int(ceil(vTextureIndex));
	vec4 textureColor = texture(uTextures[i], vTexCoord) * vColor;
	if (textureColor.a < 0.01F)
	{
		discard;//cutout
	}
	fragColor.rgb = mix(fogColor, textureColor.rgb, visibility);
	fragColor.a = 1.0F;
}