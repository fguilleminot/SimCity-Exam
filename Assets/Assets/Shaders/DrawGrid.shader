Shader "Custom/DrawGrid" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Pas ("Pas de grille", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:myVert
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		float _Pas;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};
		
		void myVert (inout appdata_full v, out Input IN)
		{
			IN.worldPos = WorldSpaceViewDir(v.vertex);
		}

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = (modf((IN.worldPos.x, _Pas) == 0) || (modf(IN.worldPos.z, _Pas) == 0) ? 0 : c.a);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
