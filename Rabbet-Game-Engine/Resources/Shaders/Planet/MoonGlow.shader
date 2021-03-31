//This shader will be for gl_points on the moons to emulate moon glow
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
out vec4 vColor;
out vec2 coords;
uniform vec3 sunDir;
uniform vec2 viewPortSize;

vec4 lookAtZeroRotationNoFlip(float rad)
{
    vec4 endPos = vec4(corner.x * rad * 2, corner.y * rad * 2, 0, 0);
    mat4 result = mat4(0);
    vec3 dir = normalize(-spritePos.xyz);
    vec3 up = vec3(0, 1, 0);
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
    return result * endPos + spritePos;
}

void main()
{
	gl_Position = projectionMatrix * viewMatrix * lookAtZeroRotationNoFlip(spriteScale.x * 4);
    coords = corner * 2;
    vColor = spriteColor;
    float d = 1 - (dot(sunDir, spritePos.xyz) + 1) * 0.5F;
    vColor.a = pow(d, 4);
    float h = (sunDir.y + 1) * 0.5F;
    vColor.a *= 1 - clamp(h * h * 2.0, 0, 1);
}

#shader fragment
#version 330 core

in vec2 coords;
out vec4 color;
in vec4 vColor;
void main()
{
    float coordLength = dot(coords, coords);
    color = vColor;
    color.a *= pow(0.9 - sqrt(coordLength ), 3);
}