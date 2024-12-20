Shader "CpRemix/Igloo/IglooFurnitureUnlit" {
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_Highlight ("Additional Highlight", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Geometry" "RenderType" = "Opaque" }
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Geometry" "RenderType" = "Opaque" }
			ZClip Off
			GpuProgramID 13439
			// No subprograms found
		}
	}
	Fallback "VertexLit"
}