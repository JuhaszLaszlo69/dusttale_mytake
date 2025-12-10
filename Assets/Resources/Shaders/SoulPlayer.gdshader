shader_type canvas_item;

uniform bool immortally = true;
uniform vec4 modulate : source_color = vec4(1, 1, 1, 1);

void fragment() {
	vec4 col = texture(TEXTURE, UV);
	
	if(immortally)
		col.a *= pow(sin(TIME * 5.0), 2.0) * 0.7 + 0.3; 
	
	col.rgb = modulate.rgb;
	
	COLOR = col;
}