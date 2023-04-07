Shader "Custom/DisableZwrite"
{
	SubShader{
		Tags{
		"RenderType" = "Opaque"
		}

		Pass{
			Zwrite Off

		}
	}
}
