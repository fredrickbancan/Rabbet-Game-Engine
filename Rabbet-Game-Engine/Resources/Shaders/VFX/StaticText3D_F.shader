//shader for rendering point particles Dynamically opaque
#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float textureIndex;
layout(location = 4) in vec3 textPos;

uniform float fogStart = 1000.0;
uniform float fogEnd = 1000.0;

out vec4 vColor;
out float visibility;
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
    vec4 positionRelativeToCam = modelViewBillboard * position;
    gl_Position = projectionMatrix * positionRelativeToCam;

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = (distanceFromCam - fogStart) / (fogEnd - fogStart);
    visibility = clamp(visibility, 0.0, 1.0);
    visibility = 1.0 - visibility;
    visibility *= visibility;

    vColor = vertexColor;
    fTexCoord = texCoord;
    vTextureIndex = textureIndex;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
layout(location = 0) out vec4 fragColor;

in float visibility;
in float vTextureIndex;
in vec4 vColor;
in vec2 fTexCoord;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 fogColor;
uniform sampler2D uTextures[8];
void main()
{
    int i = int(ceil(vTextureIndex));
    vec4 textureColor = texture(uTextures[i], fTexCoord) * vColor;
    if (textureColor.a < 0.01F)
    {
        discard;//cutout
    }
    fragColor.rgb = mix(fogColor, textureColor.rgb, visibility);
    fragColor.a = 1;
}