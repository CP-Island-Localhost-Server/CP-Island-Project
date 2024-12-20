Shader "CpRemix/Particles/UnlitVertexColor" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "RenderType" = "Opaque" }
			ZClip Off
			GpuProgramID 57857
			// No subprograms found
		}
	}
}