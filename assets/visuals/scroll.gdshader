shader_type spatial;

uniform sampler2D albedo_tex : source_color;
uniform float opacity : hint_range(0,1) = 1;

void fragment() {
	vec2 uv = UV;
    vec4 tex_color = texture(albedo_tex, (uv*30.0f)+TIME);
    ALBEDO = tex_color.rgb;
    ALPHA = clamp(tex_color.a - (1.0f-opacity),0,1);
}
