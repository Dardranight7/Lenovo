Shader "Custom/BodyStencil"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry-10" }

      
        // Aquí escribimos stencil
        Stencil
        {
            Ref 5          // El valor que va a dejar en el buffer
            Comp Always    // Siempre pasa el test de stencil
            Pass Replace   // Reemplaza con Ref donde hay fragmento visible
        }

        ZWrite On         // Escribe al depth buffer (importante para ocultar detrás de la ropa real)
        ZTest LEqual      // Respeta visibilidad por cámara
        ColorMask 0       // No escribe nada en el color buffer (invisible)
        Cull Back


        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return 0; // no importa, ColorMask 0 lo descarta
                //return half4(1,0,0,1); // rojo puro
            }
            ENDHLSL
        }
    }
}
