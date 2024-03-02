// Color boilerplate for converting between rgb and OKLAB/OKHSL/OKLCH in-shader
// This could almost certainly be massively optimized, but as a basic implementation it should be Good Enough

// Uses a lot of code from the following
// https://github.com/GarrettGunnell/AcerolaFX/blob/main/Shaders/AcerolaFX_ColorSpaces.fx
// https://github.com/bottosson/bottosson.github.io/blob/master/misc/colorpicker/colorconversion.js

// modify to use hue 0-1 so i don't have to think as much
static const float PI = 3.1415926535897932384626433832795;
static const float hueMultiplier = (PI * 2) / 1;

// OKLAB matrices
static const float3x3 lrgb2cone = float3x3(
    0.412165612, 0.211859107, 0.0883097947,
    0.536275208, 0.6807189584, 0.2818474174,
    0.0514575653, 0.107406579, 0.6302613616);

static const float3x3 cone2lab = float3x3(
    +0.2104542553, +1.9779984951, +0.0259040371,
    +0.7936177850, -2.4285922050, +0.7827717662,
    +0.0040720468, +0.4505937099, -0.8086757660);

static const float3x3 lab2cone = float3x3(
    +4.0767416621, -1.2684380046, -0.0041960863,
    -3.3077115913, +2.6097574011, -0.7034186147,
    +0.2309699292, -0.3413193965, +1.7076147010);

static const float3x3 cone2lrgb = float3x3(
    1, 1, 1,
    +0.3963377774f, -0.1055613458f, -0.0894841775f,
    +0.2158037573f, -0.0638541728f, -1.2914855480f);          

float3 linear_srgb_to_oklab(float3 rgb) 
{
    float l = 0.4122214708 * rgb.r + 0.5363325363 * rgb.g + 0.0514459929 * rgb.b;
    float m = 0.2119034982 * rgb.r + 0.6806995451 * rgb.g + 0.1073969566 * rgb.b;
    float s = 0.0883024619 * rgb.r + 0.2817188376 * rgb.g + 0.6299787005 * rgb.b;

    float l_ = pow(l, 1.0 / 3.0);
    float m_ = pow(m, 1.0 / 3.0);
    float s_ = pow(s, 1.0 / 3.0);

    return float3(
        0.2104542553*l_ + 0.7936177850*m_ - 0.0040720468*s_,
        1.9779984951*l_ - 2.4285922050*m_ + 0.4505937099*s_,
        0.0259040371*l_ + 0.7827717662*m_ - 0.8086757660*s_
    );
}

float3 oklab_to_linear_srgb(float3 lab) 
{
    float l_ = lab.r + 0.3963377774 * lab.g + 0.2158037573 * lab.b;
    float m_ = lab.r - 0.1055613458 * lab.g - 0.0638541728 * lab.b;
    float s_ = lab.r - 0.0894841775 * lab.g - 1.2914855480 * lab.b;

    float l = l_*l_*l_;
    float m = m_*m_*m_;
    float s = s_*s_*s_;

    return float3(
        (+4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s),
        (-1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s),
        (-0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s)
    );
}

float3 RGBtoOKLAB(float3 col) {    
    col = mul(col, lrgb2cone);
    col = pow(col, 1.0 / 3.0);
    col = mul(col, cone2lab);
    return col;
}
            
float3 RGBtoOKLCH(float3 col) {
    col = RGBtoOKLAB(col);

    float3 lch = 0.0f;
    lch.r = col.x;
    lch.g = sqrt(col.g * col.g + col.b * col.b);
    lch.b = atan2(col.b, col.g) * hueMultiplier;

    return lch;
}

float3 OKLABtoRGB(float3 col) {
    col = mul(col, cone2lrgb);
    col = col * col * col;
    col = mul(col, lab2cone);
    return col;
}

float ComputeMaxSaturation(float a, float b)
{
    float k0, k1, k2, k3, k4, wl, wm, ws;

    if (-1.88170328 * a - 0.80936493 * b > 1)
    {
        k0 = +1.19086277; k1 = +1.76576728; k2 = +0.59662641; k3 = +0.75515197; k4 = +0.56771245;
        wl = +4.0767416621; wm = -3.3077115913; ws = +0.2309699292;
    }
    else if (1.81444104 * a - 1.19445276 * b > 1)
    {
        k0 = +0.73956515; k1 = -0.45954404; k2 = +0.08285427; k3 = +0.12541070; k4 = +0.14503204;
        wl = -1.2684380046; wm = +2.6097574011; ws = -0.3413193965;
    }
    else
    {
        k0 = +1.35733652; k1 = -0.00915799; k2 = -1.15130210; k3 = -0.50559606; k4 = +0.00692167;
        wl = -0.0041960863; wm = -0.7034186147; ws = +1.7076147010;
    }

    float S = k0 + k1 * a + k2 * b + k3 * a * a + k4 * a * b;

    float k_l = +0.3963377774 * a + 0.2158037573 * b;
    float k_m = -0.1055613458 * a - 0.0638541728 * b;
    float k_s = -0.0894841775 * a - 1.2914855480 * b;

    float l_ = 1 + S * k_l;
    float m_ = 1 + S * k_m;
    float s_ = 1 + S * k_s;

    float l = l_ * l_ * l_;
    float m = m_ * m_ * m_;
    float s = s_ * s_ * s_;

    float l_dS = 3 * k_l * l_ * l_;
    float m_dS = 3 * k_m * m_ * m_;
    float s_dS = 3 * k_s * s_ * s_;

    float l_dS2 = 6 * k_l * k_l * l_;
    float m_dS2 = 6 * k_m * k_m * m_;
    float s_dS2 = 6 * k_s * k_s * s_;

    float f  = wl * l     + wm * m     + ws * s;
    float f1 = wl * l_dS  + wm * m_dS  + ws * s_dS;
    float f2 = wl * l_dS2 + wm * m_dS2 + ws * s_dS2;

    S = S - f * f1 / (f1*f1 - 0.5 * f * f2);

    return S;
}

float2 FindCusp(float a, float b)
{
    // First, find the maximum saturation (saturation S = C/L)
    float S_cusp = ComputeMaxSaturation(a, b);

    // Convert to linear sRGB to find the first point where at least one of r,g or b >= 1:
    float3 rgb_at_max = oklab_to_linear_srgb(float3(1, S_cusp * a, S_cusp * b));
    float L_cusp = pow(1 / max(max(rgb_at_max.r, rgb_at_max.g), rgb_at_max.b), 1 / 3);
    float C_cusp = L_cusp * S_cusp;

    return float2(L_cusp , C_cusp);
}

float FindGamutIntersection(float a, float b, float L1, float C1, float L0, float2 cusp)
{
    // truncated to remove some gamut stuff
    // this comes at the cost of accuracy, but for realtime rendering it should HOPEFULLY be close
    // reimpl if this causes weird color problems
    return cusp.y * L0 / (C1 * cusp.x + cusp.y * (L0 - L1));
}

float2 GetSTMax(float2 cusp)
{
    float L = min(max(cusp.x, .0001), 0.9999);
    float C = max(cusp.y, .0001); 
    return float2(C/L, C/(1-L));
}

float Toe(float x)
{
    float k_1 = 0.206;
    float k_2 = 0.03;
    float k_3 = (1+k_1)/(1+k_2);
    
    return 0.5*(k_3*x - k_1 + sqrt((k_3*x - k_1)*(k_3*x - k_1) + 4*k_2*k_3*x));
}

float ToeInv(float x)
{
    float k_1 = 0.206;
    float k_2 = 0.03;
    float k_3 = (1+k_1)/(1+k_2);
    return (x*x + k_1*x)/(k_3*(x+k_2));
}

// https://github.com/bottosson/bottosson.github.io/blob/master/misc/colorpicker/colorconversion.js
float3 GetCs(float L, float a_, float b_)
{
    float2 cusp = FindCusp(a_, b_);

    float C_max = FindGamutIntersection(a_,b_,L,1,L,cusp);
    float2 ST_max = GetSTMax(cusp);

    float S_mid = 0.11516993 + 1/(
        + 7.44778970 + 4.15901240*b_
        + a_*(- 2.19557347 + 1.75198401*b_
        + a_*(- 2.13704948 -10.02301043*b_ 
        + a_*(- 4.24894561 + 5.38770819*b_ + 4.69891013*a_
        )))
    );

    float T_mid = 0.11239642 + 1/(
        + 1.61320320 - 0.68124379*b_
        + a_*(+ 0.40370612 + 0.90148123*b_
        + a_*(- 0.27087943 + 0.61223990*b_ 
        + a_*(+ 0.00299215 - 0.45399568*b_ - 0.14661872*a_
        )))
    );

    float k = C_max/min((L*ST_max.r), (1-L)*ST_max.g);

    float C_a = L*S_mid;
    float C_b = (1-L)*T_mid;
    float C_mid = 0.9*k*sqrt(sqrt(1/(1/(C_a*C_a*C_a*C_a) + 1/(C_b*C_b*C_b*C_b))));

    float C_a2 = L*0.4;
    float C_b2 = (1-L)*0.8;
    float C_0 = sqrt(1/(1/(C_a2*C_a2) + 1/(C_b2*C_b2)));

    return float3(C_0, C_mid, C_max);
}

float3 RGBtoOKHSL(float3 col)
{
    float3 lab = linear_srgb_to_oklab(col);

    float C = sqrt(lab.g * lab.g + lab.b * lab.b);
    float a_ = lab.g/C;
    float b_ = lab.b/C;

    float L = lab.r;
    float h = 0.5 + 0.5*atan2(-lab.b, -lab.g)/PI;

    float3 Cs = GetCs(L, a_, b_);
    float C_0 = Cs.r;
    float C_mid = Cs.g;
    float C_max = Cs.b;

    float s;
    if (C < C_mid)
    {   
        float k_0 = 0;
        float k_1 = 0.8*C_0;
        float k_2 = (1-k_1/C_mid);

        float t = (C - k_0)/(k_1 + k_2*(C - k_0));
        s = t*0.8;
    }
    else
    {
        float k_0 = C_mid;
        float k_1 = 0.2*C_mid*C_mid*1.25*1.25/C_0;
        float k_2 = (1 - (k_1)/(C_max - C_mid));

        float t = (C - k_0)/(k_1 + k_2*(C - k_0));
        s = 0.8 + 0.2*t;
    }

    float l = Toe(L);
    return float3(h,s,l);
}

float3 OKHSLtoRGB(float3 hsl)
{
    float h = hsl.r;
    float s = hsl.g;
    float l = hsl.b;

    if (l == 1)
    {
        return float3(255,255,255);
    }
    else if (l == 0)
    {
        return float3(0,0,0);
    }
    
    float a_ = cos(2*PI*h);
    float b_ = sin(2*PI*h);   
    float L = ToeInv(l);

    float3 Cs = GetCs(L, a_, b_);
    float C_0 = Cs.r;
    float C_mid = Cs.g;
    float C_max = Cs.b;

    float t, k_0, k_1, k_2;
    if (s < 0.8)
    {   
        t = 1.25*s;
        k_0 = 0;
        k_1 = 0.8*C_0;
        k_2 = (1-k_1/C_mid);
    }
    else
    {
        t = 5*(s-0.8);
        k_0 = C_mid;
        k_1 = 0.2*C_mid*C_mid*1.25*1.25/C_0;
        k_2 = (1 - (k_1)/(C_max - C_mid));
    }

    float C = k_0 + t*k_1/(1-k_2*t);

    // If we would only use one of the Cs:
    //C = s*C_0;
    //C = s*1.25*C_mid;
    //C = s*C_max;

    float3 rgb = OKLABtoRGB(float3(L, C*a_, C*b_));
    return rgb;
}

float3 OKLCHtoRGB(float3 col)
{
    float3 oklab;
    oklab.r = col.r;
    oklab.g = col.g * cos(col.b * hueMultiplier);
    oklab.b = col.g * sin(col.b * hueMultiplier);

    return OKLABtoRGB(oklab);
}