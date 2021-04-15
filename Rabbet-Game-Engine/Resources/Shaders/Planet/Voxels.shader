#shader vertex
#version 330 core
layout(location = 0) in uint data;
const float voxelSize = 0.25F;
const float halfVoxelSize = voxelSize * 0.5F;
const float maxVoxelLuminance = 1.5F;
const uint chunkSize = 64U;
const uint chunkSizeMinusOne = 63U;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform vec3 camPos;
out vec3 worldPos;
out vec3 camToVoxelDir;
out float lightLevel;

int unpackVoxelID()
{
    return int(data >> 24U);
}

vec3 unpackChunkPos()
{
   // uint index = (data & uint(0x00FFFFFF)) >> 6U;
    uint index = data >> 6U;
    uint x = index >> 12U;
    uint z = (index >> 6U) & chunkSizeMinusOne;
    uint y = index;
    return vec3(0, y, 0) * voxelSize + halfVoxelSize;
}

int unpackLightLevel()
{
    return int(data & uint(0x3F));
}


void main()
{
    worldPos = (modelMatrix * vec4(unpackChunkPos(), 1.0F)).xyz;
    camToVoxelDir = normalize(worldPos - camPos);
    lightLevel =  float(unpackLightLevel() + 1) / 64.0 * maxVoxelLuminance;
}

/*#############################################################################################################################################################################################*/
#shader geometry
#version 330 core
layout(points) in;
layout(triangle_strip, max_vertices = 12) out;
const float voxelSize = 0.25F;
const float halfVoxelSize = voxelSize * 0.5F;
const uint chunkSize = 64U;
const uint chunkSizeMinusOne = 63U;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
in vec3 camToVoxelDir[];
in vec3 worldPos[];
in float lightLevel[];
out float light;
void main() 
{
    mat4 projView = projectionMatrix * viewMatrix;
    vec3 camToVox = camToVoxelDir[0];
    vec3 wPos = worldPos[0];
    light = lightLevel[0];
    if (camToVox.x > 0.0001F)
    {//west face
        vec3 centerEast = wPos - vec3(halfVoxelSize, 0.0, 0.0);
        gl_Position = projView * vec4(centerEast + vec3(0.0, -halfVoxelSize, -halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerEast + vec3(0.0, -halfVoxelSize, halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerEast + vec3(0.0, halfVoxelSize, -halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerEast + vec3(0.0, halfVoxelSize, halfVoxelSize), 1.0);
        EmitVertex();
        EndPrimitive();
    }
    else
    {//East face
        vec3 centerWest = wPos + vec3(halfVoxelSize, 0.0, 0.0);
        gl_Position = projView * vec4(centerWest + vec3(0.0, -halfVoxelSize, halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerWest + vec3(0.0, -halfVoxelSize, -halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerWest + vec3(0.0, halfVoxelSize, halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerWest + vec3(0.0, halfVoxelSize, -halfVoxelSize), 1.0);
        EmitVertex();
        EndPrimitive();
    }
    
    if (camToVox.z > 0.0001F)
    {//south face
        vec3 centerSouth = wPos - vec3(0.0, 0.0, halfVoxelSize);
        gl_Position = projView * vec4(centerSouth + vec3(-halfVoxelSize, halfVoxelSize, 0.0), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerSouth + vec3(halfVoxelSize, halfVoxelSize, 0.0), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerSouth + vec3(-halfVoxelSize, -halfVoxelSize, 0.0), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerSouth + vec3(halfVoxelSize, -halfVoxelSize, 0.0), 1.0);
        EmitVertex();
        EndPrimitive();
    }
    else
    {//north face
        vec3 centerNorth = wPos + vec3(0.0, 0.0, halfVoxelSize);
        gl_Position = projView * vec4(centerNorth + vec3(halfVoxelSize, halfVoxelSize, 0.0), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerNorth + vec3(-halfVoxelSize, halfVoxelSize, 0.0), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerNorth + vec3(halfVoxelSize, -halfVoxelSize, 0.0), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerNorth + vec3(-halfVoxelSize, -halfVoxelSize, 0.0), 1.0);
        EmitVertex();
        EndPrimitive();
    }

    if (camToVox.y > 0.0001F)
    {//bottom face
        vec3 centerBottom = wPos - vec3(0.0, halfVoxelSize, 0.0);
        gl_Position = projView * vec4(centerBottom + vec3(-halfVoxelSize, 0.0, -halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerBottom + vec3(halfVoxelSize, 0.0, -halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerBottom + vec3(-halfVoxelSize, 0.0, halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerBottom + vec3(halfVoxelSize, 0.0, halfVoxelSize), 1.0);
        EmitVertex();
        EndPrimitive();
    }
    else
    {//top face
        vec3 centerTop = wPos + vec3(0.0, halfVoxelSize, 0.0);
        gl_Position = projView * vec4(centerTop + vec3(-halfVoxelSize, 0.0, halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerTop + vec3(halfVoxelSize, 0.0, halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerTop + vec3(-halfVoxelSize, 0.0, -halfVoxelSize), 1.0);
        EmitVertex();
        gl_Position = projView * vec4(centerTop + vec3(halfVoxelSize, 0.0, -halfVoxelSize), 1.0);
        EmitVertex();
        EndPrimitive();
    }  
}


/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
layout(location = 0) out vec4 fragColor;
in float light;

void main()
{
    fragColor = vec4(light, light, light,1.0);
}