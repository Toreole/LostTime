Shader "Spheya/Ice"
{
    Properties
    {
        _MainTex("Ice Texture", 2D) = "white" {}
        _Depth("Paralax", float) = 0.1
        _Tint("Tint", Color) = (1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        float _Depth;
        float3 _Tint;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            const int SAMPLES = 20;

            float4 color = tex2D(_MainTex, IN.uv_MainTex);

            float2 paralax = (IN.viewDir.xz / IN.viewDir.y) * _Depth / SAMPLES;

            for (int i = 1; i < SAMPLES / 2; i++)
                color += (1.0 / SAMPLES) * tex2D(_MainTex, IN.uv_MainTex + paralax * i);

            for (int i = SAMPLES / 4; i < SAMPLES / 4 + SAMPLES / 2; i++)
                color += (1.0 / SAMPLES) * tex2D(_MainTex, -(IN.uv_MainTex + paralax * i));

            o.Albedo = color.rgb * _Tint;
            o.Smoothness = 1.0 - color.r;
            o.Metallic = 0.0;

            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
