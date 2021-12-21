Shader "Spheya/Ice"
{
    Properties
    {
        _MainTex("Ice Texture", 2D) = "white" {}
        _Depth("Paralax", float) = 0.1
        _BlendDepth("Transparency", Range(0.0, 1.0)) = 0.007
        _Tint("Tint", Color) = (1, 1, 1)
        _Exposure("Exposure", float) = 1.0
        //_SecondTex("Sand Texture", 2D) = "white"{}
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "ForceNoShadowCasting" = "True"}
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            LOD 200

            CGPROGRAM

            #pragma surface surf Standard fullforwardshadows alpha:fade
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            //sampler2D _SecondTex;

            float _Depth;
            float _BlendDepth;
            fixed3 _Tint;
            float _Exposure;

            struct Input
            {
                float2 uv_MainTex;
                float3 viewDir;
                float4 screenPos;
                float3 worldPos;
            };

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                const int SAMPLES = 20;

                float2 clipPosXY = IN.screenPos.xy / IN.screenPos.w;
                float depth01 = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, clipPosXY).r);
                float4 viewDir = mul(unity_CameraInvProjection, float4 (clipPosXY * 2.0 - 1.0, 1.0, 1.0));
                float depth = length((viewDir.xyz / viewDir.w) * depth01);


                float4 color = tex2D(_MainTex, IN.uv_MainTex);
                //float4 secondColor = tex2D(_SecondTex, IN.uv_MainTex);
                float dist = distance(_WorldSpaceCameraPos.xyz, IN.worldPos);

                float2 paralax = (IN.viewDir.xz / IN.viewDir.y) * _Depth / SAMPLES;

                for (int i = 1; i < SAMPLES / 2; i++)
                    color += (1.0 / SAMPLES) * tex2D(_MainTex, IN.uv_MainTex + paralax * i);

                for (int i = SAMPLES / 4; i < SAMPLES / 4 + SAMPLES / 2; i++)
                    color += (1.0 / SAMPLES) * tex2D(_MainTex, -(IN.uv_MainTex + paralax * i));

                o.Albedo = color.rgb * _Tint * _Exposure;//lerp(secondColor, color.rgb * _Tint, saturate(1.0 - pow(_BlendDepth, (depth - dist))));
                o.Smoothness = 1.0 - color.r;
                o.Metallic = 0.0;

                float grayscale = (o.Albedo.r + o.Albedo.g + o.Albedo.b) / 3.0;

                o.Alpha = clamp(grayscale, min(0.85, saturate(1.0 - pow(_BlendDepth, (depth - dist)))), 1);
            }
            ENDCG
        }
            FallBack "Diffuse"
}
