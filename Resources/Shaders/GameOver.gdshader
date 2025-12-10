shader_type canvas_item;

const vec2 res = vec2(320, 180);

void fragment() {
	vec2 uv = UV;
	
	vec3 col;
	col.r = texture(TEXTURE, 0.5 + (uv - 0.5) * (1.0 + sin(TIME) * 0.08)).r;
	col.g = texture(TEXTURE, uv).r;
	col.b = texture(TEXTURE, 0.5 + (uv - 0.5) * (1.0 + cos(TIME) * 0.08)).r;
	
	COLOR = vec4(col, 1.0);
}