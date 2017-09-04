#version 410

in vec3 vp;
in vec2 light_pos;
in float radius;

uniform float exp_base;
uniform float exp_k;
uniform float light_intensity;

uniform vec3 light_color;

out vec4 frag_colour;

void main()
{
	float d = distance(light_pos,vp.xy);	
	float diffuse = 0.0;
	//float k = 2 / 0.11394 * radius;
	
	float k = 6.9078 / radius;
	
	if (d <= radius)
		diffuse = pow(2.81, -k* d) * light_intensity;

	if (diffuse > 0.8)
		diffuse = 0.8;
	
	frag_colour = vec4(light_color,diffuse); //Da sostituire
	
	

}