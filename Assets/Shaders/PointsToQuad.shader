Shader "Custom/GeometryShaderTest1" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Amount ("Height Adjustment", Float) = 0.0
    }
    SubShader {
        Pass {
         //   Tags { "RenderType"="Opaque" }
         //   LOD 200

            CGPROGRAM
            #pragma target 5.0
            //#pragma addshadow
            #pragma vertex vert
            #pragma geometry GS_Main
            #pragma fragment frag
            #include "UnityCG.cginc"

			uniform float size; // Particle size

			// Vert input
			 struct vertexInput {
				 float4 vertex : POSITION;
				//float4 tex : TEXCOORD0;
				float4 color : COLOR;
			 };

            // Vert to geo
            struct v2g
            {
                float4 pos: POSITION;
				float4 color: COLOR;
            };

            // geo to frag
            struct g2f
            {
                float4 pos: POSITION;
            };


            // Vars
            fixed4 _Color;
            float _Amount;


            // Vertex modifier function
            v2g vert(vertexInput input) {

                v2g output;
				output.pos = input.vertex;
 				output.color = input.color;

				return output;
            }

            // GS_Main(point v2g p[1], inout TriangleStream<g2f> triStream)
            // GS_Main(line v2g p[2], inout TriangleStream<g2f> triStream)
            // GS_Main(triangle v2g p[3], inout TriangleStream<g2f> triStream)
            [maxvertexcount(4)]
            void GS_Main(point v2g p[1], inout TriangleStream<g2f> triStream)
            {
				float3 right = (UNITY_MATRIX_MV[0][0], 
                    UNITY_MATRIX_MV[1][0], 
                    UNITY_MATRIX_MV[2][0]);

				float3 up = (UNITY_MATRIX_MV[0][1], 
                 UNITY_MATRIX_MV[1][1], 
                 UNITY_MATRIX_MV[2][1]);

				float3 P = p[0].pos.xyz;
				
				float3 va = P - (right + up) * size;
				p[0].pos = UNITY_MATRIX_MVP * float4(va, 1.0);
				Vertex_UV = float2(0.0, 0.0);
				Vertex_Color = p[0].color;
				EmitVertex(); 
			}

            fixed4 frag(g2f input) : COLOR {
                return float4(1.0, 0.0, 0.0, 1.0); 
            }

            ENDCG
        }


    }

   // FallBack "Diffuse"
}