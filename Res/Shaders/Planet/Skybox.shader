//This shader will be for a simple gradient skybox
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;

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
uniform vec3 fogColor;
uniform vec3 sunDir;
void main()
{
	vec3 fragDir = normalize(worldSpacePos.xyz);
	
	if (fragDir.y > 0)
	{
		vec3 skyModified = skyTop;
		skyModified *= (dot(sunDir, fragDir) + 1) * 0.5;

		vec3 skyHorizonModified = skyHorizon;
		float horizonSunDot = dot(sunDir.xz, fragDir.xz);

		float ratio = 1 - (horizonSunDot + 1) * 0.5F;
		ratio += fragDir.y * 0.75F;

		color.rgb = mix(skyHorizonModified, skyModified, ratio);

		float sunDirDot = dot(vec3(0, 1, 0), sunDir);
		color.rgb *= 1 - (fragDir.y * 1 - ((sunDirDot + 1 )* 0.5));
		color.rgb *= 0.5;
	}
	else
	{
		color.rgb = fogColor;
	}
	color.a = 1;
	gl_FragDepth = 0.9999999F;//force fragdepth to be in background at all times
}