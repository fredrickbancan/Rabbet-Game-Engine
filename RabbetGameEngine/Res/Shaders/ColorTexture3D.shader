#shader vertex
#version 330 core
//location is the location of the value in the vertex atrib array
//for vec4 position, the gpu automatically fills in the 4th component with a 1.0F. This means you can treat position as a vec4 no problem. (no need for messy conversions)
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 colour;
layout(location = 2) in vec2 texCoord;

out vec2 vTexCoord;

out vec4 vcolour;
//matrix for projection transformations.
uniform mat4 projectionMatrix;
//matrix for camera transformations.
uniform mat4 viewMatrix;
//matrix for model transformations. All transformations in this matrix are relative to the model origin.
uniform mat4 modelMatrix;
void main()
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;
	vTexCoord = texCoord;
	vcolour = colour;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
in vec4 vcolour;
in vec2 vTexCoord;
out vec4 color;

uniform sampler2D uTexture;

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

	color = vec4(textureColor.rgb, fragmentAlpha);
}