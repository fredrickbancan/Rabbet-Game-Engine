//shader for rendering point particles Dynamically opaque
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


uniform float fogStart = 1000.0;
uniform float fogEnd = 1000.0;

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
    //lerping position
    vec4 worldPosition = prevPosition + (position - prevPosition) * percentageToNextTick;
    vec4 positionRelativeToCam = viewMatrix * worldPosition;
    gl_Position = projectionMatrix * positionRelativeToCam;

    //keeps the point size consistent with distance AND resolution. Lerp radius.
    gl_PointSize = viewPortSize.y * projectionMatrix[1][1] * (prevRadius + (radius - prevRadius) * percentageToNextTick) / gl_Position.w;//FIX: this does not take into account aspect ratio and can cause points to be elipsical in shape.

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = (distanceFromCam - fogStart) / (fogEnd - fogStart);
    visibility = clamp(visibility, 0.0, 1.0);
    visibility = 1.0 - visibility;
    visibility *= visibility;

    //lerping color
    vColor = mix(prevPointColor, pointColor, percentageToNextTick);
    fAoc = aoc;
}


/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core

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
    if (bool(fAoc))
        ambientOcclusion = sqrt(1.0 - coordLength);
}


void main()
{
    makeCircle();

    vec3 colorModified = vColor.rgb;
    if (bool(fAoc))
    {
        //add ambient occlusion shading
        colorModified.r *= ambientOcclusion;
        colorModified.g *= ambientOcclusion;
        colorModified.b *= ambientOcclusion;
    }

    //add fog effect to frag
    fragColor.rgb = mix(fogColor, colorModified, visibility);
    fragColor.a = 1;
}