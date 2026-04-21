Shader "Custom/ToonOutlineWithAlpha"
{
    Properties
    {
        // --- 本体用の設定 ---
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        // --- ★重要★ セルシェーディング（アニメ調影）の設定 ---
        _ToonThreshold("Toon Threshold", Range(0.0, 1.0)) = 0.5
        _ToonSmoothness("Toon Smoothness", Range(0.0, 1.0)) = 0.05
        _ToonShadowColor("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)

        // --- アウトライン設定 ---
        _OutlineColor("Outline Color", Color) = (1,0,0,1) // 赤
        _OutlineWidth("Outline Width", Range(0, 0.1)) = 0.01
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType"="TransparentCutout" "Queue"="AlphaTest" }

        // --- 工程1：アウトライン（背面膨張） ---
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // URPの基本的な関数を使うために必須
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // SRP Batcher対応
            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
                float4 _BaseColor;
                float _Cutoff;
                // 本体用のプロパティもここに宣言しておく必要がある
                float _ToonThreshold;
                float _ToonSmoothness;
                float4 _ToonShadowColor;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            Varyings vert(Attributes input) {
                Varyings output;
                float3 posWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normWS = TransformObjectToWorldNormal(input.normalOS);
                
                // 法線方向に膨らませる
                float3 outlinePosWS = posWS + normWS * _OutlineWidth;
                
                output.positionCS = TransformWorldToHClip(outlinePosWS);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                // アルファテスト
                float alpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a;
                clip(alpha - _Cutoff);
                return _OutlineColor;
            }
            ENDHLSL
        }

        // --- ★重要★ 工程2：本体（自作のライティング） ---
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" } // URPのメイン描画パス
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // ★重要★ 光の計算に必要な辞書を読み込む
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                // ★重要★ 光の計算には「世界座標での法線」が必要
                float3 normalWS : TEXCOORD1;
            };

            // SRP Batcher対応（Outlineと同じものをコピペ）
            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
                float4 _BaseColor;
                float _Cutoff;
                float _ToonThreshold;
                float _ToonSmoothness;
                float4 _ToonShadowColor;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            Varyings vert(Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                // 法線を世界座標へ変換して渡す
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                // 1. アルファテスト（Outlineと同じ）
                float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                clip(albedo.a - _Cutoff);

                // 2. ★重要★ ライティングの計算
                // 法線を正規化（長さを1にする）
                float3 normalWS = normalize(input.normalWS);
                // メインライト（太陽）の情報を取得
                Light mainLight = GetMainLight();
                
                // 【世界で最も擦られた計算】法線とライト方向の内積（NdotL）
                // これで「光がどれくらい正面から当たっているか」がわかる
                float NdotL = dot(normalWS, mainLight.direction);
                // 値を0〜1の範囲に直す
                float diffuse = saturate(NdotL);

                // 3. 【デザイン表現】セルシェーディング（アニメ調）へ変換
                // 0〜1のグラデーションを、threshold（閾値）でパキッと分ける
                float toon = smoothstep(_ToonThreshold - _ToonSmoothness, _ToonThreshold + _ToonSmoothness, diffuse);
                // 影の色と本体の色を混ぜる
                float3 finalColor = lerp(_ToonShadowColor.rgb, albedo.rgb, toon);
                
                // ライトの色を掛ける（これで光の影響を受ける）
                finalColor *= mainLight.color;

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}