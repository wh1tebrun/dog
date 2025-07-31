// Texture input
Texture2D inputTexture : register(t0);
SamplerState samplerState : register(s0);

// Constant buffer for shader parameters
cbuffer Constants : register(b0)
{
    float2 screenSize;      // Screen size (for scaling the coordinates)
    float2 impactPosition;  // Impact position (center of the hitbox)
    float2 hitboxSize;      // Hitbox size (width, height)
};

// Main pixel shader function
float4 MainPS(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
    // Calculate distance from the impact position (center of the hitbox)
    float dist = distance(textureCoordinates * screenSize, impactPosition * screenSize);

    // Flash intensity based on distance, using smoothstep for fade effect
    float flashIntensity = smoothstep(0.0, 0.1, 50.0 - dist); 

    // Flash color (white with intensity)
    float3 color = float3(1.0, 1.0, 1.0) * flashIntensity;

    return float4(color, 1.0); // Final color with full opacity
}

// Technique and pass for the shader
technique BulletImpactEffect
{
    pass P0
    {
        PixelShader = compile ps_3_0 MainPS();
        AlphaBlendEnable = TRUE;
        SrcBlend = SRCALPHA;
        DestBlend = INVSRCALPHA;
    }
}