Shader "CpRemix/Avatar Body Preview" {
	Properties {
		_Diffuse ("Diffuse", 2D) = "black" {}
		_BodyColorsMaskTex ("Body Color Mask", 2D) = "black" {}
		_BodyRedChannelColor ("Body Red Channel Color", Color) = (1,0,0,1)
		_BodyGreenChannelColor ("Body Green Channel Color", Color) = (1,1,0,1)
		_BodyBlueChannelColor ("Body Blue Channel Color", Color) = (1,0,1,1)
		_DetailAndMatcapMaskAndEmissive ("r=detail g=MatCapMask b=emissive", 2D) = "black" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "ALWAYS" }
			ZClip Off
			GpuProgramID 8890
			// No subprograms found
		}
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			Blend DstColor SrcColor, DstColor SrcColor
			ZClip Off
			GpuProgramID 121101
			// No subprograms found
		}
	}
}