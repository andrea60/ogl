#version 410

in vec2 texture_st;

uniform sampler2D first_tex;
uniform sampler2D second_tex;




out vec4 frag_colour;

void main()
{
	vec4 f_colour = texture2D(first_tex,texture_st).rgba;
	vec4 s_colour = texture2D(second_tex,texture_st).rgba;
	
	frag_colour = f_colour * s_colour;

}