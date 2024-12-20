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
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR0;
				float4 position : SV_POSITION0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float _RotationSpeed;
			float _PivotX;
			float _PivotY;
			// $Globals ConstantBuffers for Fragment Shader
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _MainTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0.x = _RotationSpeed * _Time.y;
                tmp0.x = sin(tmp0.x);
                tmp1.x = cos(tmp0.x);
                tmp2.x = -tmp0.x;
                tmp0.yz = v.texcoord.xy + -float2(_PivotX.x, _PivotY.x);
                tmp2.y = tmp1.x;
                tmp2.z = tmp0.x;
                tmp1.y = dot(tmp2.xy, tmp0.xy);
                tmp1.x = dot(tmp2.xy, tmp0.xy);
                o.texcoord.xy = tmp1.xy + float2(_PivotX.x, _PivotY.x);
                o.color = v.color;
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
                tmp0 = tex2D(_MainTex, inp.texcoord.xy);
                o.sv_target.xyz = tmp0.xyz * inp.color.xyz;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
	Fallback "Mobile/Diffuse"
}