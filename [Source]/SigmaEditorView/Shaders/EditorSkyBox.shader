Shader "SigmaEditorView/EditorSkyBox"
{
	Properties
	{
		_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
		[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
		[NoScaleOffset] _CubeMap ("Cubemap   (HDR)", Cube) = "black" { }
	}
	SubShader
	{
		Tags
		{
			"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"
		}
		Cull Off ZWrite Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"
			samplerCUBE _CubeMap;
			half4 _CubeMap_HDR;
			half4 _Tint;
			half _Exposure;
			uniform float4x4 _Rotation;
			struct appdata_t
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = mul(_Rotation, v.vertex);
				o.vertex = UnityObjectToClipPos(o.vertex);
				o.texcoord = v.vertex.xyz;
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				half4 cubemap = texCUBE (_CubeMap, i.texcoord);
				half3 c = DecodeHDR (cubemap, _CubeMap_HDR);
				c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
				c *= _Exposure;
				return half4(c, 1);
			}
			ENDCG
		}
	}
	Fallback Off
}
