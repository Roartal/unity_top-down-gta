// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Self-Illumin/Custom" {
	Properties {
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	    _MaskTex ("Mask (RGB)", 2D) = "white" {}
	    _Illum ("Illumin (A)", 2D) = "white" {}
	    _Emission ("Emission (Lightmapper)", Float) = 1.0
	    _Color ("Primary", Color) = (1,1,1,1)
	    _Color1 ("Reference", Color) = (1, 1, 1, 1)
		_Color2 ("Secondary", Color) = (1, 1, 1, 1)
		_Color3 ("Rims", Color) = (1, 1, 1, 1)
		}
		SubShader {
		    Tags { "RenderType"="Opaque" }
		    LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex, _MaskTex;
		float4 _Color1, _Color2, _Color3;
		sampler2D _Illum;
		fixed4 _Color;
		fixed _Emission;

		struct Input {
		    float2 uv_MainTex;
		    float2 uv_Illum;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half3 m = tex2D (_MaskTex, IN.uv_MainTex);
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half3 res = m;
			if (m.r == 1) res = _Color1.rgb;
			else if (m.g == 1) res = _Color2.rgb;
			else if (m.b == 1) res = _Color3.rgb;
			else if (m.b < 0.6) res = _Color.rgb;
	
		   // fixed4 c = tex * _Color;
		    o.Emission = c.rgb * res;
		    o.Albedo = c.rgb * res;
		   // o.Alpha = c.a;
			}
		ENDCG
		}
	FallBack "Legacy Shaders/Self-Illumin/VertexLit"
	CustomEditor "LegacyIlluminShaderGUI"
}