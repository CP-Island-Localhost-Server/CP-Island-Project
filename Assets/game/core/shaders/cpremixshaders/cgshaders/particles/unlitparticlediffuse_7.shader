Shader "CpRemix/Particles/UnlitDiffuse" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor ("Tint Color", Color) = (1,1,1,1)
	}
	SubShader {
		LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "RenderType" = "Opaque" }
			ZClip Off
			GpuProgramID 58083
			// No subprograms found
		}
	}
}