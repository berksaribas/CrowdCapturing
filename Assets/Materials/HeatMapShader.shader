Shader "Custom/HeatMapShader"
{
	Properties
	{
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			
			v2f vert (appdata i)
			{
			    v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv = i.uv;
			    return o;
			}
			
			const int _HeatMapGranularity = 32;
			const float _AgentHeatMap[32 * 32];
			
			fixed4 calcHeatColor(float density)
			{
			    return fixed4(density / 14, min(density / 7, 0.4), 0, 1);
			}

			fixed4 frag (v2f i) : SV_Target
			{
			    int x = _HeatMapGranularity - floor(i.uv.x * _HeatMapGranularity);
			    int y = _HeatMapGranularity - floor(i.uv.y * _HeatMapGranularity);
				float density = _AgentHeatMap[x * _HeatMapGranularity + y];
				return calcHeatColor(density);
			}
			ENDCG
		}
	}
}
