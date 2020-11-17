//Shader for rendering stars in the sky, should be rendered after skybox, sun and moon.
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

void main()
{
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;

    gl_PointSize = viewPortSize.y * radius;

    vColor = pointColor;
}


/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core

layout(location = 0) out vec4 fragColor;
uniform float visibility;
in vec4 vColor;
void main()
{
    vec2 centerVec = gl_PointCoord - vec2(0.5F);
    float coordLength = length(centerVec);
    float fade = pow(sqrt(1.05 - coordLength), 32);
    fragColor = vec4(vColor.rgb, fade * visibility);
    if (fragColor.a <= 0.01F)discard;
}