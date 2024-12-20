Shader "CpRemix/World/Water" {
	Properties {
		_Color ("Water Color", Color) = (0,0.5,1,0.7)
		_WavesMap ("Waves r=shore g=diffuse b=spec", 2D) = "white" {}
		_ShoreFoamBrightness ("Shore Foam Brightness", Range(0, 2)) = 1
		_ShoreTile ("Shore Waves Tile", Range(0.05, 299)) = 1
		_ShoreWavesColor ("Shore Waves Color", Color) = (0,0,1,1)
		_ShoreWavesTimeScale ("Shore Time Scale", Range(0.05, 5)) = 1.2
		_ShoreWavesOpacity ("Shore Waves Opacity", Range(0.05, 1)) = 0.5
		_ShoreWavesUVDirection ("Shore Waves UV direction", Vector) = (0.5,0.5,0,0)
		_ShoreTextureSampleAmnt ("Shore Sample Amount", Range(0.05, 1)) = 0.5
		_DiffuseWavesBounce ("Diffuse Waves Bounce", Range(0, 0.1)) = 0.03
		_DiffuseTile ("Diffuse Waves Tile", Range(0.05, 299)) = 1
		_DiffuseWavesColor ("Diffuse Waves Color", Color) = (1,1,1,1)
		_DiffuseWavesTimeScale ("Diffuse Time Scale", Range(0.001, 5)) = 0.7
		_DiffuseWavesOpacity ("Diffuse Waves opacity", Range(0.05, 1)) = 0.5
		_DiffuseWavesUVDirection ("Diffuse Waves UV direction", Vector) = (1,0,0,0)
		_SpecWavesBounce ("Spec Waves Bounce", Range(0, 0.1)) = 0
		_SpecTile ("Spec Waaves Tile", Range(0.05, 299)) = 1
		_SpecWavesColor ("Spec Waves Color", Color) = (1,1,1,1)
		_SpecTimeScale ("Spec Time Scale", Range(0.001, 5)) = 1
		_SpecIntensity ("Specular Intensity", Range(0.05, 5)) = 1
		_SpecUVDirection ("Spec Waves UV direction", Vector) = (1,0,0,0)
		_Shininess ("Specular Shininess", Float) = 5
	}
	SubShader {
		LOD 200
		Tags { "QUEUE" = "Transparent" }
		Pass {
			LOD 200
			Tags { "QUEUE" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			GpuProgramID 14418
			// No subprograms found
		}
	}
}