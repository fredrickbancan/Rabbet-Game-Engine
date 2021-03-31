#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 uv;

uniform float barrelDistortion;   // s: 0 = perspective, 1 = stereographic
uniform float cylRatio;           // c: cylindrical distortion ratio. 1 = spherical
uniform float height;             // h: tan(verticalFOVInRadians / 2)
uniform float aspectRatio;        // a: screenWidth / screenHeight

out vec3 vUV;                 // output to interpolate over screen
out vec2 vUVDot;              // output to interpolate over screen

void main()
{
	gl_Position = position;

    float scaledHeight = barrelDistortion * height;
    float cylAspectRatio = aspectRatio * cylRatio;
    float aspectDiagSq = aspectRatio * aspectRatio + 1.0;
    float diagSq = scaledHeight * scaledHeight * aspectDiagSq;
    vec2 signedUV = (2.0 * uv + vec2(-1.0, -1.0));

    float z = 0.5 * sqrt(diagSq + 1.0) + 0.5;
    float ny = (z - 1.0) / (cylAspectRatio * cylAspectRatio + 1.0);

    vUVDot = sqrt(ny) * vec2(cylAspectRatio, 1.0) * signedUV;
    vUV = vec3(0.5, 0.5, 1.0) * z + vec3(-0.5, -0.5, 0.0);
    vUV.xy += uv;
}

#shader fragment
#version 330 core

in vec3 vUV;
in vec2 vUVDot;
layout(location = 0) out vec4 color;
layout(location = 1) out vec4 brightColor;//for bloom 

uniform sampler2D renderedTexture;
uniform vec3 cameraFrontVec;
uniform float gamma;
uniform float exposure = 5.0;//TODO: implement automatic eye adjust exposure
void main()
{
    vec3 uv = dot(vUVDot, vUVDot) * vec3(-0.5, -0.5, -1.0) + vUV;
	vec3 hdrColor = texture(renderedTexture, uv.xy / uv.z).rgb;
    //exposure tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor * exposure);
    // gamma correction 
    mapped = pow(mapped, vec3(1.0 / gamma));
    color = vec4(mapped, 1.0);

    // check whether fragment output is higher than threshold, if so output as brightness color for bloom
    float brightness = dot(color.rgb, vec3(0.2126, 0.7152, 0.0722));
    brightColor = brightness > 1.0 ? vec4(color.rgb, 1.0) : vec4(0, 0, 0, 1.0);
    brightColor = vec4(1,0,0,1);
}