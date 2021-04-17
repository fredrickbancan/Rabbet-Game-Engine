#shader vertex
#version 330 core
layout(location = 0) in uint data;
//layout(location = 1) in byte id;
const float voxelSize = 0.25F;
const float halfVoxelSize = 0.125F;
const float maxVoxelLuminance = 1.25F;
const uint chunkSize = 64U;
const uint chunkSizeMinusOne = 63U;
out float lightLevel;
out vec3 worldPos;
out int orientation;

vec3 unpackChunkPos()
{
    uint index = (data & 0xFFFFC000U) >> 14U;
    uint x = index >> 12U;
    uint z = (index >> 6U) & chunkSizeMinusOne;
    uint y = index & chunkSizeMinusOne;
    return vec3(x, y, z);
}

int unpackLightLevel()
{
    return int((data & 0x00003F00U) >> 8);
}

int unpackMetadata()
{
    return int((data & 0x000000E0U) >> 5);
}

int unpackOrientation()
{
    return int((data & 0x0000001CU) >> 2);
}

void main()
{
    worldPos = unpackChunkPos() * voxelSize;
    lightLevel =  float(unpackLightLevel() + 1) / 64.0 * maxVoxelLuminance;
    orientation = unpackOrientation() * 4;//pre-multplying by 4 for indexing offsets
}

/*#############################################################################################################################################################################################*/
#shader tesscontrol
#version 410
layout(vertices = 4) out;
const float voxelSize = 0.25F;
const float halfVoxelSize = 0.125F;

const vec3[] faceOffsets = vec3[]
(
    //posX face
    vec3(0.5, 1, 1) * halfVoxelSize,
    vec3(0.5, 1, -1)* halfVoxelSize,
    vec3(0.5, -1, 1)* halfVoxelSize,
    vec3(0.5, -1, -1)* halfVoxelSize,

    //posY face
    vec3(1, 0.5, 0)* halfVoxelSize,
    vec3(-1, 0.5, 0)* halfVoxelSize,
    vec3(1, 0.5, 0)* halfVoxelSize,
    vec3(-1, 0.5, 0)* halfVoxelSize,

    //posZ face
    vec3(1, 1, 0.5)* halfVoxelSize,
    vec3(-1, 1, 0.5)* halfVoxelSize,
    vec3(1, -1, 0.5)* halfVoxelSize,
    vec3(-1, -1, 0.5)* halfVoxelSize,

    //negX face
    vec3(-0.5, 1, 0)* halfVoxelSize,
    vec3(-0.5, 1, 0)* halfVoxelSize,
    vec3(-0.5, -1, 0)* halfVoxelSize,
    vec3(-0.5, -1, 0)* halfVoxelSize,

    //negY face
    vec3(1, -0.5, 0)* halfVoxelSize,
    vec3(-1, -0.5, 0)* halfVoxelSize,
    vec3(1, -0.5, 0)* halfVoxelSize,
    vec3(-1, -0.5, 0)* halfVoxelSize,

    //negZ face
    vec3(1, 1, -0.5)* halfVoxelSize,
    vec3(-1, 1, -0.5)* halfVoxelSize,
    vec3(1, -1, -0.5)* halfVoxelSize,
    vec3(-1, -1, -0.5)* halfVoxelSize
    );

in float lightLevel[];
in vec3 worldPos[];
in int orientation[];

out Tess
{
    vec3 worldPos;
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
    Out[gl_InvocationID].worldPos = worldPos[0] + faceOffsets[orientation[0] + gl_InvocationID];
}

/*#############################################################################################################################################################################################*/
#shader tessevaluation
#version 410
layout(quads) in;
uniform mat4 projViewModel;
out float lightLevel;
in Tess
{
    vec3 worldPos;
    float lightLevel;
} In[];

void main(void)
{
    vec3 wpTop = mix(In[0].worldPos, In[1].worldPos, gl_TessCoord.x);
    vec3 wpBottom = mix(In[2].worldPos, In[3].worldPos, gl_TessCoord.x);
    vec3 newWorldPos = mix(wpTop, wpBottom, gl_TessCoord.y);
    gl_Position = projViewModel * vec4(newWorldPos, 1.0F);
    lightLevel = In[0].lightLevel;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
layout(location = 0) out vec4 fragColor;
in float lightLevel;

void main()
{
    fragColor = vec4(lightLevel, lightLevel, lightLevel,1.0);
    //fragColor = vec4(1.0);
}