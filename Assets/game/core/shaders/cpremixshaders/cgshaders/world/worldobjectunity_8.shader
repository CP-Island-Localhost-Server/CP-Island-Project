Shader "CpRemix/World/WorldObject" {
	Properties {
		_Diffuse ("Diffuse Texture", 2D) = "" {}
		[HideInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			ZClip Off
			GpuProgramID 46021
			// No subprograms found
		}
	}
}