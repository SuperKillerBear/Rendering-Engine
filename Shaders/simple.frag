#version 460 core
#extension GL_ARB_bindless_texture : require

in vec3 vWorldNormal;
in vec2 vUV;

layout(bindless_sampler) uniform sampler2D uTexture;     // bindless texture handle (set with glUniformHandleui64ARB)
uniform vec3 uBaseColor;        // used if texture not applied

uniform vec3 uLightDir;		// normalize this on CPU
uniform vec3 uLightColor;	// e.g. vec3(1)
uniform vec3 uAmbientColor;	// e.g. vec3(0.2)

out vec4 FragColor;

void main()
{
    vec3 albedo = texture(uTexture, vUV).rgb * uBaseColor;

    float ndl = max(dot(normalize(vWorldNormal), normalize(uLightDir)), 0.0);

    // PS2 Banding Effect:
    //float steps = 4.0;
    //ndl = floor(ndl * steps) / steps;

    vec3 lit = albedo * (uAmbientColor + uLightColor * ndl);

    FragColor = vec4(lit, 1.0);
    
}
