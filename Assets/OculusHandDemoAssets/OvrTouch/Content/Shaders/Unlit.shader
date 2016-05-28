/********************************************************************************//**
\file      Unlit.shader
\brief     Unlit with color and texture -- copied from the Unity built-in shader:
		   builtin_shaders-5.1.2f1\DefaultResourcesExtra\Unlit\Unlit-Normal.shader.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

Shader "OvrTouch/Unlit" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {

		Tags { "RenderType"="Opaque" }
		LOD 100
	
		//==============================================================================
		// Render Pass
		//==============================================================================

		Pass {

			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(0)
				};
				
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;
		
				//==============================================================================
				// Main
				//==============================================================================
			
				//==============================================================================
				v2f vert (appdata_t v) {
					v2f output;
					output.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					output.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					UNITY_TRANSFER_FOG(output, output.vertex);
					return output;
				}
			
				//==============================================================================
				fixed4 frag (v2f input) : COLOR {
					fixed4 color = tex2D(_MainTex, input.texcoord) * _Color;
					UNITY_APPLY_FOG(input.fogCoord, color);
					UNITY_OPAQUE_ALPHA(color.a);
					return color;
				}
			
			ENDCG

		}

	}

}
