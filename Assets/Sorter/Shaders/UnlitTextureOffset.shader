Shader"Custom/UnlitTextureOffset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OffsetVelocity ("Texture Offset Horizontal Velocity", Float) = 0.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _Color;
            float _OffsetVelocity;
            CBUFFER_END


            Varyings vert(Attributes IN) 
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv * _MainTex_ST.xy - float2(_OffsetVelocity * _TimeParameters.x, 0);
                return OUT;
            }

half4 frag(Varyings IN) : SV_Target
{
    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
}
            ENDHLSL
        }
    }
}