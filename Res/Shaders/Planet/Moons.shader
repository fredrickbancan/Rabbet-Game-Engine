//This shader will be for a simple point sun
#shader vertex
#version 330 core
layout(location = 0) in vec4 spritePos;
layout(location = 1) in vec4 spriteColor;
layout(location = 2) in vec3 spriteScale;
layout(location = 3) in vec4 spriteUVMinMax;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec2 viewPortSize;
out vec4 vColor;
out vec4 sUVMinMax;
uniform vec3 sunDir;
void main()
{
    sUVMinMax = spriteUVMinMax;
    vColor = spriteColor;
    float d = 1 - (dot(sunDir, spritePos.xyz) + 1) * 0.5F;
    vColor.a *= (d * d) * 1.25F;
	gl_Position = projectionMatrix * viewMatrix * spritePos;
	gl_PointSize = spriteScale.x * viewPortSize.y;
}

#shader fragment
#version 330 core
out vec4 color;
in vec4 vColor;
in vec4 sUVMinMax;
uniform sampler2D uTexture;
void main()
{
    vec2 centerVec = gl_PointCoord - vec2(0.5F);
    float coordLength = length(centerVec);
    if (coordLength >= 0.5F) discard;
    
    vec2 uv = vec2(0, 0);
    uv.x = sUVMinMax.x + (sUVMinMax.z - sUVMinMax.x) * gl_PointCoord.x;
    uv.y = -(sUVMinMax.y + (sUVMinMax.w - sUVMinMax.y) * gl_PointCoord.y);
    vec4 textureColor = texture(uTexture, uv) * vColor;
    color = textureColor;
}