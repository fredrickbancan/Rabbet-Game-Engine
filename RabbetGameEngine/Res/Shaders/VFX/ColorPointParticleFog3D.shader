#shader vertex
#version 330 
//location is the location of the value in the vertex atrib array
//for vec4 position, the gpu automatically fills in the 4th component with a 1.0F. This means you can treat position as a vec4 no problem. (no need for messy conversions)
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;

uniform float fogDensity = 0.0075;
const float fogGradient = 2.5;

out vec4 vColor;
out float visibility;//for fog

//matrix for projection transformations.
uniform mat4 projectionMatrix;
//matrix for camera transformations.
uniform mat4 viewMatrix;
//matrix for model transformations. All transformations in this matrix are relative to the model origin.
uniform mat4 modelMatrix;
//vector of viewport dimensions
uniform vec2 viewPortSize;

uniform float pointRadius = 0.1;
float positionResolution = 256.0;
void main()
{
    vec4 worldPosition = modelMatrix * position;
    vec4 positionRelativeToCam = viewMatrix * worldPosition;
    gl_Position = projectionMatrix * positionRelativeToCam;

    //keeps the point size consistent with distance AND resolution.
    gl_PointSize = viewPortSize.y * projectionMatrix[1][1] * pointRadius / gl_Position.w;//TODO: this does not take into account aspect ratio and can cause points to be elipsical in shape.

    //position jitter for retro feel
   // float aspectRatio = viewPortSize.X / viewPortSize.Y;
   // gl_Position.X = floor(gl_Position.X * (positionResolution / gl_Position.w)) / (positionResolution / gl_Position.w);
  //  gl_Position.Y = floor(gl_Position.Y * (positionResolution / (gl_Position.w * aspectRatio))) / (positionResolution / (gl_Position.w  * aspectRatio));

    float distanceFromCam = length(positionRelativeToCam.xyz);
    visibility = exp(-pow((distanceFromCam * fogDensity), fogGradient));
    visibility = clamp(visibility, 0.0, 1.0);

    vColor = vertexColor;
}


/*#############################################################################################################################################################################################*/
//Out variables from vertex shader are passed into the fragment shaders in variables, part of glsl language.
#shader fragment
#version 330

layout(location = 0) out vec4 color;
in float visibility;
in vec4 vColor;

uniform int renderPass = 0;
uniform vec3 fogColor;
uniform bool aoc = false;
uniform vec2 viewPortSize;//vector of viewport dimensions
uniform int frame = 0;
float ambientOcclusion;//variable for applying a shadowing effect towards the edges of the point to give the illusion of a sphereical shape

float rand3D(in vec3 xyz)
{
    return fract(tan(distance(xyz.xy * 1.6F, xyz.xy) * xyz.z) * xyz.x);
}


void makeCircle()
{
    //clamps fragments to circle shape. 
    vec2 centerVec = gl_PointCoord - vec2(0.5F);//get a vector from center of square to coord
    float coordLength = length(centerVec);

    if (coordLength >= 0.5F)
    {//discard if the vectors length is more than 0.5
        discard;
    }

    //calc ambient occlusion for circle
    if(aoc)
    ambientOcclusion = sqrt(1.0 - coordLength);
}


void main()
{
    makeCircle();
	if (vColor.a < 0.99)
	{
		if(rand3D(gl_FragCoord.xyz + (float(frame) * 0.0000001F)) > vColor.a)//do stochastic transparency
	    discard;
	}

    vec3 colorModified = vColor.rgb;
    if (aoc)
    {
        //add ambient occlusion shading
        colorModified.r *= ambientOcclusion;
        colorModified.g *= ambientOcclusion;
        colorModified.b *= ambientOcclusion;
    }

	//add fog effect to frag
    color.rgb = mix(fogColor, vColor.rgb, visibility);
}