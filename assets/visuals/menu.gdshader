shader_type canvas_item;

uniform float size : hint_range(1.0, 100.0) = 25.0;
uniform vec4 color1 : source_color = vec4(1.0, 1.0, 1.0, 1.0); 
uniform vec4 color2 : source_color = vec4(0.7, 0.7, 0.7, 1.0);
uniform sampler2D img : source_color,repeat_enable;
uniform float speed : hint_range(0.0, 10.0) = 1.0;
uniform float direction_x : hint_range(-1.0, 1.0) = 1.0; // Horizontal movement
uniform float direction_y : hint_range(-1.0, 1.0) = 1.0; // Vertical movement

void fragment() {
    vec2 pos = (UV.xy * size)*vec2(16.0f/9.0f,1.0f);
    float time_offset = TIME * speed;

    // Use the slider values for direction and normalize them
    vec2 direction = normalize(vec2(direction_x, direction_y));
    vec2 movement = direction * time_offset;

    pos -= movement;
	vec2 postex = pos;
	pos = floor(pos);

    float pattern_mask = mod(pos.x + mod(pos.y, 2.0), 2.0)+sin(TIME)*0.67f;
	vec4 tex = texture(img,postex);
	if (int(pos.x)%44 != 0) tex = vec4(0.0f,0.0f,0.0f,0.0f);
	if (int(pos.y)%62 != 0) tex = vec4(0.0f,0.0f,0.0f,0.0f);
	vec4 col = mix(color1, color2, pattern_mask);
    COLOR = mix(col,tex*col,tex.a/3.14f);
}
