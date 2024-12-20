Shader "CpRemix/World/ScrollingTexture" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_XScrollSpeed ("X Scroll Speed", Float) = 1
		_YScrollSpeed ("Y Scroll Speed", Float) = 1
	}
	SubShader {
		Tags { "QUEUE" = "Geometry" }
		Pass {
			Tags { "QUEUE" = "Geometry" }
			GpuProgramID 32658
			// No subprograms found
		}
	}
	Fallback "Diffuse"
}