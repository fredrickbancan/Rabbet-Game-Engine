#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 uv;  

out vec2 fTexCoord;           

void main()
{
	gl_Position = position;
    fTexCoord = uv;
}

#shader fragment
#version 330 core

in vec2 fTexCoord;
layout(location = 0) out vec4 color;

uniform sampler2D srcTex;
uniform bool verticalPass;
uniform int layer;

//values for 9 tap blur (optimised, requires linear texture filtering)
const float weights_9t[3] = float[](0.22702703, 0.31621622, 0.07027027);
const float offsets_9t[3] = float[](0.00000000, 1.38461538, 3.23076923);

//values for 21 tap blur (optimised, requires linear texture filtering)
const float weights_21t[6] = float[](0.14945008, 0.25281972, 0.12888849, 0.03730982, 0.00581452, 0.00044241);
const float offsets_21t[6] = float[](0.00000000, 1.44827586, 3.37931034, 5.31034483, 7.24137931, 9.17241379);

//values for 41 tap blur (optimised, requires linear texture filtering)
const float weights_41t[11] = float[](0.10614691, 0.19472467, 0.13779911, 0.07368936, 0.02959271, 0.00883993, 0.00193798, 0.00030612, 0.00003398, 0.00000256, 0.00000012);
const float offsets_41t[11] = float[](0.00000000, 1.47368421, 3.43859649, 5.40350877, 7.36842105, 9.33333333, 11.29824561, 13.26315789, 15.22807018, 17.19298246, 19.15789474);

void main()
{
	vec2 texelSize = 1.0 / textureSize(srcTex, 0);
	vec3 result;

	vec2 offset;

	switch (layer)
	{
	case 0:
		break;

	case 1:
		break;

	case 2:
		break;

	default:
		break;
	}
	
	color = vec4(result, 1.0); 
}