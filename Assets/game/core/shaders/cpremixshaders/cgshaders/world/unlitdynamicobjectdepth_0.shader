Shader "CpRemix/World/Unlit Dynamic Object Depth" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_DepthMultiply ("DepthMultiply", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Tags { "RenderType" = "Opaque" }
			GpuProgramID 1518
			// No subprograms found
		}
	}
}