Shader "CpRemix/World/WorldObject Depth" {
	Properties {
		_Diffuse ("Diffuse Texture", 2D) = "" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			ZClip Off
			GpuProgramID 32076
			// No subprograms found
		}
	}
}