Shader "Depth Mask" {
	SubShader {
		Tags{"Queue" = "Geometry+10"}
		Colormask A
		ztest Lequal
		Zwrite on
		Pass{
			
			}
	} 
	FallBack "Diffuse", 1
}
