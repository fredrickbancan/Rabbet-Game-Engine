#shader vertex
#version 330 core
//In order to properly render objects with pixelation, Any object rendered with PS1 shader must first be rendererd to an off screen texture and then projected.
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 colour;
layout(location = 2) in vec2 texCoord;

const float fogDensity = 0.025;
const float fogGradient = 3.5;

out vec2 vTexCoord;
out vec4 vcolour;
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
	vcolour = colour;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
noperspective in vec2 vTexCoord;//removing texture perspective
in vec4 vcolour;
in float visibility;
layout(origin_upper_left) in vec4 gl_FragCoord;
out vec4 color;
uniform sampler2D uTexture;

uniform vec3 fogColour;
void main()
{

	vec4 textureColor = texture(uTexture, vTexCoord) * vcolour;// *texture(uTexture, vTexCoord); // mixes colour and textures
	if (textureColor.a < 0.9)
	{
		discard;
	}
	color = textureColor;
	color = mix(vec4(fogColour, color.a), color, visibility);
}