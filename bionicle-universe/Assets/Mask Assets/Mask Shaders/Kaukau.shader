Shader "Custom/Kaukau" {
	Properties {
		_MainColor ("Color", Color) = (0, 0, 0, 0)
		_Transparency ("Transparency (R)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _Transparency;
		float4 _Color;

		struct Input {
			float2 uv_Transparency;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_Transparency, IN.uv_Transparency);
			o.Albedo = _Color.rgb;
			o.Alpha = c.r;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
