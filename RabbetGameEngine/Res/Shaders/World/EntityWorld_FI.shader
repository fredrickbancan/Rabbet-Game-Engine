//base shader for instance rendering a large model multiple times with fog and transparency
#shader vertex
#version 330 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord; //other data such as color and uv can also be provided for each instance rendered
layout(location = 3) in vec4 modelMatrixRow0; 
layout(location = 4) in vec4 modelMatrixRow1;
layout(location = 5) in vec4 modelMatrixRow2;
layout(location = 6) in vec4 modelMatrixRow3;


uniform float fogDensity = 0.0075;
uniform float fogGradient = 2.5;

out vec3 fogColorV;
out vec2 vTexCoord;
out vec4 vColor;
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
	vColor = vertexColor;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
in vec2 vTexCoord;
in vec4 vColor;
in float visibility;

uniform int frame = 0;//number of frame, used to itterate noise

uniform sampler2D uTexture;

uniform vec3 fogColor;
out vec4 color;

void main()
{
	vec4 textureColor = texture(uTexture, vTexCoord) * vColor;// mixes colour and textures
	if (textureColor.a < 0.01F)
	{
		discard;//cutout
	}
	color.rgb = mix(fogColor, textureColor.rgb, visibility);
	color.a = 1;
}