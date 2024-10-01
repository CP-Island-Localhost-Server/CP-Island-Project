Shader "Hidden/ClothingOutlinerImageEffect" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineTex ("Outline", 2D) = "white" {}
		_OutlineColor ("Outline Color", Vector) = (1,1,1,1)
		_OutlineLookups ("Outline Lookups", Range(1, 8)) = 3
		_OutlineLookupDistance ("Outline Lookup Distance", Float) = 0.01
	}
	SubShader {
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			GpuProgramID 33675
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				float4 position : SV_POSITION0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			// $Globals ConstantBuffers for Fragment Shader
			float4 _OutlineColor;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			sampler2D _OutlineTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                o.texcoord.xy = v.texcoord.xy;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                tmp1.xy = inp.texcoord.xy > float2(0.125, 0.125);
                tmp1.zw = inp.texcoord.xy < float2(0.875, 0.875);
                tmp1.x = tmp1.z ? tmp1.x : 0.0;
                tmp1.x = tmp1.y ? tmp1.x : 0.0;
                tmp1.x = tmp1.w ? tmp1.x : 0.0;
                if (tmp1.x) {
                    tmp1 = tex2D(_OutlineTex, inp.texcoord.xy);
                    tmp2 = inp.texcoord.xyxy - float4(0.0039063, 0.0039063, 0.0039063, -0.0);
                    tmp3 = tex2D(_OutlineTex, tmp2.xy);
                    tmp3.xyz = tmp3.xyz - tmp1.xyz;
                    tmp1.w = dot(abs(tmp3.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp2 = tex2D(_OutlineTex, tmp2.zw);
                    tmp2.xyz = tmp2.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp2.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp1.w * 0.707 + tmp2.x;
                    tmp2 = inp.texcoord.xyxy + float4(-0.0039063, 0.0039063, 0.0039063, -0.0039063);
                    tmp3 = tex2D(_OutlineTex, tmp2.xy);
                    tmp3.xyz = tmp3.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp3.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.707 + tmp1.w;
                    tmp2 = tex2D(_OutlineTex, tmp2.zw);
                    tmp2.xyz = tmp2.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp2.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.707 + tmp1.w;
                    tmp2 = inp.texcoord.xyxy + float4(0.0039063, 0.0, 0.0039063, 0.0039063);
                    tmp3 = tex2D(_OutlineTex, tmp2.xy);
                    tmp3.xyz = tmp3.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp3.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp1.w + tmp2.x;
                    tmp2 = tex2D(_OutlineTex, tmp2.zw);
                    tmp2.xyz = tmp2.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp2.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.707 + tmp1.w;
                    tmp2 = inp.texcoord.xyxy + float4(0.0, 0.0039063, 0.0, -0.0039063);
                    tmp3 = tex2D(_OutlineTex, tmp2.xy);
                    tmp3.xyz = tmp3.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp3.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp1.w + tmp2.x;
                    tmp2 = tex2D(_OutlineTex, tmp2.zw);
                    tmp2.xyz = tmp2.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp2.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp1.w + tmp2.x;
                    tmp2 = inp.texcoord.xyxy - float4(0.0117188, 0.0117188, 0.0117188, -0.0);
                    tmp3 = tex2D(_OutlineTex, tmp2.xy);
                    tmp3.xyz = tmp3.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp3.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.303 + tmp1.w;
                    tmp2 = tex2D(_OutlineTex, tmp2.zw);
                    tmp2.xyz = tmp2.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp2.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.5 + tmp1.w;
                    tmp2 = inp.texcoord.xyxy + float4(-0.0117188, 0.0117188, 0.0117188, -0.0117188);
                    tmp3 = tex2D(_OutlineTex, tmp2.xy);
                    tmp3.xyz = tmp3.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp3.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.303 + tmp1.w;
                    tmp2 = tex2D(_OutlineTex, tmp2.zw);
                    tmp2.xyz = tmp2.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp2.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.303 + tmp1.w;
                    tmp2 = inp.texcoord.xyxy + float4(0.0117188, 0.0, 0.0117188, 0.0117188);
                    tmp3 = tex2D(_OutlineTex, tmp2.xy);
                    tmp3.xyz = tmp3.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp3.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.5 + tmp1.w;
                    tmp2 = tex2D(_OutlineTex, tmp2.zw);
                    tmp2.xyz = tmp2.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp2.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.303 + tmp1.w;
                    tmp2 = inp.texcoord.xyxy + float4(0.0, 0.0117188, 0.0, -0.0117188);
                    tmp3 = tex2D(_OutlineTex, tmp2.xy);
                    tmp3.xyz = tmp3.xyz - tmp1.xyz;
                    tmp2.x = dot(abs(tmp3.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.w = tmp2.x * 0.5 + tmp1.w;
                    tmp2 = tex2D(_OutlineTex, tmp2.zw);
                    tmp1.xyz = tmp2.xyz - tmp1.xyz;
                    tmp1.x = dot(abs(tmp1.xyz), float3(0.2126, 0.7152, 0.0722));
                    tmp1.x = tmp1.x * 0.5 + tmp1.w;
                    tmp1.x = tmp1.x * 0.1666667;
                } else {
                    tmp1.x = 0.0;
                }
                o.sv_target = _OutlineColor * tmp1.xxxx + tmp0;
                return o;
			}
			ENDCG
		}
	}
}