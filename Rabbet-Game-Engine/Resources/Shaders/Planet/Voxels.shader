#shader vertex
#version 330 core 
layout(location = 0) in uint data;
layout(location = 1) in uint id;
const float voxelSize = 0.25F;
const float halfVoxelSize = 0.125F;
const float maxVoxelLuminance = 1.25F;
const float lightLevelMultiplier = 0.01953125F;// this value * 64 = maxVoxelLuminance
const float voxelUvScale = 0.0625F;
const uint chunkSize = 64U;
const uint chunkSizeMinusOne = 63U;
out float lightLevel;
out vec3 worldPos;
out int orientation;
out vec2 faceUV;

uint unpackLightLevel()
{
    return (data & 0x00003F00U) >> 8;
}

uint unpackMetadata()
{
    return (data & 0x000000E0U) >> 5;
}

uint unpackOrientation()
{
    return (data & 0x0000001CU) >> 2;
}

void main()
{
    uint index = (data & 0xFFFFC000U) >> 14U;
    worldPos = vec3(index >> 12U, index & chunkSizeMinusOne, (index >> 6U) & chunkSizeMinusOne) * voxelSize;
    lightLevel =  float(unpackLightLevel() + 1U) * lightLevelMultiplier;
    orientation = int(unpackOrientation()) * 4;//pre-multplying by 4 for indexing offsets

    int voxID =  int(id & 255U);
    faceUV = vec2(voxID & 15, voxID >> 4) * voxelUvScale;
    faceUV.y = 1.0F - faceUV.y;//flip y
}

/*#############################################################################################################################################################################################*/
#shader tesscontrol
#version 410
layout(vertices = 4) out;
const float voxelSize = 0.25F;
const float halfVoxelSize = 0.125F;
const float voxelUvScale = 0.0625F;

const vec3[] faceOffsets = vec3[]
(
    //posX face
    vec3(1, 1, -1) * halfVoxelSize,
    vec3(1, 1, 1)* halfVoxelSize,
    vec3(1, -1, -1)* halfVoxelSize,
    vec3(1, -1, 1)* halfVoxelSize,

    //posY face
    vec3(1, 1, 1)* halfVoxelSize,
    vec3(1, 1, -1)* halfVoxelSize,
    vec3(-1, 1, 1)* halfVoxelSize,
    vec3(-1, 1, -1)* halfVoxelSize,

    //posZ face
    vec3(1, 1, 1)* halfVoxelSize,
    vec3(-1, 1, 1)* halfVoxelSize,
    vec3(1, -1, 1)* halfVoxelSize,
    vec3(-1, -1, 1)* halfVoxelSize,

    //negX face
    vec3(-1, 1, 1)* halfVoxelSize,
    vec3(-1, 1, -1)* halfVoxelSize,
    vec3(-1, -1, 1)* halfVoxelSize,
    vec3(-1, -1, -1)* halfVoxelSize,

    //negY face
    vec3(-1, -1, 1)* halfVoxelSize,
    vec3(-1, -1, -1)* halfVoxelSize,
    vec3(1, -1, 1)* halfVoxelSize,
    vec3(1, -1, -1)* halfVoxelSize,

    //negZ face
    vec3(-1, 1, -1)* halfVoxelSize,
    vec3(1, 1, -1)* halfVoxelSize,
    vec3(-1, -1, -1)* halfVoxelSize,
    vec3(1, -1, -1)* halfVoxelSize
    );

const vec2[] uvOffsets = vec2[]
(
    vec2(1, 0) * voxelUvScale,
    vec2(0, 0) * voxelUvScale,
    vec2(1, -1) * voxelUvScale,
    vec2(0, -1) * voxelUvScale
    );

in float lightLevel[];
in vec3 worldPos[];
in vec2 faceUV[];
in int orientation[];
out Tess
{
    vec3 worldPos;
    vec2 uv;
    float lightLevel;
} Out[];

void main(void)
{
    if (gl_InvocationID == 0)
    {
        gl_TessLevelInner[0] = 0;
        gl_TessLevelInner[1] = 0;
        gl_TessLevelOuter[0] = 1;
        gl_TessLevelOuter[1] = 1;
        gl_TessLevelOuter[2] = 1;
        gl_TessLevelOuter[3] = 1;
    }
    Out[gl_InvocationID].lightLevel = lightLevel[0];
    Out[gl_InvocationID].uv = faceUV[0] + uvOffsets[gl_InvocationID];
    Out[gl_InvocationID].worldPos = worldPos[0] + faceOffsets[orientation[0] + gl_InvocationID];
}

/*#############################################################################################################################################################################################*/
#shader tessevaluation
#version 410
layout(quads) in;
uniform mat4 projViewModel;
out float lightLevel;
out vec2 uv;
in Tess
{
    vec3 worldPos;
    vec2 uv;
    float lightLevel;
} In[];

void main(void)
{
    gl_Position = projViewModel * vec4(In[int(gl_TessCoord.x) + int(gl_TessCoord.y) * 2].worldPos, 1.0F);
    uv = In[int(gl_TessCoord.x) + int(gl_TessCoord.y) * 2].uv;
    lightLevel = In[0].lightLevel;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
layout(location = 0) out vec4 fragColor;
in float lightLevel;
in vec2 uv;
uniform sampler2D voxelTexture; 
void main()
{
    fragColor = texture2D(voxelTexture, uv);
    fragColor.r *= lightLevel;
    fragColor.g *= lightLevel;
    fragColor.b *= lightLevel;
   // fragColor = vec4(lightLevel, lightLevel, lightLevel, 1.0F);
    if (fragColor.a < 0.01F)discard;
    fragColor.a = 1;
}