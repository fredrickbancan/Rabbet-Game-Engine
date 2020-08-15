#shader vertex
#version 330 core
//location is the location of the value in the vertex atrib array
//for vec4 position, the gpu automatically fills in the 4th component with a 1.0F. This means you can treat position as a vec4 no problem. (no need for messy conversions)
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 colour;

const float fogDensity = 0.015;
const float fogGradient = 3.5;

out vec4 vcolour;
out float visibility;//for fog

out float radiusPixels;
out float radius;

//matrix for projection transformations.
uniform mat4 projectionMatrix;
//matrix for camera transformations.
uniform mat4 viewMatrix;
//matrix for model transformations. All transformations in this matrix are relative to the model origin.
uniform mat4 modelMatrix;
//vector of viewport dimensions
uniform vec2 viewPortSize;

float pointRadius = 0.25;
float positionResolution = 256.0F;
float innacuracyOverDistanceFactor = 1.0F;
void main()
{
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;
    //keeps the point size consistent with distance AND resolution.
    gl_PointSize = viewPortSize.y * projectionMatrix[1][1] * pointRadius / gl_Position.w;
    //position jitter for retro feel
    gl_Position.xy = floor(gl_Position.xy * (positionResolution / (gl_Position.w * innacuracyOverDistanceFactor))) / (positionResolution / (gl_Position.w * innacuracyOverDistanceFactor));
    radiusPixels = gl_PointSize / 2.0;
    radius = pointRadius;

    vcolour = colour;
}


/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330 core

layout(location = 0) out vec4 color;

//radius of points in pixels
in float radiusPixels;
in float radius;
in float visibility;
in vec4 vcolour;

uniform vec3 fogColour;
uniform bool aoc = false;

float ambientOcclusion;//variable for applying a shadowing effect towards the edges of the point to give the illusion of a sphereical shape

void makeCircle()
{
    //clamps fragments to circle shape. 
    vec2 centerVec = gl_PointCoord - vec2(0.5);//get a vector from center of square to coord
    float coordLength = length(centerVec);
    if (coordLength > 0.5)                  //discard if the vectors length is more than 0.5
        discard;

    //calc ambient occlusion for circle
    if(aoc)
    ambientOcclusion = sqrt(1.0 - coordLength);
}


void main()
{
    makeCircle();
    color = vec4(vcolour.xyz, 1.0);

    if (aoc)
    {
        //add ambient occlusion shading
        color.r *= ambientOcclusion;
        color.g *= ambientOcclusion;
        color.b *= ambientOcclusion;
    }

    //add fog effect to frag
    color = mix(vec4(fogColour, color.a), color, visibility);
}