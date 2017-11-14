Shader "Unlit/Impossible/StencilClearerShader"
{
	Properties
	{
		_StencilMask("Stencil Mask", Int) = 1
	}
		SubShader
		{
			Stencil
			{
				Ref[_StencilMask]
				Comp Equal
				Pass Zero
				ZFail Zero
			}
			Tags{ "Queue" = "Geometry+1" }
			Pass{
			ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

		CGPROGRAM

		#pragma vertex vert 
		#pragma fragment frag

		float4 vert(float4 vertexPos : POSITION) : SV_POSITION
		{
			return UnityObjectToClipPos(vertexPos);
		}

		float4 frag(void) : COLOR
		{
			return float4(0.0, 0.0, 0.0, 0.0);
		}

		ENDCG
	}
	}
}