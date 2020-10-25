﻿//shader for rendering point particles with transparency
#shader vertex
#version 330
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 pointColor;
layout(location = 2) in float radius;
layout(location = 3) in float aoc;


uniform float fogDensity = 0.0075;
const float fogGradient = 2.5;

out vec4 vColor;
out float visibility;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
//vector of viewport dimensions
uniform vec2 viewPortSize;
uniform float percentageToNextTick;
uniform int frame;
out float fAoc;
void main()
{
    vec4 positionRelativeToCam = viewMatrix * position;
    gl_Position = projectionMatrix * positionRelativeToCam;

    //keeps the point size consistent with distance AND resolution.
    gl_PointSize = viewPortSize.y * projectionMatrix[1][1] * radius / gl_Position.w;//TODO: this does not take into account aspect ratio and can cause points to be elipsical in shape.

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = exp(-pow((distanceFromCam * fogDensity), fogGradient));
    visibility = clamp(visibility, 0.0, 1.0);

    vColor = pointColor;
    fAoc = aoc;
}


/*#############################################################################################################################################################################################*/
#shader fragment
#version 330

layout(location = 0) out vec4 fragColor;
in float visibility;
in vec4 vColor;
in float fAoc;

uniform vec3 fogColor;


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
    if(bool(fAoc))
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

    fragColor = mix(vec4(fogColor, colorModified.a), colorModified, visibility);
    if (fragColor.a < 0.01)
    {
        discard;
    }
}