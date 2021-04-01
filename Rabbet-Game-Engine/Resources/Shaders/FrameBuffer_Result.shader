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

uniform sampler2D renderedTexture;
uniform sampler2D bloomTexture;
uniform float exposure = 1.0;//TODO: implement automatic eye adjust exposure
uniform float gamma;
void main()
{
	vec3 hdrColor = texture2D(renderedTexture, fTexCoord).rgb;
	vec3 bloomColor = texture2D(bloomTexture, fTexCoord).rgb;
    hdrColor += bloomColor;//additive blending
    //exposure tone mapping
    vec3 result = vec3(1.0) - exp(-hdrColor * exposure);

    // gamma correction 
    result = pow(result, vec3(1.0 / gamma));

    color = vec4(result, 1.0);
}