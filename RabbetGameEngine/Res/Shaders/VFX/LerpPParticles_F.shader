//shader for rendering point particles Dynamically opaque
#shader vertex
#version 330 
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 pointColor;
layout(location = 2) in float radius;
layout(location = 3) in float aoc;
layout(location = 4) in vec4 prevPosition;
layout(location = 5) in vec4 prevPointColor;
layout(location = 6) in float prevRadius;
layout(location = 7) in float prevAoc;


uniform float fogDensity = 0.0075;
const float fogGradient = 2.5;

out vec4 vColor;
out float visibility;
out vec4 worldPos;
out float rad;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
//vector of viewport dimensions
uniform vec2 viewPortSize;
uniform float percentageToNextTick;
uniform int frame;
out float fAoc;
void main()
{
    //lerping radius
    rad = (prevRadius + (radius - prevRadius) * percentageToNextTick);

    //lerping position
    worldPos = prevPosition + (position - prevPosition) * percentageToNextTick;

    vec4 positionRelativeToCam = viewMatrix * worldPos;
    gl_Position = projectionMatrix * positionRelativeToCam;
    //keeps the point size consistent with distance AND resolution. Lerp radius.
    gl_PointSize = viewPortSize.y * projectionMatrix[1][1] * rad / gl_Position.w;//TODO: this does not take into account aspect ratio and can cause points to be elipsical in shape.

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = exp(-pow((distanceFromCam * fogDensity), fogGradient));
    visibility = clamp(visibility, 0.0, 1.0);

    //lerping color
    vColor = mix(prevPointColor, pointColor, percentageToNextTick);
    fAoc = aoc;
}


/*#############################################################################################################################################################################################*/
#shader fragment
#version 330

layout(location = 0) out vec4 fragColor;
in float visibility;
in vec4 vColor;
in float fAoc;
in vec4 worldPos;
in float rad;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 fogColor;

float ambientOcclusion;//variable for applying a shadowing effect towards the edges of the point to give the illusion of a sphereical shape

void makeSphere()
{
    //clamps fragments to circle shape. 
    vec2 centerVec = gl_PointCoord - vec2(0.5F);//get a vector from center of square to coord
    float coordLength = length(centerVec);

    if (coordLength >= 0.5F)
    {//discard if the vectors length is more than 0.5
        discard;
    }
    vec2 mapping = gl_PointCoord * 2.0F - 1.0F;
    vec3 cameraSpherePos = vec3(viewMatrix * worldPos);
    vec3 cameraPlanePos = vec3(mapping * rad, 0.0F) + cameraSpherePos;
    vec3 rayDirection = normalize(cameraPlanePos);

    float B = 2.0 * dot(rayDirection, -cameraSpherePos);
    float C = dot(cameraSpherePos, cameraSpherePos) - (rad * rad);

    float det = (B * B) - (4 * C);
    if (det < 0.0)
        discard;

    float sqrtDet = sqrt(det);
    float posT = (-B + sqrtDet) / 2;
    float negT = (-B - sqrtDet) / 2;

    float intersectT = min(posT, negT);

    vec3 cameraPos = rayDirection * intersectT;
    vec3 cameraNormal = normalize(cameraPos - cameraSpherePos);

    //Set the depth based on the new cameraPos.
    vec4 clipPos = projectionMatrix * vec4(cameraPos, 1.0);
    float ndcDepth = clipPos.z / clipPos.w;
    gl_FragDepth = ((gl_DepthRange.diff * ndcDepth) + gl_DepthRange.near + gl_DepthRange.far) / 2.0;

    //calc ambient occlusion for circle
    if(bool(fAoc))
    ambientOcclusion = sqrt(1.0 - coordLength);
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