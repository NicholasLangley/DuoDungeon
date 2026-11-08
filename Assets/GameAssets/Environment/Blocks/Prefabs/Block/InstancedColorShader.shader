Shader "Custom/InstancedColorShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow

        #pragma multi_compile_instancing
        
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            fixed4 _Color;
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 instancedColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * instancedColor;
            
             float4x4 ditherMatrix = float4x4(
                1.0/17.0,  9.0/17.0,  3.0/17.0,  11.0/17.0,
                13.0/17.0, 5.0/17.0,  15.0/17.0, 7.0/17.0,
                4.0/17.0,  12.0/17.0, 2.0/17.0,  10.0/17.0,
                16.0/17.0, 8.0/17.0,  14.0/17.0, 6.0/17.0
            );
            
            // Calculate the exact pixel screen coordinates
            int2 pixelPos = int2(IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy);
            
            // Look up the threshold for this specific pixel (loops every 4 pixels)
            float threshold = ditherMatrix[pixelPos.x % 4][pixelPos.y % 4];

            // 3. Clip/discard the pixel if its threshold is higher than the instance's alpha.
            // If alpha is 1.0, nothing is discarded. If alpha is 0.5, exactly half the pixels drop.
            clip(c.a - threshold);
            
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
            UNITY_APPLY_DITHER_CROSSFADE(IN.screenPos.xy / IN.screenPos.w);
            
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
