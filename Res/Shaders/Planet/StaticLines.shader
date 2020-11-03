//base shader for rendering objects statically with fog
#shader vertex
#version 330 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 vertexColor;
layout(location = 2) in vec2 texCoord;
layout(location = 3) in float objectID;

uniform float fogDensity = 0.0075;
uniform float fogGradient = 2.5;
out vec4 vColor;

uniform float percentageToNextTick;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
	gl_Position = projectionMatrix * viewMatrix * position;
	vColor = vertexColor;
}

/*#############################################################################################################################################################################################*/
#shader fragment
#version 330 core
in vec2 vTexCoord;
in vec4 vColor;
out vec4 fragColor;

uniform int frame = 0;
uniform vec3 fogColor;


void main()
{
	fragColor.rgb = vColor.rgb;
	fragColor.a = 1;
}