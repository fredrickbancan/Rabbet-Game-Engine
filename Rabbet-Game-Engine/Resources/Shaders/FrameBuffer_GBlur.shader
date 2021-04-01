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

uniform sampler2D bloomTexture;
uniform bool verticalPass;
uniform int layer;
const float weights_5[5] = float[](0.175713,0.121703,0.065984,0.028002,0.0093);
const float weights_11[11] = float[](0.048473,0.048449,0.048378,0.048261,0.048096,0.047885,0.047629,0.047327,0.046982,0.046593,0.046163);
const float weights_21[21] = float[](0.050359, 0.049967, 0.048811, 0.046944, 0.044449, 0.041434, 0.038027, 0.034359, 0.030564, 0.026767, 0.023079, 0.019591, 0.016373, 0.013471, 0.010913, 0.008703, 0.006833, 0.005282, 0.00402, 0.003012, 0.002222);
void main()
{
	vec2 texelSize = 1.0 / textureSize(bloomTexture, 0);
	vec3 result;

	vec2 offset;

	switch (layer)
	{
	case 0:
		result = texture2D(bloomTexture, fTexCoord).rgb * weights_5[0];
		for (int i = 0; i < 5; ++i)
		{
			offset = verticalPass ? vec2(0.0, texelSize.y * i) : vec2(texelSize.x * i, 0.0);
			result += texture2D(bloomTexture, fTexCoord + offset).rgb * weights_5[i];
			result += texture2D(bloomTexture, fTexCoord - offset).rgb * weights_5[i];
		}
		break;

	case 1:
		result = texture2D(bloomTexture, fTexCoord).rgb * weights_11[0];
		for (int i = 0; i < 11; ++i)
		{
			offset = verticalPass ? vec2(0.0, texelSize.y * i) : vec2(texelSize.x * i, 0.0);
			result += texture2D(bloomTexture, fTexCoord + offset).rgb * weights_11[i];
			result += texture2D(bloomTexture, fTexCoord - offset).rgb * weights_11[i];
		}
		break;

	case 2:
		result = texture2D(bloomTexture, fTexCoord).rgb * weights_21[0];
		for (int i = 0; i < 21; ++i)
		{
			offset = verticalPass ? vec2(0.0, texelSize.y * i) : vec2(texelSize.x * i, 0.0);
			result += texture2D(bloomTexture, fTexCoord + offset).rgb * weights_21[i];
			result += texture2D(bloomTexture, fTexCoord - offset).rgb * weights_21[i];
		}
		break;

	default:
		break;
	}
	
	color = vec4(result, 1.0); 
}