Shader "CpRemix/World/Unlit Dynamic Object (FOG)" {
	Properties {
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Tags { "RenderType" = "Opaque" }
			ZClip Off
			GpuProgramID 44478
			// No subprograms found
		}
	}
}