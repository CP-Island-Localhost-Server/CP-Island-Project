Shader "CpRemix/World/WorldObject Depth" {
	Properties {
		_Diffuse ("Diffuse Texture", 2D) = "" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			ZClip Off
			GpuProgramID 32076
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float3 color : COLOR0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 texcoord3 : TEXCOORD3;
				float3 texcoord4 : TEXCOORD4;
				float2 texcoord5 : TEXCOORD5;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _SurfaceYCoord;
			float _DeepestYCoord;
			float3 _DepthColor;
			float3 _SurfaceReflectionColor;
			float _SurfaceTexTile;
			float _SurfaceMultiplier;
			float _SurfaceVelocityX;
			float _SurfaceVelocityZ;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _Diffuse;
			sampler2D _SurfaceReflectionsRGB;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp0.xyz = unity_ObjectToWorld._m03_m13_m23 * v.vertex.www + tmp0.xyz;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;
                o.color.xyz = v.color.xyz;
                o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                o.texcoord.xy = v.texcoord.xy;
                tmp0.w = tmp0.y - _DeepestYCoord;
                tmp1.x = _SurfaceYCoord - _DeepestYCoord;
                tmp0.w = saturate(tmp0.w / tmp1.x);
                tmp0.w = 1.0 - tmp0.w;
                tmp1.x = 1.0 - tmp0.w;
                o.texcoord3.xyz = _DepthColor * tmp0.www + tmp1.xxx;
                tmp2.x = v.normal.x * unity_WorldToObject._m00;
                tmp2.y = v.normal.x * unity_WorldToObject._m01;
                tmp2.z = v.normal.x * unity_WorldToObject._m02;
                tmp3.x = v.normal.y * unity_WorldToObject._m10;
                tmp3.y = v.normal.y * unity_WorldToObject._m11;
                tmp3.z = v.normal.y * unity_WorldToObject._m12;
                tmp1.yzw = tmp2.xyz + tmp3.xyz;
                tmp2.x = v.normal.z * unity_WorldToObject._m20;
                tmp2.y = v.normal.z * unity_WorldToObject._m21;
                tmp2.z = v.normal.z * unity_WorldToObject._m22;
                tmp1.yzw = tmp1.yzw + tmp2.xyz;
                tmp0.w = dot(tmp1.xyz, tmp1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp0.w = tmp0.w * tmp1.z;
                tmp1.y = tmp0.w > 0.0;
                tmp0.w = tmp0.w * tmp0.w;
                tmp1.y = tmp1.y ? 1.0 : 0.0;
                tmp0.w = tmp0.w * tmp1.y;
                tmp0.y = _SurfaceYCoord - tmp0.y;
                tmp1.y = tmp0.y > 0.0;
                tmp1.y = tmp1.y ? 1.0 : 0.0;
                tmp0.y = tmp0.y * tmp1.y;
                tmp0.y = min(tmp0.y, 1.0);
                tmp0.y = tmp0.y * tmp0.w;
                tmp0.y = tmp1.x * tmp0.y;
                tmp0.y = tmp0.y * _SurfaceMultiplier;
                tmp0.y = tmp0.y * 0.5;
                o.texcoord4.xyz = tmp0.yyy * _SurfaceReflectionColor;
                tmp0.yw = _SurfaceTexTile.xx * float2(_SurfaceVelocityX.x, _SurfaceVelocityZ.x);
                tmp0.yw = tmp0.yw * _Time.xx;
                o.texcoord5.xy = tmp0.xz * _SurfaceTexTile.xx + -tmp0.yw;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, inp.texcoord1.xy);
                tmp0.w = tmp0.w * unity_Lightmap_HDR.x;
                tmp0.xyz = tmp0.xyz * tmp0.www;
                tmp1 = tex2D(_Diffuse, inp.texcoord.xy);
                tmp0.xyz = tmp0.xyz * tmp1.xyz;
                tmp0.xyz = tmp0.xyz * inp.color.xyz;
                tmp1 = tex2D(_SurfaceReflectionsRGB, inp.texcoord5.xy);
                tmp1.xyz = tmp1.xxx * inp.texcoord4.xyz;
                o.sv_target.xyz = tmp0.xyz * inp.texcoord3.xyz + tmp1.xyz;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
}