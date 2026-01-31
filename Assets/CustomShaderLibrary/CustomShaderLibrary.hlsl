#ifndef CustomShaderLibrary_Included
#define CustomShaderLibrary_Included

float remap(float In, float2 InMinMax, float2 OutMinMax)
{
    return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

inline float noise_randomValue(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

float simpleNoise(float2 uv, float scale)
{
    float2 scaledUV = uv * scale;

    float2 i = floor(scaledUV);
    float2 f = frac(scaledUV);
    f = f * f * (3.0 - 2.0 * f);

    float a = noise_randomValue(i);
    float b = noise_randomValue(i + float2(1.0, 0.0));
    float c = noise_randomValue(i + float2(0.0, 1.0));
    float d = noise_randomValue(i + float2(1.0, 1.0));

    return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
}

float2 twirl(float2 uv, float2 center, float strength, float2 offset)
{
    float2 delta = uv - center;
    float angle = strength * length(delta) + offset.x;
    float x = cos(angle) * delta.x - sin(angle) * delta.y;
    float y = sin(angle) * delta.x + cos(angle) * delta.y;
    return float2(x + center.x, y + center.y);
}

float2 rotate_Radians(float2 uv, float2 center, float rotation)
{
    uv -= center;
                
    float s = sin(rotation);
    float c = cos(rotation);
                
    float2x2 rMatrix = float2x2(c, -s, s, c);
    uv = mul(rMatrix, uv);
                
    uv += center;
    return uv;
}

float modulo(float A, float B)
{
    return A - B * floor(A / B);
}

float2 modulo(float2 A, float2 B)
{
    return A - B * floor(A / B);
}

float2 GetPixelCoordinates(float4 screenPos)
{
    float2 normalizedPos = screenPos.xy / screenPos.w;
    return normalizedPos * _ScreenParams.xy;
}

float2 GetPixelCoordinatesSnapped(float4 screenPos)
{
    float2 normalizedPos = screenPos.xy / screenPos.w;
    return floor(normalizedPos * _ScreenParams.xy);
}

#endif

  