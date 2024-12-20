Shader "CpRemix/World/UV Rotation Unlit (No Fog)" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_RotationSpeed ("Rotation Speed", Float) = 2
		_PivotX ("Pivot X", Float) = 0.5
		_PivotY ("Pivot Y", Float) = 0.5
	}
	SubShader {
		LOD 100
		Tags { "RenderType" = "Opaque" }
		Pass {
			LOD 100
			Tags { "RenderType" = "Opaque" }
			ZClip Off
			GpuProgramID 51744
			// No subprograms found
		}
	}
	Fallback "Mobile/Diffuse"
}