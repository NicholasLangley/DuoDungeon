Shader "Unlit/WorldGrid Shader"
{
    Properties
    {
        _GridSize ("Grid Size (Meters)", Float) = 1.0
        _GridWidth ("Line Width", Float) = 0.025
        _GridColor ("Grid Line Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };
            
            float _GridSize;
            float _GridWidth;
            fixed4 _GridColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //need to offset grid so it goes around the blocks rather than through the center
                float3 offsetPos = i.worldPos + _GridSize * 0.5f;
                
                //fractional part of the world pos from 0 - 1
                float3 gridUV = frac(offsetPos);
                
                
                
                //line masks for where the fractional value is close to 0 / 1
                float3 lineX = step(gridUV.x, _GridWidth) + step(1.0 - _GridWidth, gridUV.x);
                float3 lineY = step(gridUV.y, _GridWidth) + step(1.0 - _GridWidth, gridUV.y);
                float3 lineZ = step(gridUV.z, _GridWidth) + step(1.0 - _GridWidth, gridUV.z);
                
                float3 normal = abs(normalize(i.worldNormal));
                
                //saturate compresses values to be from 0 - 1, so anywhere its 1 means its on at least one line
                //normal multiplication will remove pixels that are only on the currently alligned plane
                //example if y plane, normal.y will = 1.0 and xz will be 0.
                //so lines xz will be multipled by 1 and line y by 0
                float gridMask = saturate(
                    (lineX * (normal.y + normal.z)) + 
                    (lineY * (normal.x + normal.z)) + 
                    (lineZ * (normal.x + normal.y)));
                if (gridMask < 0.1)
                {
                    discard;
                }
                
                return _GridColor;
            }
            ENDCG
        }
    }
}
