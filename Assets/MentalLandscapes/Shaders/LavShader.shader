// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/LavShader"
{
	Properties
	{
		_MainTex("Texture 1", 2D) = ""
		_Texture2("Light Texture",2D) = ""
		_Texture3("Dark Texture",2D) = ""
		_Blend("Blend Amount", Float) = 0
		[HideInInspector]_RelativeVector("Vector", Vector) =(0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 normal: NORMAL;
			};

			sampler2D _MainTex;
			sampler2D _Texture2;
			sampler2D _Texture3;
			float3 _RelativeVector;
			float4 _Texture2_ST;
			float4 _MainTex_ST;
			float4 _Texture3_ST;
			float _Blend;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = v.normal;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float lerpAmount = dot(i.normal,_RelativeVector);
				lerpAmount = (lerpAmount + 1) / 2;
				fixed4 main = tex2D(_MainTex, i.uv);
				fixed4 col = tex2D(_Texture2, i.uv);
				fixed4 col2 = tex2D(_Texture3, i.uv);
				fixed4 lightCol = lerp(col, col2, lerpAmount);
				fixed4 result = lerp(lightCol, main, _Blend);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return result;
			}
			ENDCG
		}
	}
}
