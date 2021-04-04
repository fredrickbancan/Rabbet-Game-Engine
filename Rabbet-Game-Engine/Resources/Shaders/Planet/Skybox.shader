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
uniform float minSkyLuminosity = 0.2;
uniform float maxSkyLuminosity = 3.0;
uniform sampler2D ditherTex;

void main()
{
	vec3 fragDir = normalize(worldSpacePos);
	if (fragDir.y < -0.01) discard;
	
	vec3 hazeTone = vec3(1.0 / maxSkyLuminosity); 
	float sunHeight = clamp( sunDir.y + 0.35, 0, 1);
	float sunProximity = clamp(dot(sunDir, fragDir), 0, 1);
	sunProximity *= sunProximity;
	float b = (1 - (sunProximity * (1 - sunHeight * 0.25)));
	float horizonStrength = pow( 1 - fragDir.y, 7 * b * clamp(1 - pow(sunHeight, 8), 0.5, 1.0) );
	float hazeStrength = pow(horizonStrength, 5 * b);

	horizonStrength *= clamp(pow(sunHeight, 0.4), 0.25, 1.0);
	hazeStrength *= clamp(pow(sunHeight, 0.4), 0.25, 1.0);

	vec3 skyGradient = mix(skyAmbient, skyColor, horizonStrength);

	vec3 hazeColor = mix(skyHorizon, hazeTone, b);
	skyGradient = mix(skyGradient, hazeColor, hazeStrength);




	skyGradient *= mix(minSkyLuminosity, maxSkyLuminosity, pow(sunHeight,3 * b));//set sky brightness based on time of day
	color.rgb = skyGradient;
	color += vec4(texture2D(ditherTex, gl_FragCoord.xy / 8.0).r / 32.0 - (1.0 / 128.0));//dithering
	color.a = 1.0;
}