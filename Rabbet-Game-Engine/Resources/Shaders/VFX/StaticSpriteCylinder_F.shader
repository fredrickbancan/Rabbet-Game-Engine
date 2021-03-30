//shader for rendering point particles Dynamically opaque
#shader vertex
#version 330 core
layout(location = 0) in vec4 spritePos;
layout(location = 1) in vec4 spriteColor;
layout(location = 2) in vec3 spriteScale;
layout(location = 3) in vec4 spriteUVMinMax;
layout(location = 4) in float textureIndex;
layout(location = 5) in vec2 corner;//instanced quad corner

uniform float fogStart = 1000.0;
uniform float fogEnd = 1000.0;

out vec4 vColor;
out float visibility;
out vec2 fTexCoord;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform float percentageToNextTick;

void main()
{
   
    //create translation
    mat4 modelMatrix = mat4(1.0);
    modelMatrix[3][0] = spritePos.x;
    modelMatrix[3][1] = spritePos.y;
    modelMatrix[3][2] = spritePos.z;

        //remove horizontal rotation
    mat4 modelViewBillboard = viewMatrix * modelMatrix;
    modelViewBillboard[0][0] = 1;
    modelViewBillboard[0][1] = 0;
    modelViewBillboard[0][2] = 0;
    modelViewBillboard[2][0] = 0;
    modelViewBillboard[2][1] = 0;
    modelViewBillboard[2][2] = 1;

    vec4 cornerScaled = vec4(corner.x * spriteScale.x, corner.y * spriteScale.y, 0, 1);

    vec4 positionRelativeToCam = modelViewBillboard * cornerScaled;
    gl_Position = projectionMatrix * positionRelativeToCam;

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = (distanceFromCam - fogStart) / (fogEnd - fogStart);
    visibility = clamp(visibility, 0.0, 1.0);
    visibility = 1.0 - visibility;
    visibility *= visibility;

    vec2 uv = vec2(0.0,0.0);
    uv.x = corner.x < 0.0 ? spriteUVMinMax.x : spriteUVMinMax.z;
    uv.y = corner.y < 0.0 ? spriteUVMinMax.y : spriteUVMinMax.w;

    vColor = spriteColor;
    fTexCoord = uv;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
layout(location = 0) out vec4 fragColor;

in float visibility;
in vec4 vColor;
in vec2 fTexCoord;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 fogColor;
uniform sampler2D uTexture;
void main()
{
    vec4 textureColor = texture(uTexture, fTexCoord) * vColor;
    if (textureColor.a < 0.01F)
    {
        discard;//cutout
    }
    fragColor.rgb = mix(fogColor, textureColor.rgb, visibility);
    fragColor.a = 1;
}