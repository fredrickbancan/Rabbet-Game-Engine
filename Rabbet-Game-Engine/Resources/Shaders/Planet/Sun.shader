//This shader will be for a simple point sun
#shader vertex
#version 330 core

layout(location = 0) in vec4 spritePos;
layout(location = 1) in vec4 spriteColor;
layout(location = 2) in vec3 spriteScale;
layout(location = 3) in vec4 spriteUVMinMax;
layout(location = 4) in float textureIndex;
layout(location = 5) in vec2 axis;//axis of moon orbit
layout(location = 6) in vec2 corner;//instanced quad corner

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 sunPos;
uniform vec2 viewPortSize;
out vec2 coords;
vec4 lookAtZeroRotationNoFlip(float rad)
{
    vec4 endPos = vec4(corner.x * rad * 2, corner.y * rad * 2, 0, 0);
    mat4 result = mat4(0);
    vec3 dir = normalize(-sunPos);
    vec3 up = vec3(axis.x, 0, axis.y);
    vec3 xAxis = cross(up, dir);
    xAxis = normalize(xAxis);
    vec3 yAxis = cross(dir, xAxis);
    yAxis = normalize(yAxis);
    result[0][0] = xAxis.x;
    result[1][0] = yAxis.x;
    result[2][0] = dir.x;

    result[0][1] = xAxis.y;
    result[1][1] = yAxis.y;
    result[2][1] = dir.y;

    result[0][2] = xAxis.z;
    result[1][2] = yAxis.z;
    result[2][2] = dir.z;

    result[3][3] = 1.0;
    return result * endPos + vec4(sunPos, 1);
}

void main()
{
	float sunRadius = 0.075;
    coords = corner * 2.0;

    gl_Position = projectionMatrix * viewMatrix * lookAtZeroRotationNoFlip(sunRadius);
}

#shader fragment
#version 330 core
out vec4 color;
in vec2 coords;
uniform vec3 sunColor;
void main()
{
    float d = dot(coords, coords);
	float fade = smoothstep(0.0, 1.0, pow(sqrt(1.25 - d), 64));
	if (fade <= 0.001) discard;
	color = vec4(sunColor * 1.5, fade);
}