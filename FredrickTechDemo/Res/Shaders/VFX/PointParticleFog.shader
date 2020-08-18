#shader vertex
#version 330 
//location is the location of the value in the vertex atrib array
//for vec4 position, the gpu automatically fills in the 4th component with a 1.0F. This means you can treat position as a vec4 no problem. (no need for messy conversions)
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 colour;
layout(location = 2) in vec2 texCoord;

uniform float fogDensity = 0.0075;
const float fogGradient = 3.5;

out vec4 vcolour;
out float visibility;//for fog

//matrix for projection transformations.
uniform mat4 projectionMatrix;
//matrix for camera transformations.
uniform mat4 viewMatrix;
//matrix for model transformations. All transformations in this matrix are relative to the model origin.
uniform mat4 modelMatrix;
//vector of viewport dimensions
uniform vec2 viewPortSize;

uniform float pointRadius = 0.1F;
float positionResolution = 256.0F;
void main()
{
    vec4 worldPosition = modelMatrix * position;
    vec4 positionRelativeToCam = viewMatrix * worldPosition;
    gl_Position = projectionMatrix * positionRelativeToCam;

    //keeps the point size consistent with distance AND resolution.
    gl_PointSize = viewPortSize.y * projectionMatrix[1][1] * pointRadius / gl_Position.w;

    //position jitter for retro feel
    float aspectRatio = viewPortSize.x / viewPortSize.y;
    gl_Position.x = floor(gl_Position.x * (positionResolution / gl_Position.w)) / (positionResolution / gl_Position.w);
    gl_Position.y = floor(gl_Position.y * (positionResolution / (gl_Position.w * aspectRatio))) / (positionResolution / (gl_Position.w  * aspectRatio));

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = exp(-pow((distanceFromCam * fogDensity), fogGradient));
    visibility = clamp(visibility, 0.0, 1.0);

    vcolour = colour;
}


/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330

layout(location = 0) out vec4 color;
in float visibility;
in vec4 vcolour;

uniform vec3 fogColour;
uniform bool aoc;
//vector of viewport dimensions
uniform vec2 viewPortSize;

float ambientOcclusion;//variable for applying a shadowing effect towards the edges of the point to give the illusion of a sphereical shape

bool xor(bool a, bool b)
{
    if (a && b)
    {
        return false;
    }
    return a || b;
}

float hash(uint n)//returns random float value from 0 to 1
{
    n = (n << 13U) ^ n;
    n = n * (n * n * 15731U + 789221U) + 1376312589U;
    return float(n & uvec3(0x7fffffffU)) / float(0x7fffffff);
}

void makeCircle()
{
    //clamps fragments to circle shape. 
    vec2 centerVec = gl_PointCoord - vec2(0.5);//get a vector from center of square to coord
    float coordLength = length(centerVec);

    if (coordLength > 0.5)
    {//discard if the vectors length is more than 0.5
        discard;
    }

    //calc ambient occlusion for circle
    if(aoc)
    ambientOcclusion = sqrt(1.0 - coordLength);
}


void main()
{
    makeCircle();

    if (aoc)
    {
        //add ambient occlusion shading
        color.r *= ambientOcclusion;
        color.g *= ambientOcclusion;
        color.b *= ambientOcclusion;
    }

    uint fragX = uint(gl_FragCoord.x);
    uint fragY = uint(gl_FragCoord.y);
    float randomFloat = hash(fragX + uint(viewPortSize.x) * fragY + (uint(viewPortSize.x) * uint(viewPortSize.y)) * uint(gl_FragCoord.z * 1376312589));
    
    if (randomFloat > vcolour.a)//do stochastic transparency, noise can be reduced with sampling. 
    {
        discard;
    }

    //add fog effect to frag
    color = mix(vec4(fogColour, 1.0), vec4(vcolour.rgb, 1.0), visibility);
}