Shader "Custom/WorldSpaceGroundWithShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Tiling Scale", Float) = 1.0
        _Color ("Color Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" } // ライトの影響を受ける設定

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // 影を有効にするためのキーワード
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" // 追加：ライト計算用

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
                float4 shadowCoord : TEXCOORD1; // 追加：影の位置情報
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Tiling;
                float4 _Color;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.worldPos = vertexInput.positionWS;
                
                // 追加：頂点から影の計算用の座標を取得
                output.shadowCoord = GetShadowCoord(vertexInput);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float2 worldUV = input.worldPos.xz * _Tiling;
                float pixelSize = 900.0;
                float2 pixelatedUV = floor(worldUV * pixelSize) / pixelSize;

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelatedUV);
                col.rgb = floor(col.rgb * 100) / 170;

                // --- 影の計算を追加 ---
                // メインライトの情報を取得
                Light mainLight = GetMainLight(input.shadowCoord);
                // 影の濃さを取得（shadowAttenuation: 1=影なし, 0=影）
                half shadow = mainLight.shadowAttenuation;
                
                // 最終的な色に影を掛け算する
                col.rgb *= shadow;
                // ---------------------

                return col * _Color;
            }
            ENDHLSL
        }

        // ShadowCaster Pass がないと、この地面自体が他のものに影を落とせませんが
        // 今回は「地面が影を受ける」ことが目的なので上記ForwardLitの修正で影が出るはずです。
    }
}