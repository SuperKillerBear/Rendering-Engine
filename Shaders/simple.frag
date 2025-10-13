#version 460
#extension GL_ARB_bindless_texture : require


layout(binding = 0) uniform sampler2D uTextureHandle; // optional default fallback
in vec2 outUV;
in vec3 outColor;


uniform u64 uTextureHandle; // bindless texture handle per object
uniform vec3 uBaseColor;    // fallback color

out vec4 FragColor;

void main()
{
    vec4 texColor = texture(uTextureHandle, outUV);

    // If texture exists, use it; otherwise fallback
    FragColor = texColor * vec4(uBaseColor, 1.0);
}
