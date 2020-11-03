//shader for rendering point particles Dynamically opaque
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 pointColor;
layout(location = 2) in float radius;
layout(location = 3) in float aoc;
layout(location = 4) in vec2 corner;//instanced quad corner

uniform float fogDensity = 0.0075;
const float fogGradient = 2.5;

out vec4 vColor;
out float visibility;
out vec4 worldPos;
out float rad;
out vec2 coords;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 cameraPos;
uniform float percentageToNextTick;
uniform int frame;
out float fAoc;

mat4 rotationMatrix(vec3 axis, float angle)
{
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;

    return mat4(oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s, 0.0,
        oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s, 0.0,
        oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c, 0.0,
        0.0, 0.0, 0.0, 1.0);
}


vec4 lookAtCamRotation(vec4 spritePos, float rad)
{
    vec3 rightVec = vec3(1, 0, 0);
    vec3 upVec = vec3(0, 1, 0);
    vec3 lookAt = vec3(0, 0, 1);
    vec4 endPos = vec4(corner.x * rad * 2, corner.y * rad * 2, 0, 0);
    vec3 spriteToCamX = cameraPos - spritePos.xyz;

    spriteToCamX.y = 0;

    spriteToCamX = normalize(spriteToCamX);
    vec3 upAux = normalize(cross(lookAt, spriteToCamX));

    float angleCosine = dot(lookAt, spriteToCamX);
    float angleY = acos(angleCosine) * 180 / 3.141;
    mat4 rotX = rotationMatrix(upAux, -radians(angleY));

    vec3 spriteToCamY = cameraPos - spritePos.xyz;

    spriteToCamY = normalize(spriteToCamY);

    angleCosine = dot(spriteToCamX, spriteToCamY);
    float angleX = acos(angleCosine) * 180 / 3.141;
    float f = spriteToCamY.y < 0.0F ? 1.0F : -1.0F;
    return rotX * rotationMatrix(vec3(f, 0.0, 0.0), -radians(angleX)) * endPos + spritePos;
}

void main()
{
    coords = corner * 2.0;
    rad = radius;
    worldPos = position;
    vec4 positionRelativeToCam = viewMatrix * lookAtCamRotation(worldPos, rad);
    gl_Position = projectionMatrix * positionRelativeToCam;

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = exp(-pow((distanceFromCam * fogDensity), fogGradient));
    visibility = clamp(visibility, 0.0, 1.0);

    vColor = pointColor;
    fAoc = aoc;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
layout(location = 0) out vec4 fragColor;
in float visibility;
in vec4 vColor;
in float fAoc;
in vec4 worldPos;
in float rad;
in vec2 coords;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 fogColor;

float ambientOcclusion;//variable for applying a shadowing effect towards the edges of the point to give the illusion of a sphereical shape

void makeSphere()
{
    //clamps fragments to circle shape. 
    float d = dot(coords, coords);

    if (d >= 1.0F)
    {//discard if the vectors length is more than 0.5
        discard;
    }
    float z = sqrt(1.0F - d);
    vec3 normal = vec3(coords, z);
    normal = mat3(transpose(viewMatrix)) * normal;
    vec3 cameraPos = vec3(worldPos) + rad * normal;


    ////Set the depth based on the new cameraPos.
    vec4 clipPos = projectionMatrix * viewMatrix * vec4(cameraPos, 1.0);
    float ndcDepth = clipPos.z / clipPos.w;
    gl_FragDepth = ((gl_DepthRange.diff * ndcDepth) + gl_DepthRange.near + gl_DepthRange.far) / 2.0;
    if (gl_FragDepth < 0.0000001)
    {
        discard;
    }
    //calc ambient occlusion for circle
    if (bool(fAoc))
        ambientOcclusion = sqrt(1.0F - d / 2);
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

    //add fog effect to frag
    fragColor= mix(vec4(fogColor, vColor.a), colorModified, visibility);
}