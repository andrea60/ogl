#version 410

in vec2 v_position;
in vec2 vt;

out vec2 texture_coordinates;


uniform vec2 stretch_scale;


void main(){
	vec3 position_eye = vec3(v_position,1.0);
	texture_coordinates = vt;
	gl_Position = vec4(position_eye * vec3(stretch_scale,1),1.0);
}