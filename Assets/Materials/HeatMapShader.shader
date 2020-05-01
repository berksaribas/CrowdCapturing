Shader "Custom/HeatMapShader"
{
	Properties
	{
	    _DefaultColor("Default Color", Color) = (0, 0, 0, 1)
	    
	    _ColdColorLimit("Cold Color Limit", Float) = 0.6
	    _ColdColor("Cold Color", Color) = (0.24, 0.94, 0, 1)
	    
        _MidColorLimit("Mid Color Limit", Float) = 3
	    _MidColor("Mid Color", Color) = (0.94, 0.94, 0, 1)
	    
	    _HotColorLimit("Hot Color Limit", Float) = 6
	    _HotColor("Hot Color", Color) = (0.94, 0, 0, 1)
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
			
			static const float _HeatMapResolution = 31;
			const float _AgentPositionGrid[_HeatMapResolution * _HeatMapResolution];
			
			float _Multisample;
			
			const fixed4 _DefaultColor;
			const float _ColdColorLimit;
			const fixed4 _ColdColor;
			const float _MidColorLimit;
			const fixed4 _MidColor;
			const float _HotColorLimit;
			const fixed4 _HotColor;
			
			static const float _ColdToMidRange = _MidColorLimit - _ColdColorLimit;
			static const float _MidToHotRange = _HotColorLimit - _MidColorLimit;
			
			uint4x2 calcGridCoordinates(float2 uv)
			{
			    uint2 f = floor(uv * (_HeatMapResolution - 1));
			    uint2 c = ceil(uv * (_HeatMapResolution - 1));
			    return uint4x2(
			        f.x, f.y,
			        f.x, c.y,
			        c.x, f.y,
			        c.x, c.y
			    );
			}
			
			uint getDensity(uint2 gridCoordinate)
			{
			    return _AgentPositionGrid[
			        gridCoordinate.x + gridCoordinate.y * _HeatMapResolution
			    ];
			}
			
			float getDensity(uint4x2 gridCoordinates, float2 uv)
			{
			    float4 densities = {
			        getDensity(gridCoordinates[0]),
			        getDensity(gridCoordinates[1]),
			        getDensity(gridCoordinates[2]),
			        getDensity(gridCoordinates[3])
			    };
			    
			    float2 f = frac(uv * (_HeatMapResolution - 1));
			    
			    return lerp(
			        lerp(densities[0], densities[1], f.y),
			        lerp(densities[2], densities[3], f.y),
			        f.x
			    );
			}
			
			fixed4 calcHeatColor(float density)
			{
			    if (density < _ColdColorLimit)
			    {
			        return _DefaultColor;
			    }
			    
			    if (density < _MidColorLimit)
			    {
			        return lerp(_ColdColor, _MidColor, (density - _ColdColorLimit) / _ColdToMidRange);
			    }
			    
			    if (density < _HotColorLimit)
			    {
			        return lerp(_MidColor, _HotColor, (density - _MidColorLimit) / _MidToHotRange);
			    }
			    
			    return _HotColor;
			}

			fixed4 frag (v2f i) : SV_Target
			{
                int4x2 coordinates = calcGridCoordinates(i.uv);
                float density = getDensity(coordinates, i.uv);
                return calcHeatColor(density);
			}
			ENDCG
		}
	}
}
