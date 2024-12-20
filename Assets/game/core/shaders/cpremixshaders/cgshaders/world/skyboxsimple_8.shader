Shader "CpRemix/Skybox/Simple Cubemap Shader" {
	Properties {
		_cubemap ("Environment Map", Cube) = "white" {}
	}
	SubShader {
		Tags { "QUEUE" = "Background" }
		Pass {
			Tags { "QUEUE" = "Background" }
			ZClip Off
			ZWrite Off
			Cull Off
			GpuProgramID 52663
			// No subprograms found
		}
	}
}