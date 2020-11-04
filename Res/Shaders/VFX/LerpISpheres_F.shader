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
layout(location = 8) in vec2 corner;//instanced quad corner


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
//vector of viewport dimensions
uniform vec2 viewPortSize;
uniform float percentageToNextTick;
uniform int frame;
out float fAoc;
void main()
{
    coords = corner * 2.0;
    //lerping radius
    rad = (prevRadius + (radius - prevRadius) * percentageToNextTick);

    //lerping pos
    worldPos = (prevPosition + (position - prevPosition) * percentageToNextTick);
    //create translation
    mat4 modelMatrix = mat4(1.0F);
    modelMatrix[3][0] = worldPos.x;
    modelMatrix[3][1] = worldPos.y;
    modelMatrix[3][2] = worldPos.z;

    //remove rotation
    mat4 modelViewBillboard = viewMatrix * modelMatrix;
    modelViewBillboard[0][0] = 1;
    modelViewBillboard[0][1] = 0;
    modelViewBillboard[0][2] = 0;
    modelViewBillboard[1][0] = 0;
    modelViewBillboard[1][1] = 1;
    modelViewBillboard[1][2] = 0;
    modelViewBillboard[2][0] = 0;
    modelViewBillboard[2][1] = 0;
    modelViewBillboard[2][2] = 1;
    vec4 positionRelativeToCam = modelViewBillboard * vec4(corner.x * rad * 2, corner.y * rad * 2, 0, 1);
    gl_Position = projectionMatrix * positionRelativeToCam;

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = exp(-pow((distanceFromCam * fogDensity), fogGradient));
    visibility = clamp(visibility, 0.0, 1.0);

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
    //calc ambient occlusion for circle
    if (bool(fAoc))
        ambientOcclusion = sqrt(1.0F - d / 2);
}


void main()
{
    makeSphere();
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