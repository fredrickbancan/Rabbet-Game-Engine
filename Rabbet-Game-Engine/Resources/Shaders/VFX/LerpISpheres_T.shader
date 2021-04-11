//Shader for rendering point particles Dynamically with transparency
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 pointColor;
layout(location = 2) in float radius;
layout(location = 3) in float aoc;
layout(location = 4) in vec4 prevPosition;
layout(location = 5) in vec4 prevPointColor;
layout(location = 6) in float prevRadius;
layout(location = 7) in float prevAoc;

out vec4 vColor;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
//vector of viewport dimensions
uniform vec2 viewPortSize;
uniform float percentageToNextTick;
uniform int frame;
out float fAoc;

void main()
{
    //lerping position
    vec4 worldPosition = prevPosition + (position - prevPosition) * percentageToNextTick;
    gl_Position = projectionMatrix * viewMatrix * worldPosition;

    //keeps the point size consistent with distance AND resolution. lerp Radius
    gl_PointSize = viewPortSize.y * projectionMatrix[1][1] * (prevRadius + (radius - prevRadius) * percentageToNextTick) / gl_Position.w;//TODO: this does not take into account aspect ratio and can cause points to be elipsical in shape.

    //lerping color
    vColor = mix(prevPointColor, pointColor, percentageToNextTick);
    fAoc = aoc;
}


/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core

layout(location = 0) out vec4 fragColor;
in vec4 vColor;
in float fAoc;

float ambientOcclusion;//variable for applying a shadowing effect towards the edges of the point to give the illusion of a sphereical shape

void makeCircle()
{
    //clamps fragments to circle shape. 
    vec2 centerVec = gl_PointCoord - vec2(0.5F);//get a vector from center of square to coord
    float coordLength = length(centerVec);

    if (coordLength >= 0.5F)
    {//discard if the vectors length is more than 0.5
        discard;
    }

    //calc ambient occlusion for circle
    if (bool(fAoc))
        ambientOcclusion = sqrt(1.0 - coordLength);
}


void main()
{
    makeCircle();

    vec4 colorModified = vColor;
    if (bool(fAoc))
    {
        //add ambient occlusion shading
        colorModified.r *= ambientOcclusion;
        colorModified.g *= ambientOcclusion;
        colorModified.b *= ambientOcclusion;
    }

    if (fragColor.a < 0.01)
    {
        discard;
    }
}