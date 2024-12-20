Shader "CpRemix/World/Snow Ramp" {
	Properties {
		_SnowRampTex ("Snow Ramp", 2D) = "white" {}
		[HideInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Tags { "RenderType" = "Opaque" }
			GpuProgramID 34332
			// No subprograms found
		}
	}
}