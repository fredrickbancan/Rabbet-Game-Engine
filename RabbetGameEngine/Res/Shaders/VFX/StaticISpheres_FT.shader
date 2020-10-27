//shader for rendering point particles with transparency
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
out float rad;
out vec4 worldPos;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
//vector of viewport dimensions
uniform vec2 viewPortSize;
uniform float percentageToNextTick;
uniform int frame;
out float fAoc;
void main()
{
    worldPos = position;
    rad = radius;
    vec4 positionRelativeToCam = viewMatrix * position;
    gl_Position = projectionMatrix * positionRelativeToCam;

    //keeps the point size consistent with distance AND resolution.
    gl_PointSize = viewPortSize.y * projectionMatrix[1][1] * radius / gl_Position.w;

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = exp(-pow((distanceFromCam * fogDensity), fogGradient));
    visibility = clamp(visibility, 0.0, 1.0);

    vColor = pointColor;
    fAoc = aoc;
}


/*#############################################################################################################################################################################################*/
#shader fragment
#version 330
#extension GL_ARB_conservative_depth : enable
layout(depth_less) out float gl_FragDepth;
layout(location = 0) out vec4 fragColor;
in float visibility;
in vec4 vColor;
in float fAoc;
in vec4 worldPos;
in float rad;
uniform vec3 fogColor;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

float ambientOcclusion;//variable for applying a shadowing effect towards the edges of the point to give the illusion of a sphereical shape

void makeSphere()
{
    //clamps fragments to circle shape. 
    vec2 mapping = gl_PointCoord * 2.0F - 1.0F;
    float d = dot(mapping, mapping);

    if (d >= 1.0F)
    {//discard if the vectors length is more than 0.5
        discard;
    }
    float z = sqrt(1.0F - d);
    vec3 normal = vec3(mapping, z);
    normal = mat3(transpose(viewMatrix)) * normal;
    vec3 cameraPos = vec3(worldPos) + rad * normal;


    ////Set the depth based on the new cameraPos.
    vec4 clipPos = projectionMatrix * viewMatrix * vec4(cameraPos, 1.0);
    float ndcDepth = clipPos.z / clipPos.w;
    gl_FragDepth = ((gl_DepthRange.diff * ndcDepth) + gl_DepthRange.near + gl_DepthRange.far) / 2.0;

    //calc ambient occlusion for circle
    if (bool(fAoc))
        ambientOcclusion = sqrt(1.0F - d * 0.5F);
}


void main()
{
    makeSphere();

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