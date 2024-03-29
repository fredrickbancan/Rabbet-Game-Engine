﻿//Shader for rendering stars in the sky, should be rendered after skybox, sun and moon.
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 pointColor;
layout(location = 2) in float radius;
layout(location = 3) in float aoc;

out vec4 vColor;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform vec2 viewPortSize;
uniform vec3 sunDir;

void main()
{
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;

    gl_PointSize = viewPortSize.y * radius;
    vColor = pointColor;
    vec3 worldPos = (modelMatrix * position).xyz;
    float r = 1.0 - (dot(worldPos, sunDir) + 1.0) * 0.5;
    float starHeight = (worldPos.y + 1) * 0.5;
    float sunHeight = (sunDir.y + 1) * 0.5;
    float dayRatio = clamp(sunHeight * sunHeight * 8.0, 0, 1);
    vColor.a = (r * r) * 1-dayRatio;
}


/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core

layout(location = 0) out vec4 fragColor;
in vec4 vColor;
void main()
{
    vec2 centerVec = gl_PointCoord - vec2(0.5);
    float coordLength = length(centerVec);
    float fade = pow(sqrt(1.05 - coordLength), 64);//making each star a fading point
    fragColor = vec4(vColor.rgb, vColor.a * fade);
    if (fragColor.a <= 0.001 || length(vColor.rgb) < 0.01) discard;
}