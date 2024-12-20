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
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float texcoord : TEXCOORD0;
				float3 texcoord1 : TEXCOORD1;
				float3 texcoord2 : TEXCOORD2;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _Exposure;
			float3 _GroundColor;
			float3 _SkyTint;
			float _AtmosphereThickness;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			
			// Keywords: _SUNDISK_NONE
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                float4 tmp4;
                float4 tmp5;
                float4 tmp6;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;
                tmp1.xyz = float3(1.0, 1.0, 1.0) - _SkyTint;
                tmp1.xyz = tmp1.xyz * float3(0.3, 0.3, 0.3) + float3(0.5, 0.42, 0.325);
                tmp1.xyz = tmp1.xyz * tmp1.xyz;
                tmp1.xyz = tmp1.xyz * tmp1.xyz;
                tmp1.xyz = float3(1.0, 1.0, 1.0) / tmp1.xyz;
                tmp0.w = log(_AtmosphereThickness);
                tmp0.w = tmp0.w * 2.5;
                tmp0.w = exp(tmp0.w);
                tmp2.xy = tmp0.ww * float2(0.05, 0.0314159);
                tmp0.w = dot(tmp0.xyz, tmp0.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp3.xyz = tmp0.www * tmp0.xyz;
                tmp0.x = tmp3.y >= 0.0;
                if (tmp0.x) {
                    tmp0.x = tmp3.y * tmp3.y + 0.050625;
                    tmp0.x = sqrt(tmp0.x);
                    tmp0.x = -tmp0.y * tmp0.w + tmp0.x;
                    tmp0.y = -tmp3.y * 1.0 + 1.0;
                    tmp0.z = tmp0.y * 5.25 + -6.8;
                    tmp0.z = tmp0.y * tmp0.z + 3.83;
                    tmp0.z = tmp0.y * tmp0.z + 0.459;
                    tmp0.y = tmp0.y * tmp0.z + -0.00287;
                    tmp0.y = tmp0.y * 1.442695;
                    tmp0.y = exp(tmp0.y);
                    tmp0.xyz = tmp0.xyx * float3(0.5, 0.2460318, 20.0);
                    tmp4.xyz = tmp0.xxx * tmp3.xyz;
                    tmp4.xyz = tmp4.xyz * float3(0.5, 0.5, 0.5) + float3(0.0, 1.0001, 0.0);
                    tmp0.w = dot(tmp4.xyz, tmp4.xyz);
                    tmp0.w = sqrt(tmp0.w);
                    tmp1.w = 1.0 - tmp0.w;
                    tmp1.w = tmp1.w * 230.8312;
                    tmp1.w = exp(tmp1.w);
                    tmp2.z = dot(_WorldSpaceLightPos0.xyz, tmp4.xyz);
                    tmp2.z = tmp2.z / tmp0.w;
                    tmp2.w = dot(tmp3.xyz, tmp4.xyz);
                    tmp0.w = tmp2.w / tmp0.w;
                    tmp2.z = 1.0 - tmp2.z;
                    tmp2.w = tmp2.z * 5.25 + -6.8;
                    tmp2.w = tmp2.z * tmp2.w + 3.83;
                    tmp2.w = tmp2.z * tmp2.w + 0.459;
                    tmp2.z = tmp2.z * tmp2.w + -0.00287;
                    tmp2.z = tmp2.z * 1.442695;
                    tmp2.z = exp(tmp2.z);
                    tmp0.w = 1.0 - tmp0.w;
                    tmp2.w = tmp0.w * 5.25 + -6.8;
                    tmp2.w = tmp0.w * tmp2.w + 3.83;
                    tmp2.w = tmp0.w * tmp2.w + 0.459;
                    tmp0.w = tmp0.w * tmp2.w + -0.00287;
                    tmp0.w = tmp0.w * 1.442695;
                    tmp0.w = exp(tmp0.w);
                    tmp0.w = tmp0.w * 0.25;
                    tmp0.w = tmp2.z * 0.25 + -tmp0.w;
                    tmp0.w = tmp1.w * tmp0.w + tmp0.y;
                    tmp0.w = max(tmp0.w, 0.0);
                    tmp0.w = min(tmp0.w, 50.0);
                    tmp5.xyz = tmp1.xyz * tmp2.yyy + float3(0.0125664, 0.0125664, 0.0125664);
                    tmp6.xyz = -tmp0.www * tmp5.xyz;
                    tmp6.xyz = tmp6.xyz * float3(1.442695, 1.442695, 1.442695);
                    tmp6.xyz = exp(tmp6.xyz);
                    tmp0.w = tmp0.z * tmp1.w;
                    tmp4.xyz = tmp3.xyz * tmp0.xxx + tmp4.xyz;
                    tmp0.x = dot(tmp4.xyz, tmp4.xyz);
                    tmp0.x = sqrt(tmp0.x);
                    tmp1.w = 1.0 - tmp0.x;
                    tmp1.w = tmp1.w * 230.8312;
                    tmp1.w = exp(tmp1.w);
                    tmp2.z = dot(_WorldSpaceLightPos0.xyz, tmp4.xyz);
                    tmp2.z = tmp2.z / tmp0.x;
                    tmp2.w = dot(tmp3.xyz, tmp4.xyz);
                    tmp0.x = tmp2.w / tmp0.x;
                    tmp2.z = 1.0 - tmp2.z;
                    tmp2.w = tmp2.z * 5.25 + -6.8;
                    tmp2.w = tmp2.z * tmp2.w + 3.83;
                    tmp2.w = tmp2.z * tmp2.w + 0.459;
                    tmp2.z = tmp2.z * tmp2.w + -0.00287;
                    tmp2.z = tmp2.z * 1.442695;
                    tmp2.z = exp(tmp2.z);
                    tmp0.x = 1.0 - tmp0.x;
                    tmp2.w = tmp0.x * 5.25 + -6.8;
                    tmp2.w = tmp0.x * tmp2.w + 3.83;
                    tmp2.w = tmp0.x * tmp2.w + 0.459;
                    tmp0.x = tmp0.x * tmp2.w + -0.00287;
                    tmp0.x = tmp0.x * 1.442695;
                    tmp0.x = exp(tmp0.x);
                    tmp0.x = tmp0.x * 0.25;
                    tmp0.x = tmp2.z * 0.25 + -tmp0.x;
                    tmp0.x = tmp1.w * tmp0.x + tmp0.y;
                    tmp0.x = max(tmp0.x, 0.0);
                    tmp0.x = min(tmp0.x, 50.0);
                    tmp4.xyz = tmp5.xyz * -tmp0.xxx;
                    tmp4.xyz = tmp4.xyz * float3(1.442695, 1.442695, 1.442695);
                    tmp4.xyz = exp(tmp4.xyz);
                    tmp0.x = tmp0.z * tmp1.w;
                    tmp0.xyz = tmp0.xxx * tmp4.xyz;
                    tmp0.xyz = tmp6.xyz * tmp0.www + tmp0.xyz;
                    tmp4.xyz = tmp1.xyz * tmp2.xxx;
                    tmp4.xyz = tmp0.xyz * tmp4.xyz;
                    tmp0.xyz = tmp0.xyz * float3(0.02, 0.02, 0.02);
                } else {
                    tmp0.w = min(tmp3.y, -0.001);
                    tmp0.w = -0.0001 / tmp0.w;
                    tmp5.xyz = tmp0.www * tmp3.xyz + float3(0.0, 1.0001, 0.0);
                    tmp1.w = dot(-tmp3.xyz, tmp5.xyz);
                    tmp2.z = dot(_WorldSpaceLightPos0.xyz, tmp5.xyz);
                    tmp1.w = 1.0 - tmp1.w;
                    tmp2.w = tmp1.w * 5.25 + -6.8;
                    tmp2.w = tmp1.w * tmp2.w + 3.83;
                    tmp2.w = tmp1.w * tmp2.w + 0.459;
                    tmp1.w = tmp1.w * tmp2.w + -0.00287;
                    tmp1.w = tmp1.w * 1.442695;
                    tmp1.w = exp(tmp1.w);
                    tmp2.z = 1.0 - tmp2.z;
                    tmp2.w = tmp2.z * 5.25 + -6.8;
                    tmp2.w = tmp2.z * tmp2.w + 3.83;
                    tmp2.w = tmp2.z * tmp2.w + 0.459;
                    tmp2.z = tmp2.z * tmp2.w + -0.00287;
                    tmp2.z = tmp2.z * 1.442695;
                    tmp2.z = exp(tmp2.z);
                    tmp5.xy = tmp1.ww * float2(0.25, 0.2499);
                    tmp1.w = tmp2.z * 0.25 + tmp5.x;
                    tmp2.zw = tmp0.ww * float2(0.5, 20.0);
                    tmp5.xzw = tmp2.zzz * tmp3.xyz;
                    tmp5.xzw = tmp5.xzw * float3(0.5, 0.5, 0.5) + float3(0.0, 1.0001, 0.0);
                    tmp0.w = dot(tmp5.xyz, tmp5.xyz);
                    tmp0.w = sqrt(tmp0.w);
                    tmp0.w = 1.0 - tmp0.w;
                    tmp0.w = tmp0.w * 230.8312;
                    tmp0.w = exp(tmp0.w);
                    tmp1.w = tmp0.w * tmp1.w + -tmp5.y;
                    tmp1.w = max(tmp1.w, 0.0);
                    tmp1.w = min(tmp1.w, 50.0);
                    tmp5.xyz = tmp1.xyz * tmp2.yyy + float3(0.0125664, 0.0125664, 0.0125664);
                    tmp5.xyz = -tmp1.www * tmp5.xyz;
                    tmp5.xyz = tmp5.xyz * float3(1.442695, 1.442695, 1.442695);
                    tmp0.xyz = exp(tmp5.xyz);
                    tmp0.w = tmp2.w * tmp0.w;
                    tmp2.yzw = tmp0.www * tmp0.xyz;
                    tmp1.xyz = tmp1.xyz * tmp2.xxx + float3(0.02, 0.02, 0.02);
                    tmp4.xyz = tmp1.xyz * tmp2.yzw;
                }
                o.texcoord.x = tmp3.y * -50.0;
                tmp1.xyz = _GroundColor * _GroundColor;
                tmp0.xyz = tmp1.xyz * tmp0.xyz + tmp4.xyz;
                o.texcoord1.xyz = tmp0.xyz * _Exposure.xxx;
                tmp0.x = dot(_WorldSpaceLightPos0.xyz, -tmp3.xyz);
                tmp0.x = tmp0.x * tmp0.x;
                tmp0.x = tmp0.x * 0.75 + 0.75;
                tmp0.xyz = tmp0.xxx * tmp4.xyz;
                o.texcoord2.xyz = tmp0.xyz * _Exposure.xxx;
                return o;
			}
			// Keywords: _SUNDISK_NONE
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0.x = saturate(inp.texcoord.x);
                tmp0.yzw = inp.texcoord1.xyz - inp.texcoord2.xyz;
                tmp0.xyz = tmp0.xxx * tmp0.yzw + inp.texcoord2.xyz;
                o.sv_target.xyz = sqrt(tmp0.xyz);
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
}