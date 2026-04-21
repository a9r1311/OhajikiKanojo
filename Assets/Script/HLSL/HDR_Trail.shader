Shader "Custom/HDR_Trail"
{
    Properties
    {
        [HDR] _EmissionColor ("Emission Color", Color) = (2, 2, 2, 1) // HDRを有効化
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        Pass
        {
            // WebGLで最も軽く、明るく見える設定
            Blend One One // Additive（加算）合成：背景と色が重なってより明るくなる
            ZWrite Off    // 軌跡同士が重なっても不自然にならないよう深度書き込みOFF
            Cull Off      // Trail Rendererの両面を描画

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR; // Trail RendererのColor設定を受け取る
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _EmissionColor;
                float4 _MainTex_ST;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // テクスチャサンプリング
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // 【ここがポイント】
                // Trail Renderer側の色(input.color) 
                // × インスペクターのHDR色(_EmissionColor)
                // × テクスチャ(texColor)
                // すべて掛け合わせることで、1.0を大きく超える輝度を作ります。
                half4 finalColor = texColor * input.color * _EmissionColor;

                return finalColor;
            }
            ENDHLSL
        }
    }
}