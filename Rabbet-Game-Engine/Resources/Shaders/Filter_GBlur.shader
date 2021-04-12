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

void main()
{
	vec2 texelSize = 1.0 / textureSize(srcTex, 0);
	vec4 result;
	int x = verticalPass ? 0 : 1;
	int y = 1 - x;
	vec2 offset = vec2(x,y) * texelSize;

	switch (clamp(layer, 0, 1))
	{
	case 0:
		result = texture2D(srcTex, fTexCoord) * weights_9t[0];

		result += texture2D(srcTex, fTexCoord + offset * offsets_9t[1]) * weights_9t[1];
		result += texture2D(srcTex, fTexCoord + offset * offsets_9t[2]) * weights_9t[2];
												
		result += texture2D(srcTex, fTexCoord - offset * offsets_9t[1]) * weights_9t[1];
		result += texture2D(srcTex, fTexCoord - offset * offsets_9t[2]) * weights_9t[2];
		break;

	case 1:
		result = texture2D(srcTex, fTexCoord) * weights_21t[0];

		result += texture2D(srcTex, fTexCoord + offset * offsets_21t[1]) * weights_21t[1];
		result += texture2D(srcTex, fTexCoord + offset * offsets_21t[2]) * weights_21t[2];
		result += texture2D(srcTex, fTexCoord + offset * offsets_21t[3]) * weights_21t[3];
		result += texture2D(srcTex, fTexCoord + offset * offsets_21t[4]) * weights_21t[4];
		result += texture2D(srcTex, fTexCoord + offset * offsets_21t[5]) * weights_21t[5];
												
		result += texture2D(srcTex, fTexCoord - offset * offsets_21t[1]) * weights_21t[1];
		result += texture2D(srcTex, fTexCoord - offset * offsets_21t[2]) * weights_21t[2];
		result += texture2D(srcTex, fTexCoord - offset * offsets_21t[3]) * weights_21t[3];
		result += texture2D(srcTex, fTexCoord - offset * offsets_21t[4]) * weights_21t[4];
		result += texture2D(srcTex, fTexCoord - offset * offsets_21t[5]) * weights_21t[5];
		break; 
	default:
		break;
	}
	
	color = vec4(result.rgb, 1.0);
}