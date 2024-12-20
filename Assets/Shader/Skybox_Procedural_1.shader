Shader "Skybox/Procedural" {
	Properties {
		[KeywordEnum(None, Simple, High Quality)] _SunDisk ("Sun", Float) = 2
		_SunSize ("Sun Size", Range(0, 1)) = 0.04
		_AtmosphereThickness ("Atmosphere Thickness", Range(0, 5)) = 1
		_SkyTint ("Sky Tint", Color) = (0.5,0.5,0.5,1)
		_GroundColor ("Ground", Color) = (0.369,0.349,0.341,1)
		_Exposure ("Exposure", Range(0, 8)) = 1.3
	}
	SubShader {
		Tags { "PreviewType" = "Skybox" "QUEUE" = "Background" "RenderType" = "Background" }
		Pass {
			Tags { "PreviewType" = "Skybox" "QUEUE" = "Background" "RenderType" = "Background" }
			ZClip Off
			ZWrite Off
			Cull Off
			GpuProgramID 33823
			// No subprograms found
		}
	}
}