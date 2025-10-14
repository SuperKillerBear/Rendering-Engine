#version 460 core
#extension GL_ARB_bindless_texture : require

in vec3 vColor;
in vec2 vUV;

layout(bindless_sampler) uniform sampler2D uTexture;     // bindless texture handle (set with glUniformHandleui64ARB)
uniform vec3 uBaseColor;        // used if texture not applied


out vec4 FragColor;

void main()
{
    vec4 texColor = texture(uTexture, vUV);
    FragColor = texColor * vec4(uBaseColor, 1.0);
    
}
