shader_type canvas_item;
render_mode unshaded;
uniform float offset = 0;
void vertex() { 
	VERTEX.x = (VERTEX.x - mod((TIME+offset) * 150.0, 2400.0) + 1100.0);
}
void light() {
	LIGHT *= 100.0f;
}