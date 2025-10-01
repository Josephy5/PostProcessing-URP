//The Anisotropic Kuwahara effect code is based on Acerola's Anisotropic Kuawahara Effect, 
//which is interpreted from the first edition of the GPU Pro book under the article, Anisotropic Kuwahara Filtering with Polynomial Weighting Functions 
Shader "Hidden/AnisotropicKuwahara" 
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader 
    {
        CGINCLUDE

        #include "UnityCG.cginc"

        struct VertexData {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f {
            float2 uv : TEXCOORD0;
            float4 position : SV_POSITION;
        };

        v2f vp(VertexData v) {
            v2f o;
            o.position = UnityObjectToClipPos(v.position);
            o.uv = v.uv;
            return o;
        }

        #define PI 3.14159265358979323846f
        
        //parameters for main texture, and data relating to main texture needed for the effect
        sampler2D _MainTex, _TemporaryTexture;
        float4 _MainTex_TexelSize;

        //parameters for the pixel kernel used to process the effect
        //Note: the number of sectors in kernel must be 8 as this kuwahara effect splits the kernel into 8 sectors to process it
        // [A kernel is the area containing the selected pixel on screen and its neighbors up to a specific amount (like up to 2 pixels away from the selected/center pixel)]
        int _KernelSize, _NumOfSectorsInKernel;

        //parameters for finetuning the effect
        float _Hardness, _Sharpness, _AnisotropyStrength, _ZeroCrossing, _Zeta;

        //the gaussian weight method for this effect
        float gaussian(float sigma, float pos) {
            return (1.0f / sqrt(2.0f * PI * sigma * sigma)) * exp(-(pos * pos) / (2.0f * sigma * sigma));
        }

        ENDCG

        // Calculate Eigenvectors
        Pass {
            CGPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            float4 fp(v2f i) : SV_Target {
                float2 texelSize = _MainTex_TexelSize.xy;

                //Sobel Operators for horizontal and vertical gradients (to detect vertical or horizontal edges),
                //We calulate each of the conditions the pixel's direction could take for the effect (We pre-compute them to save on doing extra calculations)
                float2 negDiag = i.uv + float2(-texelSize.x, -texelSize.y);
                float2 negX = i.uv + float2(-texelSize.x, 0.0);
                float2 negY = i.uv + float2(0.0, -texelSize.y);
                float2 posX = i.uv + float2(texelSize.x, 0.0);
                float2 posY = i.uv + float2(0.0, texelSize.y);
                float2 posDiag = i.uv + float2(texelSize.x, texelSize.y);
                float2 negXposY = i.uv + float2(-texelSize.x, texelSize.y);
                float2 posXnegY = i.uv + float2(texelSize.x, -texelSize.y);
                
                //Sample colors using the pre-computed values
                float3 colorNegDiag = tex2D(_MainTex, negDiag).rgb;
                float3 colorNegX = tex2D(_MainTex, negX).rgb;
                float3 colorNegY = tex2D(_MainTex, negY).rgb;
                float3 colorPosX = tex2D(_MainTex, posX).rgb;
                float3 colorPosY = tex2D(_MainTex, posY).rgb;
                float3 colorPosDiag = tex2D(_MainTex, posDiag).rgb;
                float3 colorNegXPosY = tex2D(_MainTex, negXposY).rgb;
                float3 colorPosXNegY = tex2D(_MainTex, posXnegY).rgb;    
                
                // Calculate x gradient using our Sobel operator values
                float3 gradientX = (
                    1.0 * colorNegDiag +
                    2.0 * colorNegX +
                    1.0 * colorNegXPosY +
                    -1.0 * colorPosXNegY +
                    -2.0 * colorPosX +
                    -1.0 * colorPosDiag
                ) / 4.0;

                // Calculate y gradient using our Sobel operator values
                float3 gradientY = (
                    1.0 * colorNegDiag +
                    2.0 * colorNegY +
                    1.0 * colorPosXNegY +
                    -1.0 * colorNegXPosY +
                    -2.0 * colorPosY +
                    -1.0 * colorPosDiag
                ) / 4.0;

                // Calculate the structure tensor to return back as eigenvector
                float gxx = dot(gradientX, gradientX); // Gradient magnitude in X direction
                float gyy = dot(gradientY, gradientY); // Gradient magnitude in Y direction
                float gxy = dot(gradientX, gradientY); // Cross-correlation of gradients
                
                return float4(gxx, gyy, gxy, 1.0f);
            }
            ENDCG
        }

        // Blur Pass 1, Horizontal Gaussian Blur Pass
        Pass {
            CGPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            float4 fp(v2f i) : SV_Target {
                int kernelRadius = 5;
                float gaussianSigma = 2.0f, weightSum = 0.0f;

                float4 col = 0;
                
                for (int offsetX = -kernelRadius; offsetX <= kernelRadius; ++offsetX) {
                    float2 samplePos = i.uv + float2(offsetX, 0) * _MainTex_TexelSize.xy;
                    float4 sampledColor = tex2D(_MainTex, samplePos);

                    float gaussianWeight = gaussian(gaussianSigma, offsetX);

                    col += sampledColor * gaussianWeight;
                    weightSum += gaussianWeight;
                }

                return col / weightSum;
            }
            ENDCG
        }

        // Blur Pass 2, Vertical Gaussian Blur & calculations for anisotropy
        Pass {
            CGPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            float4 fp(v2f i) : SV_Target {
                int kernelRadius = 5;
                float gaussianSigma = 2.0f, weightSum = 0.0f;

                float4 col = 0;
                
                for (int y = -kernelRadius; y <= kernelRadius; ++y) {
                    float2 samplePos = i.uv + float2(0, y) * _MainTex_TexelSize.xy;
                    float4 sampleColor = tex2D(_MainTex, samplePos);
                    float gaussianWeight = gaussian(gaussianSigma, y);

                    col += sampleColor * gaussianWeight;
                    weightSum += gaussianWeight;
                }

                //g = Structure tensor's components, will be using Acerola's formula & code to calcluate the eigenvalues for this method as this is much shorter, simplifying this part to 
                //be readable will make it a bit long for me personally, for this part, refer to Acerola's video for a good simple explanation to this at ~11:52
                
                float3 g = col.rgb / weightSum;
                
                //get the lambda values/eigenvalues for eigenvector
                float lambda1 = 0.5f * (g.y + g.x + sqrt(g.y * g.y - 2.0f * g.x * g.y + g.x * g.x + 4.0f * g.z * g.z));
                float lambda2 = 0.5f * (g.y + g.x - sqrt(g.y * g.y - 2.0f * g.x * g.y + g.x * g.x + 4.0f * g.z * g.z));

                float2 eigenvector = float2(lambda1 - g.x, -g.z);
                //normalize eigenvector, in case when the length is 0 (modified to add a slight offset to prevent division by zero, if you want to, you can replace it with the one with the zero)
                float2 normalizedDirection = length(eigenvector) > 0.0001f ? normalize(eigenvector) : float2(0.0f, 1.0f);
                //float2 normalizedDirection = length(eigenvector) > 0.0 ? normalize(eigenvector) : float2(0.0f, 1.0f);

                //Calculate orientation angle in radians
                float orientationAngle = -atan2(normalizedDirection.y, normalizedDirection.x);

                //Anisotropy Calculation, modified to add a slight offset to prevent division by zero, if you want to, you can replace it with the one with the zero
                float anisotropy = (lambda1 + lambda2 > 0.0001f) ? (lambda1 - lambda2) / (lambda1 + lambda2) : 0.0f;
                //float anisotropy = (lambda1 + lambda2 > 0.0f) ? (lambda1 - lambda2) / (lambda1 + lambda2) : 0.0f;
                
                return float4(normalizedDirection, orientationAngle, anisotropy);
            }
            ENDCG
        }

        // Apply Kuwahara Filter
        Pass {
            CGPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            float4 fp(v2f i) : SV_Target {
                float anisotropyStrength = _AnisotropyStrength;

                float4 tempTex = tex2D(_TemporaryTexture, i.uv);

                //Bit shift optimization, if not, you can go back to using the /2 one
                int kernelRadius = _KernelSize >> 1;
                //int kernelRadius = _KernelSize / 2;

                //a and b determine the ellipse semi-axes lengths, Larger anisotropy (coherence) leads to more elongated filter shape
                float a = float((kernelRadius)) * clamp((anisotropyStrength + tempTex.w) / anisotropyStrength, 0.1f, 2.0f);
                float b = float((kernelRadius)) * clamp(anisotropyStrength / (anisotropyStrength + tempTex.w), 0.1f, 2.0f);
                
                float orientationAngle = tempTex.z;
                float cosTheta = cos(orientationAngle);
                float sinTheta = sin(orientationAngle);

                float2x2 rotationMatrix = {
                    cosTheta, -sinTheta,
                    sinTheta, cosTheta
                };

                float2x2 scalingMatrix = {
                    0.5f / a, 0.0f,
                    0.0f, 0.5f / b
                };

                float2x2 transformMatrix = mul(scalingMatrix, rotationMatrix); //Combine scale and rotation (scale then rotate)

                //Calculate bounding box for kernel traversal
                int maxX = int(sqrt(a * a * cosTheta * cosTheta + b * b * sinTheta * sinTheta));
                int maxY = int(sqrt(a * a * sinTheta * sinTheta + b * b * cosTheta * cosTheta));

                //For polynomial weighting function
                float zeta = _Zeta; // Controls the filter shape
                float zeroCross = _ZeroCrossing; // Where weight function becomes zero
                float sinZeroCross = sin(zeroCross);
                
                //Calculate eta parameter for polynomial weights
                float eta = (zeta + cos(zeroCross)) / (sinZeroCross * sinZeroCross);

                //Arrays to store sector means and second moments
                float4 sectorMeans[8];
                float3 sectorSecondMoments[8];
        
                //Initialize accumulator arrays
                [unroll(8)]
                for (int k = 0; k < _NumOfSectorsInKernel; ++k) {
                    sectorMeans[k] = 0.0f;
                    sectorSecondMoments[k] = 0.0f;
                }

                [loop]
                for (int y = -maxY; y <= maxY; ++y) {
                    [loop]
                    for (int x = -maxX; x <= maxX; ++x) {
                        //Transform coordinates to normalized ellipse space
                        float2 ellipseCoord = mul(transformMatrix, float2(x, y));

                        //Check if inside unit circle in transformed space (ellipse in original space)
                        if (dot(ellipseCoord, ellipseCoord) <= 0.25f) {
                            // Sample the image at this kernel position, then saturate it to ensure its within [0,1]
                            float2 samplePos = i.uv + float2(x, y) * _MainTex_TexelSize.xy;
                            float3 pixelColor = tex2D(_MainTex, samplePos).rgb;
                            pixelColor = saturate(pixelColor);

                            //Calculate sector weights
                            float sectorWeights[8];
                            float weightSum = 0.0f;
                            float vxx, vyy, z;
                            
                            //Calculate weights for cardinal direction sectors (0,2,4,6)
                            vxx = zeta - eta * ellipseCoord.x * ellipseCoord.x;
                            vyy = zeta - eta * ellipseCoord.y * ellipseCoord.y;
                    
                            //North sector
                            z = max(0, ellipseCoord.y + vxx);
                            sectorWeights[0] = z * z;
                            weightSum += sectorWeights[0];
                    
                            //West sector
                            z = max(0, -ellipseCoord.x + vyy);
                            sectorWeights[2] = z * z;
                            weightSum += sectorWeights[2];
                    
                            //South sector
                            z = max(0, -ellipseCoord.y + vxx);
                            sectorWeights[4] = z * z;
                            weightSum += sectorWeights[4];
                    
                            //East sector
                            z = max(0, ellipseCoord.x + vyy);
                            sectorWeights[6] = z * z;
                            weightSum += sectorWeights[6];
                    
                            //Rotate coordinates by 45 degrees for diagonal sectors
                            float2 rotatedCoord = sqrt(2.0f) / 2.0f * float2(ellipseCoord.x - ellipseCoord.y, ellipseCoord.x + ellipseCoord.y);
                            vxx = zeta - eta * rotatedCoord.x * rotatedCoord.x;
                            vyy = zeta - eta * rotatedCoord.y * rotatedCoord.y;
                    
                            //Northeast sector
                            z = max(0, rotatedCoord.y + vxx);
                            sectorWeights[1] = z * z;
                            weightSum += sectorWeights[1];
                    
                            //Northwest sector
                            z = max(0, -rotatedCoord.x + vyy);
                            sectorWeights[3] = z * z;
                            weightSum += sectorWeights[3];
                    
                            //Southwest sector
                            z = max(0, -rotatedCoord.y + vxx);
                            sectorWeights[5] = z * z;
                            weightSum += sectorWeights[5];
                    
                            //Southeast sector
                            z = max(0, rotatedCoord.x + vyy);
                            sectorWeights[7] = z * z;
                            weightSum += sectorWeights[7];
                    
                            //Apply Gaussian falloff based on distance from center
                            float gaussianWeight = exp(-3.125f * dot(ellipseCoord, ellipseCoord)) / weightSum;
                    
                            //Accumulate weighted color statistics for each sector
                            [unroll(8)]
                            for (int k = 0; k < 8; ++k) {
                                float sectorWeight = sectorWeights[k] * gaussianWeight;
                                sectorMeans[k] += float4(pixelColor * sectorWeight, sectorWeight);
                                sectorSecondMoments[k] += pixelColor * pixelColor * sectorWeight;
                            }
                        }
                    }
                }
                float4 col = 0; //results to return back
                
                [unroll(8)]
                for (k = 0; k < _NumOfSectorsInKernel; ++k) {
                    float3 meanColor = sectorMeans[k].rgb / sectorMeans[k].w; //Normalize means by weight
                    float3 variance = abs(sectorSecondMoments[k] / sectorMeans[k].w - meanColor * meanColor);

                    //Sum of variances across color channels
                    float totalVariance = variance.r + variance.g + variance.b;
            
                    //Weight inversely proportional to variance (preserves edges), _Hardness and _Sharpness control edge preservation strength
                    float varianceWeight = 1.0f / (1.0f + pow(_Hardness * 1000.0f * totalVariance, 0.5f * _Sharpness));

                    col += float4(meanColor * varianceWeight, varianceWeight);
                }
                
                return saturate(col / col.w);
            }
            ENDCG
        }
    }
}