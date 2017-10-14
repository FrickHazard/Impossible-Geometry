// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ForwardRendering" {
		Properties{
			_Color("Main Color", Color) = (1,1,1,1)
			_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
			_Shininess("Shininess", Range(0.03, 1)) = 0.078125
			_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		}
			SubShader{
			Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }

			Pass{
			Tags{ "LightMode" = "ForwardBase" }                      // This Pass tag is important or Unity may not give it the correct light information.
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdbase                       // This line tells Unity to compile this pass for forward base.
#pragma fragmentoption ARB_fog_exp2
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0

#include "UnityCG.cginc"
#include "AutoLight.cginc"

			struct v2f
		{
			float4  pos         : SV_POSITION;
			float2  uv          : TEXCOORD0;
			float3  viewDir     : TEXCOORD1;
			float3  lightDir    : TEXCOORD2;
			LIGHTING_COORDS(3,4)                            // Macro to send shadow  attenuation to the vertex shader.
		};

		v2f vert(appdata_tan v)
		{
			v2f o;

			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord.xy;
			TANGENT_SPACE_ROTATION;                         // Macro for unity to build the Object>Tangent rotation matrix "rotation".
			o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
			o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));

			TRANSFER_VERTEX_TO_FRAGMENT(o);                 // Macro to send shadow  attenuation to the fragment shader.
			return o;
		}

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		half _Shininess;

		fixed4 _SpecColor;
		fixed4 _LightColor0; // Colour of the light used in this pass.

		fixed4 frag(v2f i) : COLOR
		{
			i.viewDir = normalize(i.viewDir);
		i.lightDir = normalize(i.lightDir);

		fixed atten = LIGHT_ATTENUATION(i); // Macro to get you the combined shadow  attenuation value.

		fixed4 tex = tex2D(_MainTex, i.uv);
		fixed gloss = tex.a;
		tex *= _Color;
		fixed3 normal = UnpackNormal(tex2D(_BumpMap, i.uv));

		half3 h = normalize(i.lightDir + i.viewDir);

		fixed diff = saturate(dot(normal, i.lightDir));

		float nh = saturate(dot(normal, h));
		float spec = pow(nh, _Shininess * 128.0) * gloss;

		fixed4 c;
		//c.rgb = tex.rgb;         // Ambient term. Only do this in Forward Base. It only needs calculating once.
		c.rgb = (tex.rgb * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2); // Diffuse and specular.
		c.rgb *= 
		c.a = tex.a + _LightColor0.a * _SpecColor.a * spec * atten;
		return c;
		}
			ENDCG
		}

			Pass{
			Tags{ "LightMode" = "ForwardAdd" }                       // Again, this pass tag is important otherwise Unity may not give the correct light information.
			Blend One One                                           // Additively blend this pass with the previous one(s). This pass gets run once per pixel light.
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdadd                        // This line tells Unity to compile this pass for forward add, giving attenuation information for the light.
			//#pragma multi_compile_fwdadd_fullshadows                      // This line tells Unity to compile this pass for forward add and give shadow information as well as attenuation. Swap this line for the one above if you want forward add with shadows.
#pragma fragmentoption ARB_fog_exp2
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0

#include "UnityCG.cginc"
#include "AutoLight.cginc"

			struct v2f
		{
			float4  pos         : SV_POSITION;
			float2  uv          : TEXCOORD0;
			float3  viewDir     : TEXCOORD1;
			float3  lightDir    : TEXCOORD2;
			LIGHTING_COORDS(3,4)                            // Macro to send shadow  attenuation to the vertex shader.
		};

		v2f vert(appdata_tan v)
		{
			v2f o;

			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord.xy;
			TANGENT_SPACE_ROTATION;                         // Macro for unity to build the Object>Tangent rotation matrix "rotation".
			o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
			o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));

			TRANSFER_VERTEX_TO_FRAGMENT(o);                 // Macro to send shadow  attenuation to the fragment shader.
			return o;
		}

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		half _Shininess;

		fixed4 _SpecColor;
		fixed4 _LightColor0; // Colour of the light used in this pass.

		fixed4 frag(v2f i) : COLOR
		{
			i.viewDir = normalize(i.viewDir);
		i.lightDir = normalize(i.lightDir);

		fixed atten = LIGHT_ATTENUATION(i); // Macro to get you the combined shadow  attenuation value.

		fixed4 tex = tex2D(_MainTex, i.uv);
		fixed gloss = tex.a;
		tex *= _Color;
		fixed3 normal = UnpackNormal(tex2D(_BumpMap, i.uv));

		half3 h = normalize(i.lightDir + i.viewDir);

		fixed diff = saturate(dot(normal, i.lightDir));

		float nh = saturate(dot(normal, h));
		float spec = pow(nh, _Shininess * 128.0) * gloss;

		fixed4 c;
		c.rgb = (tex.rgb * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2); // Diffuse and specular.
		c.a = tex.a + _LightColor0.a * _SpecColor.a * spec * atten;
		return c;
		}
			ENDCG
		}
		}
			FallBack "VertexLit"    // Use VertexLit's shadow caster/receiver passes.
	}