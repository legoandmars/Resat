#include "ColorSpaces.cginc"

uint2 ArrayIndexFromOKHSL(float3 okhsl, uint2 arrayResolution)
{
    float hueIndex = round(clamp(okhsl.r, 0, 1) * arrayResolution.x - 1);
    float lightnessIndex = round(clamp(okhsl.b, 0, 1) * arrayResolution.y - 1);

    return hueIndex + (lightnessIndex * arrayResolution.y);
}

float3 OKHSLFromArrayIndex(float arrayIndex, uint2 arrayResolution)
{
    float hueIndex = arrayIndex % arrayResolution.x;
    float lightnessIndex = floor(arrayIndex / arrayResolution.x);

    float hue = (hueIndex + 1) / arrayResolution.x;
    float lightness = (lightnessIndex + 1) / arrayResolution.y;
    
    return float3(hue, 0.9f, lightness);
}