Shader "Joseph&Acerola/ASCII" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader{
            CGINCLUDE

            #include "UnityCG.cginc"
            #define PI 3.14159265358979323846f

            Texture2D _MainTex, _AsciiTex, _LuminanceTex;
            float4 _MainTex_TexelSize;
            SamplerState point_clamp_sampler, linear_clamp_sampler;

            float _Sigma, _K, _Tau, _Threshold;
            int _GaussianKernelSize, _Invert;

            struct VertexData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vp(VertexData v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float luminance(float3 color) {
                return dot(color, float3(0.299f, 0.587f, 0.114f));
            }

            float gaussian(float sigma, float pos) {
                return (1.0f / sqrt(2.0f * PI * sigma * sigma)) * exp(-(pos * pos) / (2.0f * sigma * sigma));
            }

            ENDCG

            Pass { // Point Sampler
                CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                float4 fp(v2f i) : SV_Target {
                    float4 col = _MainTex.Sample(point_clamp_sampler, i.uv);

                    return col;
                }
                ENDCG
            }

            Pass { // Luminance Pass
                CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                float fp(v2f i) : SV_Target {
                    float4 col = saturate(_MainTex.Sample(point_clamp_sampler, i.uv));
                    float lum = luminance(col.rgb);

                    return lum;
                }
                ENDCG
            }

            Pass { // Pack luminance
                CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                float4 fp(v2f i) : SV_Target {
                    float3 col = saturate(_MainTex.Sample(point_clamp_sampler, i.uv));
                    float lum = _LuminanceTex.Sample(point_clamp_sampler, i.uv);

                    return float4(col, lum);
                }
                ENDCG
            }

            Pass { // Horizontal Blur
                CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                float4 fp(v2f i) : SV_Target {
                    float2 blur = 0;
                    float2 kernelSum = 0;

                    for (int x = -_GaussianKernelSize; x <= _GaussianKernelSize; ++x) {
                        float2 luminance = _MainTex.Sample(point_clamp_sampler, i.uv + float2(x, 0) * _MainTex_TexelSize.xy).r;
                        float2 gauss = float2(gaussian(_Sigma, x), gaussian(_Sigma * _K, x));

                        blur += luminance * gauss;
                        kernelSum += gauss;
                    }

                    return float4(blur / kernelSum, 0, 0);
                }
                ENDCG
            }

            Pass { // Vertical Blur and Difference
                CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                float fp(v2f i) : SV_Target {
                    float2 blur = 0;
                    float2 kernelSum = 0;

                    for (int y = -_GaussianKernelSize; y <= _GaussianKernelSize; ++y) {
                        float2 luminance = _MainTex.Sample(point_clamp_sampler, i.uv + float2(0, y) * _MainTex_TexelSize.xy).rg;
                        float2 gauss = float2(gaussian(_Sigma, y), gaussian(_Sigma * _K, y));

                        blur += luminance * gauss;
                        kernelSum += gauss;
                    }

                    blur = blur / kernelSum;

                    float4 D = (blur.x - _Tau * blur.y);

                    D = (D >= _Threshold) ? 1 : 0;

                    if (_Invert) D = 1 - D;

                    return D;
                }
                ENDCG
            }

            Pass { // Sobel Filter Horizontal Pass
                CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                float4 fp(v2f i) : SV_Target {
                    float lum1 = _MainTex.Sample(point_clamp_sampler, i.uv - float2(1, 0) * _MainTex_TexelSize.xy);
                    float lum2 = _MainTex.Sample(point_clamp_sampler, i.uv);
                    float lum3 = _MainTex.Sample(point_clamp_sampler, i.uv + float2(1, 0) * _MainTex_TexelSize.xy);

                    float Gx = 3 * lum1 + 0 * lum2 + -3 * lum3;
                    float Gy = 3 + lum1 + 10 * lum2 + 3 * lum3;

                    return float4(Gx, Gy, 0, 0);
                }
                ENDCG
            }

            Pass { // Sobel Filter Vertical Pass
                CGPROGRAM
                #pragma vertex vp
                #pragma fragment fp

                float4 fp(v2f i) : SV_Target {
                    float2 grad1 = _MainTex.Sample(point_clamp_sampler, i.uv - float2(0, 1) * _MainTex_TexelSize.xy).xy;
                    float2 grad2 = _MainTex.Sample(point_clamp_sampler, i.uv).xy;
                    float2 grad3 = _MainTex.Sample(point_clamp_sampler, i.uv + float2(0, 1) * _MainTex_TexelSize.xy).xy;

                    float Gx = 3 * grad1.x + 10 * grad2.x + 3 * grad3.x;
                    float Gy = 3 * grad1.y + 0 * grad2.y + -3 * grad3.y;

                    float2 G = float2(Gx, Gy);
                    G = normalize(G);

                    float magnitude = length(float2(Gx, Gy));
                    float theta = atan2(G.y, G.x);

                    // if ((-3.0f * PI / 5.0f) < theta && theta < (-2.0 * PI / 5)) theta = 1;
                    // else theta = 0;
                    return float4(max(0.0f, magnitude), theta, 1 - isnan(theta), 0);
                }
                ENDCG
            }
    }
}