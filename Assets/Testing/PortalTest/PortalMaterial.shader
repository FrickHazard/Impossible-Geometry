// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Test/Portal" {
Properties {
	[HideInInspector] _ReflectionTex ("Internal Reflection", 2D) = "" {}
}


// -----------------------------------------------------------
// Fragment program cards


Subshader {
	Tags { "WaterMode"="Refractive" "RenderType"="Opaque" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#pragma multi_compile WATER_REFRACTIVE WATER_REFLECTIVE WATER_SIMPLE

#if defined (WATER_REFLECTIVE) || defined (WATER_REFRACTIVE)
#define HAS_REFLECTION 1
#endif
#if defined (WATER_REFRACTIVE)
#define HAS_REFRACTION 1
#endif


#include "UnityCG.cginc"

struct appdata {
	float4 vertex : POSITION;
};

struct v2f {
	float4 pos : SV_POSITION;
	float4 ref : TEXCOORD0;
};

v2f vert(appdata v)
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.ref = ComputeNonStereoScreenPos(o.pos);
	return o;
}


sampler2D _ReflectionTex;
sampler2D _ReflectiveColor;

half4 frag( v2f i ) : SV_Target
{
	float4 uv1 = i.ref;
	half4 refl = tex2Dproj( _ReflectionTex, UNITY_PROJ_COORD(uv1) );
	half4 color;
	color.rgb = refl.rgb;
	color.a = refl.a;
	return color;
}
ENDCG

	}
}

}