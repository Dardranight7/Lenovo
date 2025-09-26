Shader "Custom/BodyStencil"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry-10" }

        Stencil
        {
            Ref 5
            Comp Always
            Pass Replace
        }

        ZWrite On
        ZTest LEqual
        ColorMask 0
        Cull Back

        Pass
        {
            Name "StencilPass"
            Tags { "LightMode" = "UniversalForward" } // URP lo reconoce

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return 0; // No importa, ColorMask 0 lo elimina
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
