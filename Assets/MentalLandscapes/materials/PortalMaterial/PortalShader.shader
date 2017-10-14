// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PortalShader" {
	Properties{
		[HideInInspector] _ReflectionTex("Internal Reflection", 2D) = "" {}

	}

		Subshader{
		Tags{ "WaterMode" = "Refractive" "RenderType" = "Opaque" }
		Pass{
		Lighting Off
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#pragma multi_compile WATER_REFRACTIVE WATER_REFLECTIVE WATER_SIMPLE

#include "UnityCG.cginc"

	uniform float _ReflDistort;

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		float4 ref : TEXCOORD0;
		float2 bumpuv0 : TEXCOORD1;
		float2 bumpuv1 : TEXCOORD2;
		float3 viewDir : TEXCOORD3;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.viewDir.xzy = WorldSpaceViewDir(v.vertex);
		o.ref = ComputeScreenPos(o.pos);
		return o;
	}


	sampler2D _ReflectionTex;

	half4 frag(v2f i) : SV_Target
	{
		i.viewDir = normalize(i.viewDir);
	// fresnel factor
	
	float4 uv1 = i.ref;
	half4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(uv1));

	half4 color;

	color.rgb = refl.rgb;
	return color;
	}
		ENDCG

	}
	}

}
