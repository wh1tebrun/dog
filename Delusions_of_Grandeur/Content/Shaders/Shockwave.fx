// Shockwave.fx

sampler2D textureSampler : register(s0);
float2 impactPoint : register(c0); // Impact point passed as constant
float rippleTime : register(c1);   // Ripple time passed as constant

// Radial ripple effect
float4 RadialRipple(float2 uv : TEXCOORD0) : COLOR
{
    // Calculate the distance from the impact point
    float2 distance = uv - impactPoint;
    float dist = length(distance);

    // Generate the ripple effect based on distance and time
    float ripple = sin(dist * 10.0 - rippleTime * 5.0) * 0.5 + 0.5;

    // Apply fading effect over time
    float fade = exp(-rippleTime * 0.5);

    // Color of the shockwave (orange)
    float4 color = float4(1.0, 0.5, 0.0, 1.0) * ripple * fade;
    return color;
}

// Define the technique with the pass using the compiled shader
technique ShockwaveTech
{
    pass P0
    {
        PixelShader = compile ps_3_0 RadialRipple();
    }
}
