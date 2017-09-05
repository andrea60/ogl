#version 410


uniform sampler2D active_texture;

in vec2 texture_coordinates;


layout(location = 0) out vec4 frag_colour;

void main(){
	

	vec4 texel = texture(active_texture,texture_coordinates);
	
	
	
	frag_colour = texel;
}