//This shader will be for a simple point sun
#shader vertex
#version 330 core
layout(location = 0) in vec4 spritePos;
layout(location = 1) in vec4 spriteColor;
layout(location = 2) in vec3 spriteScale;
layout(location = 3) in vec4 spriteUVMinMax;
layout(location = 4) in vec2 corner;//instanced quad corner
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
out vec4 vColor;
out vec2 uv;
out vec2 coords;
uniform vec3 sunDir;

vec4 lookAtZeroRotationNoFlip(float rad)
{
    vec4 endPos = vec4(corner.x * rad * 2, corner.y * rad * 2, 0, 0);
    mat4 result = mat4(0);
    vec3 dir = -spritePos.xyz;
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
    coords = corner * 2.0;
    uv = vec2(0, 0);
    uv.x = corner.x < 0.0 ? spriteUVMinMax.x : spriteUVMinMax.z;
    uv.y = corner.y < 0.0 ? -spriteUVMinMax.y : -spriteUVMinMax.w;
    vColor = spriteColor;
    float d = 1 - (dot(sunDir, spritePos.xyz) + 1) * 0.5F;
    vColor.a *= (d * d) * 1.25F;
	gl_Position = projectionMatrix * viewMatrix * lookAtZeroRotationNoFlip(spriteScale.x);
}

#shader fragment
#version 330 core
out vec4 color;
in vec4 vColor;
in vec2 uv;
in vec2 coords;
uniform sampler2D uTexture;
void main()
{
    float d = dot(coords, coords);

    if (d >= 1.0F)
    {
        discard;
    }
    vec4 textureColor = texture(uTexture, uv) * vColor;
    color = textureColor;
}