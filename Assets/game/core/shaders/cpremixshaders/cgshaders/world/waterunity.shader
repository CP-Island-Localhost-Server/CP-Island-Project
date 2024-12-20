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
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float2 texcoord5 : TEXCOORD5;
				float3 texcoord3 : TEXCOORD3;
				float texcoord6 : TEXCOORD6;
				float3 texcoord4 : TEXCOORD4;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _LightColor0;
			float _Shininess;
			float _SpecIntensity;
			float _ShoreTile;
			float _ShoreWavesTimeScale;
			float2 _ShoreWavesUVDirection;
			float _DiffuseWavesBounce;
			float _DiffuseTile;
			float _DiffuseWavesTimeScale;
			float2 _DiffuseWavesUVDirection;
			float _SpecWavesBounce;
			float _SpecTile;
			float _SpecTimeScale;
			float2 _SpecUVDirection;
			// $Globals ConstantBuffers for Fragment Shader
			float4 _Color;
			float _ShoreFoamBrightness;
			float4 _ShoreWavesColor;
			float _ShoreWavesOpacity;
			float _ShoreTextureSampleAmnt;
			float4 _DiffuseWavesColor;
			float _DiffuseWavesOpacity;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _WavesMap;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp0.xyz = unity_ObjectToWorld._m03_m13_m23 * v.vertex.www + tmp0.xyz;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;
                tmp0.w = dot(float2(_DiffuseWavesTimeScale.x, _DiffuseWavesUVDirection.x), float2(_DiffuseWavesTimeScale.x, _DiffuseWavesUVDirection.x));
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xy = tmp0.ww * _DiffuseWavesUVDirection;
                tmp1.xy = tmp1.xy * _Time.xx;
                tmp1.xy = tmp1.xy * _DiffuseWavesTimeScale.xx;
                tmp0.w = _SinTime.w * _DiffuseWavesBounce + 1.0;
                tmp1.xy = tmp1.xy * tmp0.ww + v.texcoord.xy;
                o.texcoord1.xy = tmp1.xy * _DiffuseTile.xx;
                tmp0.w = dot(_ShoreWavesTimeScale, _ShoreWavesTimeScale);
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xy = tmp0.ww * _ShoreWavesUVDirection;
                tmp1.xy = tmp1.xy * _Time.xx;
                tmp1.xy = tmp1.xy * _ShoreWavesTimeScale.xx + v.texcoord.xy;
                o.texcoord.xy = tmp1.xy * _ShoreTile.xx;
                tmp0.w = dot(_SpecUVDirection, _SpecUVDirection);
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xy = tmp0.ww * _SpecUVDirection;
                tmp1.xy = tmp1.xy * _Time.xx;
                tmp1.xy = tmp1.xy * _SpecTimeScale.xx + v.texcoord.xy;
                tmp1.xy = tmp1.xy * _SpecTile.xx;
                tmp0.w = _SinTime.w * _SpecWavesBounce + 1.0;
                o.texcoord2.xy = tmp0.ww * tmp1.xy;
                tmp0.w = v.color.x - v.color.y;
                tmp0.w = tmp0.w - v.color.z;
                tmp0.w = max(tmp0.w, 0.0);
                o.texcoord5.xy = tmp0.ww * float2(1.0, -1.0) + float2(0.0, 1.0);
                tmp1.xyz = -tmp0.xyz * _WorldSpaceLightPos0.www + _WorldSpaceLightPos0.xyz;
                tmp0.xyz = _WorldSpaceCameraPos - tmp0.xyz;
                tmp0.w = dot(tmp1.xyz, tmp1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xyz = tmp0.www * tmp1.xyz;
                tmp2.x = dot(v.normal.xyz, unity_WorldToObject._m00_m10_m20);
                tmp2.y = dot(v.normal.xyz, unity_WorldToObject._m01_m11_m21);
                tmp2.z = dot(v.normal.xyz, unity_WorldToObject._m02_m12_m22);
                tmp0.w = dot(tmp2.xyz, tmp2.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp2.xyz = tmp0.www * tmp2.xyz;
                tmp0.w = dot(-tmp1.xyz, tmp2.xyz);
                tmp0.w = tmp0.w + tmp0.w;
                tmp1.xyz = tmp2.xyz * -tmp0.www + -tmp1.xyz;
                tmp0.w = dot(tmp0.xyz, tmp0.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp0.xyz = tmp0.www * tmp0.xyz;
                tmp0.x = dot(tmp1.xyz, tmp0.xyz);
                tmp0.x = max(tmp0.x, 0.0);
                tmp0.x = tmp0.x * _Shininess + -_Shininess;
                tmp0.x = tmp0.x + 1.0;
                tmp0.x = max(tmp0.x, 0.0);
                tmp0.xyz = tmp0.xxx * _LightColor0.xyz;
                tmp0.xyz = tmp0.xyz * _SpecIntensity.xxx;
                tmp0.xyz = max(tmp0.xyz, float3(0.5, 0.5, 0.5));
                tmp0.w = max(tmp0.y, tmp0.x);
                o.texcoord6.x = max(tmp0.z, tmp0.w);
                o.texcoord3.xyz = tmp0.xyz;
                tmp0.x = v.color.x > 0.0;
                tmp0.y = v.color.x < 0.0;
                tmp0.x = tmp0.y - tmp0.x;
                o.texcoord4.x = floor(tmp0.x);
                tmp0.x = 1.0 - _SinTime.w;
                tmp0.x = min(tmp0.x, 2.0);
                o.texcoord4.y = tmp0.x - v.color.x;
                tmp0.x = _CosTime.w + 1.5;
                tmp0.x = -tmp0.x * 0.5 + 1.0;
                o.texcoord4.z = max(tmp0.x, 0.0);
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0.x = inp.texcoord4.y > 0.0;
                tmp0.y = inp.texcoord4.y < 0.0;
                tmp0.x = tmp0.y - tmp0.x;
                tmp0.x = floor(tmp0.x);
                tmp0.x = tmp0.x + 1.0;
                tmp0.x = min(tmp0.x, 1.0);
                tmp0.x = tmp0.x * inp.texcoord4.y;
                tmp0.x = tmp0.x * inp.texcoord4.z;
                tmp0.x = tmp0.x * inp.texcoord4.x;
                tmp0.x = tmp0.x * _ShoreFoamBrightness + 1.0;
                tmp1 = tex2D(_WavesMap, inp.texcoord.xy);
                tmp0.y = tmp1.x * _ShoreTextureSampleAmnt + -_ShoreTextureSampleAmnt;
                tmp0.z = tmp1.x * _ShoreWavesOpacity;
                tmp0.z = tmp0.z * inp.texcoord5.x;
                tmp0.z = tmp0.x * tmp0.z;
                tmp0.y = tmp0.y + 1.0;
                tmp0.x = tmp0.x * tmp0.y;
                tmp0.x = tmp0.x * _ShoreWavesOpacity;
                tmp0.xyw = tmp0.xxx * _ShoreWavesColor.xyz;
                tmp0.xyw = tmp0.xyw * inp.texcoord5.xxx + _Color.xyz;
                tmp1 = tex2D(_WavesMap, inp.texcoord1.xy);
                tmp1.x = tmp1.y * _DiffuseWavesOpacity;
                tmp1.yzw = tmp1.xxx * _DiffuseWavesColor.xyz;
                tmp0.xyw = tmp1.yzw * inp.texcoord5.yyy + tmp0.xyw;
                tmp2 = tex2D(_WavesMap, inp.texcoord2.xy);
                tmp2 = tmp2.zzzz * inp.texcoord3.xyz;
                o.sv_target.xyz = tmp2.xyz * inp.texcoord5.yyy + tmp0.xyw;
                tmp0.x = max(tmp1.x, tmp2.w);
                tmp0.x = tmp0.x * inp.texcoord5.y;
                tmp0.x = max(tmp0.z, tmp0.x);
                o.sv_target.w = max(tmp0.x, _Color.w);
                return o;
			}
			ENDCG
		}
	}
}