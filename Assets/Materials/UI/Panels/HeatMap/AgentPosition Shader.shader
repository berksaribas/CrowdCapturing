Shader "Unlit/AgentPositionShader"
{
    Properties
    {
        _PixelRadius ("Radius in Pixels", Float) = 10
        _ScreenAspect ("Screen Aspect", Float) = 1
        _Pow ("Power", Float) = 4
    }
    SubShader
    {
        Cull Off
        Lighting Off
        ZTest Off
        ZWrite Off
        Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform float _PixelRadius = 10;
            uniform float _Pow = 4;
            
            struct appdata
            {
                float3 vertex : POSITION;
            };
            
            struct v2g
            {
                float4 vertex : SV_POSITION;
            };

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex /= o.vertex.w;
                return o;
            }
            
            struct g2f
            {
                float4 vertex : SV_POSITION;
                float2 uv: UV;
            };
            
            [maxvertexcount(6)]
            void geom(point v2g IN[1], inout TriangleStream<g2f> triStream)
            {
                const float renderTargetPixelWidth = _ScreenParams.x;
                const float renderTargetPixelHeight = _ScreenParams.y;
                const float4 up = float4(0, -_PixelRadius / renderTargetPixelHeight, 0, 0);
                const float4 left = float4(_PixelRadius / renderTargetPixelWidth, 0, 0, 0);
                const float4 pos = IN[0].vertex;
                const float4 vertices[4] = {
                    pos + up + left,
                    pos + up - left,
                    pos - up + left,
                    pos - up - left,
                };
                
                const float4 uv_up = float4(0, 1, 0, 0);
                const float4 uv_left = float4(1, 0, 0, 0);
                const float4 uvs[4] = {
                    + uv_up + uv_left,
                    + uv_up - uv_left,
                    - uv_up + uv_left,
                    - uv_up - uv_left,
                };
                
                g2f o;
                
                o.vertex = vertices[0];
                o.uv = uvs[0];
                triStream.Append(o);
                o.vertex = vertices[1];
                o.uv = uvs[1];
                triStream.Append(o);
                o.vertex = vertices[2];
                o.uv = uvs[2];
                triStream.Append(o);
                triStream.RestartStrip();
                
                o.vertex = vertices[1];
                o.uv = uvs[1];
                triStream.Append(o);
                o.vertex = vertices[2];
                o.uv = uvs[2];
                triStream.Append(o);
                o.vertex = vertices[3];
                o.uv = uvs[3];
                triStream.Append(o);
            }

            float frag (g2f i) : SV_Target
            {
                float dist = dot(i.uv, i.uv);
                float density = 1 - dist;
                
                clip(density);
                
                return pow(density, _Pow);
            }
            ENDCG
        }
        
        /*
        GrabPass
        {
            "_DensityMapTexture"
        }
        
        Blend Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform float _PixelRadius = 10;
            uniform float _DensityHigh = 4;
            
            struct appdata
            {
                float3 vertex : POSITION;
            };
            
            struct v2g
            {
                float4 vertex : SV_POSITION;
            };

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex /= o.vertex.w;
                return o;
            }
            
            struct g2f
            {
                float4 vertex : SV_POSITION;
                float4 grabPos : TEXCOORD0;
            };
            
            [maxvertexcount(6)]
            void geom(point v2g IN[1], inout TriangleStream<g2f> triStream)
            {
                const float renderTargetPixelWidth = _ScreenParams.x;
                const float renderTargetPixelHeight = _ScreenParams.y;
                const float4 up = float4(0, -_PixelRadius / renderTargetPixelHeight, 0, 0);
                const float4 left = float4(_PixelRadius / renderTargetPixelWidth, 0, 0, 0);
                const float4 pos = IN[0].vertex;
                const float4 vertices[4] = {
                    pos + up + left,
                    pos + up - left,
                    pos - up + left,
                    pos - up - left,
                };
                
                g2f o;
                
                o.vertex = vertices[0];
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                triStream.Append(o);
                o.vertex = vertices[1];
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                triStream.Append(o);
                o.vertex = vertices[2];
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                triStream.Append(o);
                triStream.RestartStrip();
                
                o.vertex = vertices[1];
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                triStream.Append(o);
                o.vertex = vertices[2];
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                triStream.Append(o);
                o.vertex = vertices[3];
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                triStream.Append(o);
            }
            
            sampler2D _DensityMapTexture;

            fixed4 frag (g2f i) : SV_Target
            {
                /* Trippy
                float density = tex2Dproj(_DensityMapTexture, i.grabPos).r;
                clip(density - 0.05);
                return sin(density * 10);
                */

                float density = tex2Dproj(_DensityMapTexture, i.grabPos).r;
                clip(density - 0.0308);

                // Normalize
                density = smoothstep(0, _DensityHigh, density);
                
                const fixed4 red = fixed4(1, 0, 0, 1);
                const fixed4 green = fixed4(0, 1, 0, 1);
                fixed4 densityColor = lerp(green, red, density);
                densityColor.a = smoothstep(0, 1, density);
                
                return densityColor;
            }
            ENDCG
        }
        */
        
//        Pass
//        {
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma geometry geom
//            #pragma fragment frag
//
//            #include "UnityCG.cginc"
//            
//            struct appdata
//            {
//                float4 vertex : POSITION;
//            };
//            
//            struct v2g
//            {
//                float4 vertex : SV_POSITION;
//            };
//
//            v2g vert (appdata v)
//            {
//                v2g o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                //o.vertex /= o.vertex.w;
//                return o;
//            }
//            
//            struct g2f
//            {
//                float4 vertex : SV_POSITION;
//            };
//            
//            [maxvertexcount(4)]
//            void geom(triangle v2g IN[3], inout LineStream<g2f> lineStream)
//            {
//                g2f o;
//                
//                o.vertex = IN[0].vertex;
//                lineStream.Append(o);
//                o.vertex = IN[1].vertex;
//                lineStream.Append(o);
//                o.vertex = IN[2].vertex;
//                lineStream.Append(o);
//                o.vertex = IN[0].vertex;
//                lineStream.Append(o);
//            }
//
//            fixed4 frag (g2f i) : SV_Target
//            {
//            
//                return fixed4(1, 0, 0, 1);
//            }
//            ENDCG
//        }
    }
}
