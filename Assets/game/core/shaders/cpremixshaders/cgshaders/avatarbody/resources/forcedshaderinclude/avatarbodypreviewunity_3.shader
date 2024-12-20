Shader "CpRemix/Avatar Body Preview" {
	Properties {
		_Diffuse ("Diffuse", 2D) = "black" {}
		_BodyColorsMaskTex ("Body Color Mask", 2D) = "black" {}
		_BodyRedChannelColor ("Body Red Channel Color", Color) = (1,0,0,1)
		_BodyGreenChannelColor ("Body Green Channel Color", Color) = (1,1,0,1)
		_BodyBlueChannelColor ("Body Blue Channel Color", Color) = (1,0,1,1)
		_DetailAndMatcapMaskAndEmissive ("r=detail g=MatCapMask b=emissive", 2D) = "black" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "ALWAYS" }
			ZClip Off
			GpuProgramID 8890
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			// $Globals ConstantBuffers for Fragment Shader
			float3 _BodyRedChannelColor;
			float3 _BodyGreenChannelColor;
			float3 _BodyBlueChannelColor;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _Diffuse;
			sampler2D _BodyColorsMaskTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                o.texcoord.xy = v.texcoord.xy;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = tex2D(_BodyColorsMaskTex, inp.texcoord.xy);
                tmp1.xyz = tmp0.yyy * _BodyGreenChannelColor;
                tmp1.xyz = tmp0.xxx * _BodyRedChannelColor + tmp1.xyz;
                tmp1.xyz = tmp0.zzz * _BodyBlueChannelColor + tmp1.xyz;
                tmp0.x = max(tmp0.y, tmp0.x);
                tmp0.x = max(tmp0.z, tmp0.x);
                tmp0.yzw = tmp0.xxx * tmp1.xyz;
                tmp0.x = 1.0 - tmp0.x;
                tmp1 = tex2D(_Diffuse, inp.texcoord.xy);
                o.sv_target.xyz = tmp1.xyz * tmp0.xxx + tmp0.yzw;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			Blend DstColor SrcColor, DstColor SrcColor
			ZClip Off
			GpuProgramID 121101
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float3 texcoord1 : TEXCOORD1;
				float3 color : COLOR0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _LightColor0;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _DetailAndMatcapMaskAndEmissive;
			
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
                tmp0.xyz = -tmp0.xyz * _WorldSpaceLightPos0.www + _WorldSpaceLightPos0.xyz;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;
                o.texcoord.xy = v.texcoord.xy;
                tmp0.w = dot(tmp0.xyz, tmp0.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp0.xyz = tmp0.www * tmp0.xyz;
                tmp1.x = v.normal.x * unity_WorldToObject._m00;
                tmp1.y = v.normal.x * unity_WorldToObject._m01;
                tmp1.z = v.normal.x * unity_WorldToObject._m02;
                tmp2.x = v.normal.y * unity_WorldToObject._m10;
                tmp2.y = v.normal.y * unity_WorldToObject._m11;
                tmp2.z = v.normal.y * unity_WorldToObject._m12;
                tmp1.xyz = tmp1.xyz + tmp2.xyz;
                tmp2.x = v.normal.z * unity_WorldToObject._m20;
                tmp2.y = v.normal.z * unity_WorldToObject._m21;
                tmp2.z = v.normal.z * unity_WorldToObject._m22;
                tmp1.xyz = tmp1.xyz + tmp2.xyz;
                tmp0.w = dot(tmp1.xyz, tmp1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xyz = tmp0.www * tmp1.xyz;
                tmp0.x = dot(tmp1.xyz, tmp0.xyz);
                tmp0.x = max(tmp0.x, 0.0);
                tmp0.xyz = tmp0.xxx * _LightColor0.xyz;
                tmp1.xyz = glstate_lightmodel_ambient.xyz * float3(0.9, 0.9, 0.9);
                o.texcoord1.xyz = tmp0.xyz * float3(0.75, 0.75, 0.75) + tmp1.xyz;
                o.color.xyz = v.color.xyz;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                tmp0 = tex2D(_DetailAndMatcapMaskAndEmissive, inp.texcoord.xy);
                tmp0.xyz = tmp0.xxx * inp.texcoord1.xyz;
                o.sv_target.xyz = tmp0.xyz * float3(0.47, 0.47, 0.47);
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
}