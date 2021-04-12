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
uniform vec3 skyHorizon;
uniform vec3 skyHorizonAmbient;
uniform vec3 fogColor;
uniform vec3 sunDir;
uniform float skyLuminosity;
uniform float minSkyLuminosity;
uniform float maxSkyLuminosity;
uniform sampler2D ditherTex;

void main()
{
	vec3 fragDir = normalize(worldSpacePos);
	if (fragDir.y < -0.01F) discard;
	float sunHeight = (sunDir.y + 1.0F) * 0.5F;//0.0 at midnight, 1.0 at midday
	float sunProximity = (dot(sunDir, fragDir) + 1.01F) * 0.5F;//0.0 to 1.0 depending how close to the sun the fragment is
	float nightFactor = clamp(1.0F - sunHeight, 0.05F, 1.0F);//0.05 to 1.0 depending how close the sun is to being down
	float horizonIntensityFactor = sunProximity * (1.0F-fragDir.y);//0.0 to 1.0 based on closeness to sun and horizon
	float p = 1 - ((1-sunProximity) * (1-sunHeight));

	vec3 skyTop = skyColor * 0.6F;
	vec3 skyBottom = vec3(1.0F);// mix(skyColor, vec3(1.0F), 1.0F - nightFactor);
	vec3 skyMixed = mix(skyTop, skyBottom, pow(1.0F - fragDir.y * 0.5F, 6.0F + nightFactor * 8.0F));//calculate color of sky
	skyMixed = mix(skyMixed, skyHorizon, pow(horizonIntensityFactor, 3.141F));
	skyMixed = mix(skyMixed, skyHorizonAmbient, pow(horizonIntensityFactor, 6.282F));


	//change brightness
	float atmosBright = mix(minSkyLuminosity, maxSkyLuminosity, clamp(pow(1-nightFactor, 4.0F) * 16.0F, 0.0F, 1.0F) * p);//calculate brightness for sky colors
	color.rgb = skyMixed * atmosBright;


    color += vec4(texture2D(ditherTex, gl_FragCoord.xy / 8.0).r / 32.0 - (1.0 / 128.0));//dithering
	color.a = 1.0;
}