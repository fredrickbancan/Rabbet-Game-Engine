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
layout(location = 1) out vec4 bloomColor;//for bloom 

const vec3 threshHold = vec3(0.2126, 0.7152, 0.0722);

uniform sampler2D renderedTexture;

void main()
{
	vec3 hdrColor = texture(renderedTexture, fTexCoord).rgb;
    color = vec4(hdrColor, 1.0);

    // check whether hdr brightness is higher than threshold, if so output as brightness color for bloom
    float brightness = abs(dot(hdrColor, threshHold));
    bloomColor = vec4(hdrColor * brightness, 1.0);
}