Shader "Custom/HorizonBackdrop"
{
    Properties
    {
        _MainTex ("Backdrop Texture", 2D) = "white" {}
        [HDR] _Color ("Tint Color", Color) = (1, 1, 1, 1)
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.1
        _FogContribution ("Fog Contribution", Range(0, 1)) = 0.5
        _BottomFade ("Bottom Fade Height (UV Y)", Range(0, 0.5)) = 0.05
        _TopFade ("Top Fade Height (UV Y)", Range(0.5, 1.0)) = 0.95
        _UvTileX ("Horizontal Tiling", Float) = 4.0
        _UvOffsetY ("Vertical UV Offset", Range(-1, 1)) = 0.0
        _SkyBlendColor ("Sky Blend Color", Color) = (0.56, 0.78, 0.85, 1.0)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent-50" 
            "RenderPipeline" = "UniversalPipeline" 
            "IgnoreProjector" = "True"
        }

        LOD 100
        Cull Front // Inverted cylinder: render inside
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "UniversalForward"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float fogFactor : TEXCOORD1;
                float4 vertexColor : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _SkyBlendColor;
                float _Cutoff;
                float _FogContribution;
                float _BottomFade;
                float _TopFade;
                float _UvTileX;
                float _UvOffsetY;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = positionInputs.positionCS;
                
                // Tiled UV horizontally
                float2 uv = input.uv;
                uv.x = uv.x * _UvTileX;
                uv.y = uv.y + _UvOffsetY;
                output.uv = uv;

                output.vertexColor = input.color;
                output.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;

                // Vertical gradient fading at bottom and top edges
                float v = input.uv.y;
                float alphaFade = 1.0;
                if (v < _BottomFade && _BottomFade > 0.001)
                {
                    alphaFade *= smoothstep(0.0, _BottomFade, v);
                }
                if (v > _TopFade && _TopFade < 0.999)
                {
                    alphaFade *= smoothstep(1.0, _TopFade, v);
                }

                col.a *= alphaFade;
                clip(col.a - _Cutoff);

                // Blend with fog according to _FogContribution
                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    half3 foggedColor = MixFogColor(col.rgb, unity_FogColor.rgb, input.fogFactor);
                    col.rgb = lerp(col.rgb, foggedColor, _FogContribution);
                #endif

                return col;
            }
            ENDHLSL
        }
    }
}
