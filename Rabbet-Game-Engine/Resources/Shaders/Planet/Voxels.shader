#shader vertex
#version 330 core
layout(location = 0) in uint data;

const float voxelSize = 0.25F;
const float halfVoxelSize = voxelSize * 0.5F;
const uint chunkSize = 64U;
const uint chunkSizeMinusOne = 63U;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

int unpackVoxelID()
{
    return int(data >> 24U);
}

vec3 unpackWorldPos()
{
    uint index = (data & uint(0x00FFFFFF)) >> 6U;
    float x = float((index >> (chunkSize * chunkSize)) & chunkSizeMinusOne);
    float z = float((index >> chunkSize) & chunkSizeMinusOne);
    float y = float(index & chunkSizeMinusOne);
    return vec3(x * voxelSize + halfVoxelSize, y * voxelSize + halfVoxelSize, z * voxelSize + halfVoxelSize);
}

int unpackLightLevel()
{
    return int(data & uint(0x3F));
}


void main()
{
    gl_PointSize = 107;
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4( 1.0F);
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
layout(location = 0) out vec4 fragColor;

void main()
{
    fragColor = vec4(1.0F);
}