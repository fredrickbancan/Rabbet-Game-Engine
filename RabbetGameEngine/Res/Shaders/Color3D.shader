#shader vertex
#version 330 core
//location is the location of the value in the vertex atrib array
//for vec4 position, the gpu automatically fills in the 4th component with a 1.0F. This means you can treat position as a vec4 no problem. (no need for messy conversions)
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;

out vec4 vColor;
//matrix for projection transformations.
uniform mat4 projectionMatrix;
//matrix for camera transformations.
uniform mat4 viewMatrix;
//matrix for model transformations. All transformations in this matrix are relative to the model origin.
uniform mat4 modelMatrix;
void main()
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;
	vColor = vertexColor;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
in vec4 vColor;
out vec4 color;

uniform int frame = 0;

float rand3D(in vec3 xyz)
{
	return fract(tan(distance(xyz.xy * 1.6F, xyz.xy) * xyz.z) * xyz.x);
}

void main()
{
	if (vColor.a < 0.99)
	{
		if (rand3D(gl_FragCoord.xyz + (float(frame) * 0.0000001F)) > vColor.a)//do stochastic transparency
			discard;
	}

	color.rgb = vColor.rgb;
}