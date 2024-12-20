Shader "CpRemix/World/Snow Ramp" {
	Properties {
		_SnowRampTex ("Snow Ramp", 2D) = "white" {}
		[HideInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Tags { "RenderType" = "Opaque" }
			GpuProgramID 34332
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 texcoord3 : TEXCOORD3;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _ShadowPlaneDim;
			float _ShadowTextureDim;
			float3 _ShadowPlaneWorldPos;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _SnowRampTex;
			sampler2D _BlobShadowTex;
			
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
                o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                o.texcoord.xy = v.normal.yy * float2(0.45, 0.45) + float2(0.5, 0.5);
                tmp0.xz = tmp0.xz - _ShadowPlaneWorldPos.xz;
                o.texcoord3.z = tmp0.y;
                tmp0.y = _ShadowPlaneDim * 0.5;
                tmp0.xy = tmp0.xz / tmp0.yy;
                tmp0.z = _ShadowPlaneDim / _ShadowTextureDim;
                tmp0.z = tmp0.z / _ShadowPlaneDim;
                tmp0.xy = tmp0.zz + tmp0.xy;
                tmp0.xy = tmp0.xy + float2(1.0, 1.0);
                o.texcoord3.xy = tmp0.xy * float2(0.5, 0.5);
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = tex2D(_BlobShadowTex, inp.texcoord3.xy);
                tmp0.z = inp.texcoord3.z >= tmp0.y;
                tmp0.z = tmp0.z ? 2.0 : 1.0;
                tmp0.y = tmp0.y - inp.texcoord3.z;
                tmp0.y = abs(tmp0.y) * tmp0.z + tmp0.z;
                tmp0.y = tmp0.y - 0.5;
                tmp0.y = max(tmp0.y, 1.0);
                tmp0.x = tmp0.x * tmp0.y;
                tmp0.x = min(tmp0.x, 1.0);
                tmp1 = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, inp.texcoord1.xy);
                tmp0.y = tmp1.w * unity_Lightmap_HDR.x;
                tmp0.yzw = tmp1.xyz * tmp0.yyy;
                tmp1 = tex2D(_SnowRampTex, inp.texcoord.xy);
                tmp1.xyz = tmp0.yzw * tmp1.xyz;
                o.sv_target = tmp0.xxxx * tmp1;
                return o;
			}
			ENDCG
		}
	}
}