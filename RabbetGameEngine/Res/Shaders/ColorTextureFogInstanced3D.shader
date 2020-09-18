#shader vertex
#version 330 core
//location is the location of the value in the vertex atrib array
//for vec4 position, the gpu automatically fills in the 4th component with a 1.0F. This means you can treat position as a vec4 no problem. (no need for messy conversions)
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 colour;
layout(location = 2) in vec2 texCoord; //other data such as color and uv can also be provided for each instance rendered
layout(location = 3) in vec4 modelMatrixRow0; 
layout(location = 4) in vec4 modelMatrixRow1;
layout(location = 5) in vec4 modelMatrixRow2;
layout(location = 6) in vec4 modelMatrixRow3;


uniform float fogDensity = 0.0075;
uniform float fogGradient = 2.5;

out vec3 fogColorV;
out vec2 vTexCoord;
out vec4 vcolour;
out float visibility;//for fog

//matrix for projection transformations.
uniform mat4 projectionMatrix;

//matrix for camera transformations.
uniform mat4 viewMatrix;

void main()
{
	mat4 modelMatrix = mat4(1.0F);
	modelMatrix[0] = modelMatrixRow0;
	modelMatrix[1] = modelMatrixRow1;
	modelMatrix[2] = modelMatrixRow2;
	modelMatrix[3] = modelMatrixRow3;
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
in vec2 vTexCoord;
in vec4 vcolour;
in float visibility;

uniform sampler2D uTexture;
uniform vec3 fogColor;
out vec4 color;

float rand3D(in vec3 xyz) 
{
	return fract(tan(distance(xyz.xy * 1.61803398874989484820459, xyz.xy) * xyz.z) * xyz.y);
}

void main()
{
	float fragmentAlpha = 1.0F;

	vec4 textureColor = texture(uTexture, vTexCoord) * vcolour;// *texture(uTexture, vTexCoord); // mixes colour and textures

	if (textureColor.a < 1.0)
	{
		float randomFloat = rand3D(gl_FragCoord.xyz);
		fragmentAlpha = float(randomFloat < textureColor.a);//do stochastic transparency, noise can be reduced with sampling. 
	}

	color = mix(vec4(fogColor, fragmentAlpha), vec4(textureColor.rgb, fragmentAlpha), visibility);
}