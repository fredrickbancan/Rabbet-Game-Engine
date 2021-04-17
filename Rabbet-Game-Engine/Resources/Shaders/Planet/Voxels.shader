#shader vertex
#version 330 core
layout(location = 0) in uint data;
layout(location = 1) in uint id;
const float voxelSize = 0.25F;
const float halfVoxelSize = voxelSize * 0.5F;
const float maxVoxelLuminance = 1.25F;
const uint chunkSize = 64U;
const uint chunkSizeMinusOne = 63U;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform vec3 camPos;
out float lightLevel;

const int[] cornerIDs = int[](0, 1, 2, 2, 3, 0);
const vec3[] testOffsets = vec3[]
(
    vec3(0.5F, 0.5F,0),
    vec3(-0.5F, 0.5F,0),
    vec3(-0.5F,-0.5F,0),
    vec3(0.5F,-0.5F,0)
    );

vec3 unpackChunkPos()
{
    uint index = (data & 0xFFFFC000U) >> 14;
    uint x = index >> 12;
    uint z = (index >> 6) & chunkSizeMinusOne;
    uint y = index & chunkSizeMinusOne;
    return vec3(x, y, z) ;
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
    int cornerID = cornerIDs[gl_VertexID % 6];
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4((unpackChunkPos() + testOffsets[cornerID]) * voxelSize + halfVoxelSize, 1.0F);
    gl_PointSize = 100;
    lightLevel =  float(unpackLightLevel() + 1) / 64.0 * maxVoxelLuminance;
}


/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
layout(location = 0) out vec4 fragColor;
in float lightLevel;

void main()
{
    fragColor = vec4(lightLevel, lightLevel, lightLevel,1.0);
}