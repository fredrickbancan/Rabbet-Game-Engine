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

out vec3 worldSpacePos;

void main()
{
	worldSpacePos = position.xyz;
	gl_Position = projectionMatrix * viewMatrix * position;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
out vec4 color;
in vec3 worldSpacePos;
uniform vec3 skyColor;
uniform vec3 skyAmbient;
uniform vec3 skyHorizon;
uniform vec3 fogColor;
uniform vec3 sunDir;
uniform float minSkyLuminosity = 0.01;
uniform float maxSkyLuminosity = 1.5;
uniform sampler2D ditherTex;

void main()
{
	vec3 fragDir = normalize(worldSpacePos);
	if (fragDir.y < -0.01) discard;
	
	vec3 hazeTone = vec3(1.0 / maxSkyLuminosity);
	float sunHeight = (sunDir.y + 1) * 0.5;
	float sunProximity = (dot(sunDir, fragDir) + 1.01) * 0.5;
	float n = pow(sunProximity, 2) * (1-fragDir.y*0.5);
	float horizonStrength = pow(1 - fragDir.y, 32 * clamp(1-sunHeight, 0.05, 1.0) * (1-n));
	horizonStrength *= pow(sunHeight, 0.5);


	vec3 skyGradient = mix(skyAmbient/maxSkyLuminosity, skyColor, horizonStrength);

	vec3 hazeColor = mix(hazeTone, skyHorizon * (1+(maxSkyLuminosity * sunProximity)) * 0.5, sunProximity * clamp(2-sunHeight*2,0,1) * (1 - fragDir.y * 5.0) );
	skyGradient = mix(skyGradient, hazeColor, pow(1 - fragDir.y, 8 * (1-n)));


	skyGradient *= mix(maxSkyLuminosity, minSkyLuminosity, 1-sunHeight);//set sky brightness based on time of day
	color.rgb = skyGradient;
	color += vec4(texture2D(ditherTex, gl_FragCoord.xy / 8.0).r / 32.0 - (1.0 / 128.0));//dithering
	color.a = 1.0;
}