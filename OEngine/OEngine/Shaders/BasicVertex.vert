#version 410

layout(location = 0) in vec3 v_position;
layout(location = 1) in vec2 vt;

out vec2 texture_coordinates;
uniform mat4 view,model;

uniform vec2 stretch_scale;


void main(){
	vec3 position_eye = vec3(view * model * vec4(v_position,1.0));
	texture_coordinates = vt;
	gl_Position = vec4(position_eye * vec3(stretch_scale,1),1.0);
}