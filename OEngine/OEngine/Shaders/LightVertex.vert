#version 410

layout(location=0) in vec3 v_position;
layout(location=1) in vec3 ignored;

uniform mat4 t_mat,s_mat,view;
uniform vec3 light_center;
uniform vec2 stretch_scale;

out vec2 light_pos;
out vec3 vp;
out float radius;


void main()
{
	mat4 model = t_mat * s_mat;
	vec3 position_eye = vec3(view * model * vec4(v_position,1.0));

	light_pos = (view * model * vec4(light_center,1.0)).xy;
	vp = position_eye;
	radius = abs((s_mat * vec4(v_position,1.0)).x);
	gl_Position = vec4(position_eye * vec3(stretch_scale,1),1.0);
}