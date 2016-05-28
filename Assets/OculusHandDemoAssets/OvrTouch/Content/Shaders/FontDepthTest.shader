/********************************************************************************//**
\file      FontDepthTest.shader
\brief     Font shader with depth test -- copied from the Unity built-in shader:
		   builtin_shaders-5.1.2f1\DefaultResources\Font.shader.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

Shader "OvrTouch/FontDepthTest" {

	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
	}

	SubShader {

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

		//==============================================================================
		// Render Pass
		//==============================================================================

		Pass {

			Lighting Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;

			//==============================================================================
			// Main
			//==============================================================================

			//==============================================================================
			v2f vert (appdata_t v) {
				v2f output;
				output.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				output.color = v.color * _Color;
				output.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return output;
			}

			//==============================================================================
			fixed4 frag (v2f input) : SV_Target {
				fixed4 color = input.color;
				color.a *= tex2D(_MainTex, input.texcoord).a;
				return color;
			}

			ENDCG

		}

	}

}
