Shader "Custom/URP SpeedTree Leaves"
{
    Properties
    {
        _BaseMap ("Albedo (RGBA)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _TranslucencyColor ("Translucency Color", Color) = (1,1,1,1)
        _TranslucencyViewDependency ("Translucency View Dependency", Range(0,1)) = 0.7
        _ShadowStrength ("Shadow Strength", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        LOD 200
        Cull Off

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
                float3 viewDirWS : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _Color;
                float _Cutoff;
                float _ShadowStrength;
                float4 _TranslucencyColor;
                float _TranslucencyViewDependency;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            Varyings vert(Attributes input)
            {
                Varyings o;
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = normalize(TransformObjectToWorldNormal(input.normalOS));

                o.positionHCS = TransformObjectToHClip(input.positionOS);
                o.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                o.normalWS = normalWS;
                o.positionWS = worldPos;
                o.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                clip(albedoAlpha.a - _Cutoff);

                float3 normalMap = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv));
                float3 normalWS = normalize(i.normalWS + normalMap);

                // Main directional light
                Light mainLight = GetMainLight();
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float3 baseColor = albedoAlpha.rgb * _Color.rgb;
                float3 diffuse = baseColor * mainLight.color * NdotL * _ShadowStrength;

                // Translucency
                float backLight = saturate(dot(-mainLight.direction, i.normalWS));
                float viewDot = pow(1.0 - saturate(dot(normalWS, i.viewDirWS)), 2.0);
                float translucency = backLight * viewDot * _TranslucencyViewDependency;
                float3 transColor = _TranslucencyColor.rgb * baseColor * translucency;

                // Ambient
                float3 ambient = SampleSH(normalWS);
                float3 finalColor = diffuse + transColor + ambient * baseColor;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
