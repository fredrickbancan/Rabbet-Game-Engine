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

uniform sampler2D renderedTexture;
uniform vec3 cameraFrontVec;
uniform float brightness;
void main()
{
    vec3 uv = dot(vUVDot, vUVDot) * vec3(-0.5, -0.5, -1.0) + vUV;
	color = texture(renderedTexture, uv.xy / uv.z) * brightness;
	color.a = 1.0;
}