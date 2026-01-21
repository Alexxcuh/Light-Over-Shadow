shader_type spatial;
render_mode unshaded;

varying vec3 v_world_pos;

float grid(float scale, float thickness) {
    float gx = step(fract(v_world_pos.x / scale), thickness);
    float gz = step(fract(v_world_pos.z / scale), thickness);
    return max(gx, gz);
}

void vertex() {
    v_world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
    float dist = distance(CAMERA_POSITION_WORLD, v_world_pos)*0.02f;
    float thickness = 0.02;
    float level = floor(log(dist + 1.0) / log(8.0));
    float s0 = pow(8.0, level);
    float s1 = s0 * 8.0;
	float fade = clamp((1.0 - dist) / (s1*0.1), 0.0, 1.0);
    float g0 = grid(s0, thickness);
    float g1 = grid(s1, thickness*0.5f);

    float grid_combined = max(g0, g1);

    ALBEDO = vec3(0.05+((grid_combined-g0)*0.1f));
	ALPHA  = clamp(grid_combined,0.0,1.0) * fade;
	if (v_world_pos.x < (thickness+g1)/12.0 && v_world_pos.x >= .00) {
		dist /= 1.5f;
		fade = clamp((1.0 - dist) / (s1*0.1), 0.0, 1.0);
		ALBEDO = vec3(0.2,0.45,1.0);
		ALPHA  = clamp(grid_combined,0.0,1.0) * fade;
	}
	if (v_world_pos.z < (thickness+g1)/12.0 && v_world_pos.z >= 0.0) {
		dist /= 1.5f;
		fade = clamp((1.0 - dist) / (s1*0.05f), 0.0, 1.0);
		ALBEDO = vec3(1.0,0.3,0.3);
		ALPHA  = clamp(grid_combined,0.0,1.0) * fade;
	}
}
