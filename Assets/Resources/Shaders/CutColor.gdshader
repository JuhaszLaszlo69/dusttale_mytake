
shader_type canvas_item;
const vec3 c = vec3(224.0 / 255.0, 160.0 / 255.0, 64.0 / 255.0);

void fragment() {
	vec4 col = texture(TEXTURE, UV);
	vec3 sort_col = vec3(0, 0, 0);
	
	sort_col.r = max(max(col.r, col.g), col.b);
	sort_col.b = min(min(col.r, col.g), col.b);
	
	if (col.r != sort_col.r && col.r != sort_col.b)
		sort_col.g = col.r;
	if (col.g != sort_col.r && col.g != sort_col.b)
		sort_col.g = col.g;
	if (col.b != sort_col.r && col.b != sort_col.b)
		sort_col.g = col.b;
	
	
	vec3 delta = abs(sort_col.rgb - c);
	if(max(max(delta.r, delta.g), delta.b) < 0.01)
		col.a = 0.0;
	
	COLOR = col;
}
