Shader "UI/DiagonalScrollTiled"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Vector) = (0.1, 0.1, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "RenderPipeline"="UniversalPipeline" "PreviewType"="Plane" }

        Pass
        {
Name"UI"
            Tags
{"LightMode"="UniversalForward"
}

Blend SrcAlpha
OneMinusSrcAlpha
            Cull
Off
            ZWrite
Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct appdata_t
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
};

sampler2D _MainTex;
float4 _MainTex_ST;
float4 _ScrollSpeed;

v2f vert(appdata_t v)
{
    v2f o;
    o.position = TransformObjectToHClip(v.vertex.xyz);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
}

half4 frag(v2f i) : SV_Target
{
    float2 scrollOffset = _ScrollSpeed.xy * _Time.y;
    float2 uv = i.uv + scrollOffset;
    return tex2D(_MainTex, uv);
}
            ENDHLSL
        }
    }
}