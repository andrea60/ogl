#version 410

layout(location=0) in vec2 vertex_pos;
layout(location=1) in vec2 texture_uv;

uniform vec3 prova;

out vec2 texture_st;

void main()
{
	texture_st = texture_uv;

	gl_Position = vec4(vertex_pos,0.0,1.0);
}