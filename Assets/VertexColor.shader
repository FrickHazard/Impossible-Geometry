Shader "Vertex color unlit" {
	Properties{
		_StencilMask("Stencil Mask", Int) = 1
		_MainTex("Texture", 2D) = "white" {}
	}

		Category {
		Tags{ "Queue" = "Transparent+1" }
		Lighting Off
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
	    }

		SubShader {
			Pass  {
				SetTexture[_MainTex] {
					Combine texture * primary DOUBLE
				}
			}
		}
	}
}