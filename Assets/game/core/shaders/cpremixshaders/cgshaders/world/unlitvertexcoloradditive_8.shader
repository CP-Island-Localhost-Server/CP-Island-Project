Shader "CpRemix/World/Unlit Vertex Color Additive" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		LOD 100
		Tags { "QUEUE" = "Transparent" }
		Pass {
			LOD 100
			Tags { "QUEUE" = "Transparent" }
			Blend One One, One One
			ZClip Off
			ZWrite Off
			Cull Off
			GpuProgramID 56366
			// No subprograms found
		}
	}
	Fallback "Diffuse"
}