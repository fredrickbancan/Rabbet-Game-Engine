//shader for rendering point particles Dynamically opaque
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float textureIndex;
layout(location = 4) in vec3 textPos;

out vec4 vColor;
out float vTextureIndex;
out vec2 fTexCoord;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform float percentageToNextTick;

void main()
{
   
    //create translation
    mat4 modelMatrix = mat4(1.0F);
    modelMatrix[3][0] = textPos.x;
    modelMatrix[3][1] = textPos.y;
    modelMatrix[3][2] = textPos.z;

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

    gl_Position = projectionMatrix * modelViewBillboard * position;

    vColor = vertexColor;
    fTexCoord = texCoord;
    vTextureIndex = textureIndex;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core

layout(location = 0) out vec4 fragColor;

in float vTextureIndex;
in vec4 vColor;
in vec2 fTexCoord;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform sampler2D uTextures[8];

void main()
{
    int i = int(ceil(vTextureIndex));
    vec4 textureColor = texture(uTextures[i], fTexCoord) * vColor;
    if (textureColor.a < 0.01F)
    {
        discard;//cutout
    }
    fragColor.a = 1;
}