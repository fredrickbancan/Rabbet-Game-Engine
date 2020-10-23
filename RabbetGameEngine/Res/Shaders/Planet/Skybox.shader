//This shader will be for a simple gradient skybox
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float objectID;

//matrix for projection transformations.
uniform mat4 projectionMatrix;
//matrix for camera transformations.
uniform mat4 viewMatrix;

out vec4 worldSpacePos;

void main()
{
	worldSpacePos = position;

	mat4 viewMatrixNoTranslation = viewMatrix;//removing translation values from view matrix, to make skybox follow player camera
	viewMatrixNoTranslation[3][0] = 0.0F;
	viewMatrixNoTranslation[3][1] = 0.0F;
	viewMatrixNoTranslation[3][2] = 0.0F;
	gl_Position = projectionMatrix * viewMatrixNoTranslation * position;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
out vec4 color;
in vec4 worldSpacePos;

uniform vec3 skyTop;
uniform vec3 skyHorizon;

void main()
{
	gl_FragDepth = 0.9999F;//force fragdepth to be in background at all times
	vec3 pointOnSphere = normalize(worldSpacePos.xyz);//normalizing the position of the frag to form a spherical depth from the camera, leaving out the w coordinate

	color.rgb = mix(skyHorizon, skyTop, clamp(pointOnSphere.y, 0, 1));
	color.a = 1;
}