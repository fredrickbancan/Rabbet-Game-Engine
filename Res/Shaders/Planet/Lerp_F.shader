//base shader for dynamically rendering world objects with fog and linear interpolation.
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float objectID;
layout(location = 4) in mat4 modelMatrix;
layout(location = 8) in mat4 prevTickModelMatrix;

uniform float fogDensity = 0.0075;
uniform float fogGradient = 2.5;

out vec2 vTexCoord;
out vec4 vColor;
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
	visibility = exp(-pow((distanceFromCam * fogDensity), fogGradient));
	visibility = clamp(visibility, 0.0, 1.0);

	vColor = vertexColor;
	vTexCoord = texCoord;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
in vec2 vTexCoord;
in vec4 vColor;
in float visibility;
out vec4 fragColor;

uniform sampler2D uTexture;
uniform int frame = 0;
uniform vec3 fogColor;


void main()
{
	vec4 textureColor = texture(uTexture, vTexCoord) * vColor;
	if (textureColor.a < 0.01F)
	{
		discard;//cutout
	}
	fragColor.rgb = mix(fogColor, textureColor.rgb, visibility);
	fragColor.a = 1;
}