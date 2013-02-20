Shader "Custom/MapColoration" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BaseColor ("Color Basse Altitude", Color) = (0,0,0,0)
		_HeightColor ("Color Haute Altitude", Color) = (0,0,0,0)
		_MaxAltitude ("Altitude Max", Float) = 10
		_Proportion ("Proportion", Float) = 5
	}
	SubShader {

		CGPROGRAM
		#pragma surface surf Lambert vertex:myVert
		#include "UnityCG.cginc"

		float4		_HeightColor;
		float4		_BaseColor;
		float		_MaxAltitude;
		float		_Proportion;
		sampler2D	_MainTex;

		struct Input {
			float2	uv_MainTex;
			float	weight;
		};
		
		void myVert (inout appdata_full v, out Input IN)
		{
			float3 worldPos = WorldSpaceViewDir(v.vertex);
			IN.weight = saturate((v.vertex.y - _MaxAltitude) / _Proportion);
			
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = lerp(_BaseColor, _HeightColor, IN.weight);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
