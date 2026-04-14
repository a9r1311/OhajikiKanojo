Shader "Custom/WorldSpaceGround"
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
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Tiling;
                float4 _Color;
            CBUFFER_END

            // 頂点シェーダー：座標変換を行う
            Varyings vert (Attributes input)
            {
                Varyings output;
                // オブジェクトのローカル座標をワールド座標に変換
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.worldPos = vertexInput.positionWS;
                return output;
            }

            // ピクセルシェーダー：色を塗る
            half4 frag (Varyings input) : SV_Target
            {
                // ここが魔法のロジック：
                // 通常のUVの代わりに、世界の横(x)と奥行き(z)の座標をUVとして使う
                float2 worldUV = input.worldPos.xz * _Tiling;

                // 座標をあえて粗くする
                float pixelSize = 600.0; // 数字が大きいほどドットが大きくなる
                float2 pixelatedUV = floor(worldUV * pixelSize) / pixelSize;

                // テクスチャをサンプリング
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelatedUV);
                col.rgb = floor(col.rgb * 100) / 100;
                return col * _Color;
            }
            ENDHLSL
        }
    }
}