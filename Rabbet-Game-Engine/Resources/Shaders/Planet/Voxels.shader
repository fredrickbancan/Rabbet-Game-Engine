#shader vertex
#version 330 core
layout(location = 0) in uint data;
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

int unpackVoxelID()
{
    return int(data >> 24U);
}

vec3 unpackChunkPos()
{
    uint index = (data & uint(0x00FFFFFF)) >> 6U; 
    uint x = index >> 12U;
    uint z = (index >> 6U) & chunkSizeMinusOne;
    uint y = index & chunkSizeMinusOne;
    return vec3(x, y, z) * voxelSize + halfVoxelSize;
}

int unpackLightLevel()
{
    return int(data & uint(0x3F));
}


void main()
{
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(unpackChunkPos(), 1.0F);
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