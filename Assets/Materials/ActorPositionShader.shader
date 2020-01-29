// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/AgentCaptureShader"
{
    Properties
    {
        _PointSize("Point Size", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        ZWrite Off
        ZTest Always
        Blend One One
        BlendOp Add

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            float _PointSize = 10;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float size: PSIZE;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.size = _PointSize;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(0.5, 1, 0, 1);
            }
            ENDCG
        }
    }
}
