//base shader for rendering objects statically with fog and transparency
#shader vertex
#version 330 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float objectID;

uniform float fogDensity = 0.0075;
uniform float fogGradient = 2.5;

out vec2 vTexCoord;
out vec4 vColor;
out float visibility;//for fog

uniform float percentageToNextTick;
uniform int frame;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	vec4 positionRelativeToCam = viewMatrix * position;

	gl_Position = projectionMatrix * positionRelativeToCam;

	float distanceFromCam = length(positionRelativeToCam.xyz);
	visibility = exp(-pow((distanceFromCam * fogDensity), fogGradient));
	visibility = clamp(visibility, 0.0, 1.0);

	vTexCoord = texCoord;
	vColor = vertexColor;
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

	fragColor = mix(vec4(fogColor, textureColor.a), textureColor, visibility);

	//this avoids alpha sorting issues with fully transparent surfaces
	if (fragColor.a < 0.01)
	{
		discard;
	}
}