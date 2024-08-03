Shader "Joseph&Acerola/Halftone" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader{

            CGINCLUDE

            #include "UnityCG.cginc"

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

                float4 _MainTex_TexelSize;

            ENDCG

                // RGB To CMYK
                Pass {
                    CGPROGRAM
                    #pragma vertex vp
                    #pragma fragment fp

                    sampler2D _MainTex;
                    float _CyanDotSize, _MagentaDotSize, _YellowDotSize, _BlackDotSize, _CyanBias, _MagentaBias, _YellowBias, _BlackBias;

                    float halftone(float2 uv, float v, float bias, float dotSize, float curve) {
                        float halftone = (sin(uv.x * _MainTex_TexelSize.z * dotSize) + sin(uv.y * _MainTex_TexelSize.w * dotSize)) / 2.0f;
                        float halftoneChange = fwidth(halftone) * 0.5f;

                        return halftone < pow(v + bias, curve);
                    }

                    float4 fp(v2f i) : SV_Target {
                        float3 col = saturate(tex2D(_MainTex, i.uv).rgb);
                        float r = col.r;
                        float g = col.g;
                        float b = col.b;
                        float k = min(1.0f - r, min(1.0f - g, 1.0f - b));
                        float3 cmy = 0.0f;
                        float invK = 1.0f - k;

                        if (invK != 0.0f) {
                            cmy.r = (1.0f - r - k) / invK;
                            cmy.g = (1.0f - g - k) / invK;
                            cmy.b = (1.0f - b - k) / invK;
                        }

                        // Cyan
                        float2x2 R = float2x2(cos(0.261799), -sin(0.261799), sin(0.261799), cos(0.261799));
                        cmy.r = halftone(mul(i.uv, R), cmy.r, _CyanBias, _CyanDotSize, 1.0f);

                        // Magenta
                        R = float2x2(cos(1.309), -sin(1.309), sin(1.309), cos(1.309));
                        cmy.g = halftone(mul(i.uv, R), cmy.g, _MagentaBias, _MagentaDotSize, 1.0f);

                        // Yellow
                        cmy.b = halftone(i.uv, cmy.b, _YellowBias, _YellowDotSize, 1.0f);

                        // Black
                        R = float2x2(cos(0.785398), -sin(0.785398), sin(0.785398), cos(0.785398));
                        k = halftone(mul(i.uv, R), k, _BlackBias, _BlackDotSize, 0.15f);

                        return float4(saturate(cmy), k);
                    }
                    ENDCG
                }

                Pass {
                    CGPROGRAM
                    #pragma vertex vp
                    #pragma fragment fp

                    Texture2D _MainTex;
                    SamplerState linear_mirror_sampler;

                    int _PrintCyan, _PrintMagenta, _PrintYellow, _PrintBlack;
                    float2 _CyanOffset, _MagentaOffset, _YellowOffset, _BlackOffset;

                    float4 fp(v2f i) : SV_Target {

                        float c = _MainTex.Sample(linear_mirror_sampler, i.uv + _CyanOffset * _MainTex_TexelSize.xy).r;
                        float m = _MainTex.Sample(linear_mirror_sampler, i.uv + _MagentaOffset * _MainTex_TexelSize.xy).g;
                        float y = _MainTex.Sample(linear_mirror_sampler, i.uv + _YellowOffset * _MainTex_TexelSize.xy).b;
                        float k = _MainTex.Sample(linear_mirror_sampler, i.uv + _BlackOffset * _MainTex_TexelSize.xy).a;

                        float3 output = 1.0f;
                        output.r -= c * _PrintCyan;
                        output.g -= m * _PrintMagenta;
                        output.b -= y * _PrintYellow;
                        return float4(saturate(output - k * _PrintBlack), 1.0f);
                    }
                    ENDCG
                }
    }
}