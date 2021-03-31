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
	gl_Position = projectionMatrix * viewMatrix * position;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
out vec4 color;
in vec4 worldSpacePos;
uniform vec3 skyTop;
uniform vec3 skyAmbient;
uniform vec3 skyHorizon;
uniform vec3 fogColor;
uniform vec3 sunDir;
uniform sampler2D ditherTex;

void main()
{
	vec3 fragDir = normalize(worldSpacePos.xyz);
	
	if (fragDir.y > 0)
	{
		vec3 skyModified = skyTop;
		float sunDirDot = dot(vec3(0, 1, 0), sunDir);
		float fragDirDot = (dot(sunDir, fragDir) + 1) * 0.5;
		skyModified = mix(skyAmbient, skyTop, (fragDirDot + 1) * 0.5);

		float horizonRatio = 1 - (fragDirDot + 1) * 0.5;
		horizonRatio += fragDir.y * 1.5;
		horizonRatio += clamp(-sunDir.y, 0, 1);//make horizon color fade to nothing when sun goes over horizon
		vec3 skyHorizonModified = mix(skyHorizon, skyModified, clamp(horizonRatio, 0, 1));
		color.rgb = mix(skyHorizonModified, skyModified, clamp(horizonRatio,0,1));

		color.rgb *= 1 - (fragDir.y * 1 - ((sunDirDot + 1 )* 0.5));

		color = 1.0 - exp(-1.0 * color);//"exposure"

		color += vec4(texture2D(ditherTex, gl_FragCoord.xy / 8.0).r / 32.0 - (1.0 / 128.0));//dithering

		color.a = 1;
	}
}