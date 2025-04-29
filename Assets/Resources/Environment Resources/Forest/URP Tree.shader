Shader "Custom/URP Tree"
{
    Properties
    {
        _BaseMap ("Albedo (RGBA)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _WindStrength ("Wind Strength", Float) = 0
        _WindScale ("Wind Scale", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        LOD 200
        Cull Off // Double-sided leaves

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _Color;
                float _Cutoff;
                float _WindStrength;
                float _WindScale;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            float noise(float3 p)
            {
                return frac(sin(dot(p, float3(12.9898,78.233,45.164))) * 43758.5453);
            }

            Varyings vert(Attributes input)
            {
                Varyings o;

                float3 displaced = input.positionOS.xyz;

                if (_WindStrength > 0.0001)
                {
                    float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                    float timeOffset = _Time.y * _WindScale;
                    float noiseValue = noise(worldPos * 0.2 + timeOffset);
                    float3 windOffset = float3(
                        sin(worldPos.y + timeOffset + noiseValue),
                        0.0,
                        cos(worldPos.x + timeOffset + noiseValue)
                    ) * _WindStrength;

                    displaced += windOffset;
                }

                o.positionHCS = TransformObjectToHClip(float4(displaced, 1.0));
                o.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                o.normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));
                o.positionWS = TransformObjectToWorld(displaced);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                clip(albedoAlpha.a - _Cutoff);

                float3 normalMap = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv));
                float3 normalWS = normalize(i.normalWS + normalMap);

                // Lighting
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float3 diffuse = albedoAlpha.rgb * _Color.rgb * mainLight.color * NdotL;

                // Ambient
                float3 ambient = SampleSH(normalWS);
                float3 finalColor = diffuse + ambient * albedoAlpha.rgb * _Color.rgb;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
