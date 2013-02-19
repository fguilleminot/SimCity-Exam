Shader "Custom/MapColoration" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BaseColor ("Color Basse Altitude", Color) = (0,0,0,0)
		_HeightColor ("Color Haute Altitude", Color) = (0,0,0,0)
		_MaxAltitude ("Altitude Max", Float) = 10
		_Proportion ("Proportion", Float) = 5
		//_MaxHeightMap ("Altitude Max de la Map", Float) = 0
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

		//void myColoration (Input IN, SurfaceOutput o, inout fixed4 color)
		//{
		//	color = lerp(_BaseColor, _HeightColor, IN.weight);
		//}
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = lerp(_BaseColor, _HeightColor, IN.weight); //tex2D (_MainTex, IN.uv_MainTex).rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
