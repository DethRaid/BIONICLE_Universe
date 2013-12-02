Shader "Selfmade/TransparentShadowReceiver" 
{ 

Properties 
{ 
 	// Usual stuffs
	_Color ("Main Color", Color) = (1,1,1,1)	
	// Shadow Stuff
	_ShadowIntensity ("Shadow Intensity", Range (0, 1)) = 0.6
} 


SubShader 
{ 
	Tags {
	"Queue"="AlphaTest" 
	"IgnoreProjector"="True" 
	"RenderType"="Transparent"
	}

	LOD 300


// Main Surface Pass (Handles Spot/Point lights)
CGPROGRAM
		#pragma surface surf BlinnPhong alpha vertex:vert fullforwardshadows approxview
		float4 _Color;
		
		struct v2f { 
			V2F_SHADOW_CASTER; 
			float2 uv : TEXCOORD1;
		};

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			//float3 viewDir;
		};

		v2f vert (inout appdata_full v) { 
			v2f o; 
			return o; 
		} 

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo =  _Color.rgb;
			o.Alpha = _Color.a;
		}
ENDCG

		// Shadow Pass : Adding the shadows (from Directional Light)
		// by blending the light attenuation
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha 
			Name "ShadowPass"
			Tags {"LightMode" = "ForwardBase"}
			  
			CGPROGRAM 
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members lightDir)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
 
			struct v2f { 
				float2 uv_MainTex : TEXCOORD1;
				float4 pos : SV_POSITION;
				LIGHTING_COORDS(3,4)
				float3	lightDir;
			};
 
			float4 _MainTex_ST;

			sampler2D _MainTex;
			float4 _Color;
			float _ShadowIntensity;
 
			v2f vert (appdata_full v)
			{
				v2f o;
                o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.lightDir = ObjSpaceLightDir( v.vertex );
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			float4 frag (v2f i) : COLOR
			{
				float atten = LIGHT_ATTENUATION(i);
				
				half4 c;
				c.rgb =  0;
				c.a = (1-atten) * _ShadowIntensity * (tex2D(_MainTex, i.uv_MainTex).a); 
				return c;
			}
			ENDCG
		}
	
	
}

FallBack "Transparent/Cutout/VertexLit"
}