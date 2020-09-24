//base shader for rendering world terrain and entities with fog and transparency
#shader vertex
#version 330 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;

uniform float fogDensity = 0.0075;
uniform float fogGradient = 2.5;

out vec2 vTexCoord;
out vec4 vColor;
out float visibility;//for fog

//matrix for projection transformations.
uniform mat4 projectionMatrix;
//matrix for camera transformations.
uniform mat4 viewMatrix;
//matrix for model transformations. All transformations in this matrix are relative to the model origin.
uniform mat4 modelMatrix;

void main()
{
	vec4 worldPosition = modelMatrix * position;

	vec4 positionRelativeToCam = viewMatrix * worldPosition;

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
}