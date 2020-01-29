Shader "Unlit/Circles"
{
    Properties
    {
        _ScreenRatio("Screen Ratio", float) = 1.0
        
        _BorderWidth("Border Width", float) = 0.0
        _BorderColor("Border Color", Color) = (0.3, 0.4, 1.0, 1.0)
        
        _BackgroundColor("Background Color", Color) = (0.0, 0.0, 0.0, 1.0)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            const static int _MAX_CIRCLE_COUNT = 16;
            float4 _Circles[_MAX_CIRCLE_COUNT];
            float4 _CircleColors[_MAX_CIRCLE_COUNT];
            
            float _ScreenRatio;
            
            float _BorderWidth;
            fixed4 _BorderColor;
            
            fixed4 _BackgroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f input) : SV_Target
            {                
                float2 frag_pos = input.uv;
                frag_pos.x *= _ScreenRatio;
                
                float4 circle;
                
                for (int i = 0; i < _MAX_CIRCLE_COUNT; i++)
                {
                    circle = _Circles[i];

                    if (circle.w <= 0.0)
                    {
                        continue;
                    }
                    
                    circle.x *= _ScreenRatio;
                    
                    float dist = distance(frag_pos, circle.xy);
                    
                    if (dist <= circle.z - _BorderWidth)
                    {
                        return _CircleColors[i];
                    }
                    else if (dist <= circle.z)
                    {
                        return _BorderColor;
                    }
                }
                
                return _BackgroundColor;
            }
            ENDCG
        }
    }
}
