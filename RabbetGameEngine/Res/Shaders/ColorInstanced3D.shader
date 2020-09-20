#shader vertex
#version 330 core
//location is the location of the value in the vertex atrib array
//for vec4 position, the gpu automatically fills in the 4th component with a 1.0F. This means you can treat position as a vec4 no problem. (no need for messy conversions)
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord; //other data such as color and uv can also be provided for each instance rendered
layout(location = 3) in vec4 modelMatrixRow0; 
layout(location = 4) in vec4 modelMatrixRow1;
layout(location = 5) in vec4 modelMatrixRow2;
layout(location = 6) in vec4 modelMatrixRow3;

out vec4 vColor;

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

	gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;

	vColor = vertexColor;
}

/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core
in vec4 vColor;
out vec4 fragColor;


uniform int frame = 0;

float rand3D(in vec3 xyz) 
{
	return fract(tan(distance(xyz.xy * 1.61803398874989484820459, xyz.xy) * xyz.z) * xyz.y);
}

void main()
{
	if (vColor.a < 0.99)
	{
		if (rand3D(gl_FragCoord.xyz + (float(frame) * 0.001F)) > vColor.a)//stochastic transparency
			discard;
	}

	fragColor = vec4(vColor.rgb, 1.0F);
}