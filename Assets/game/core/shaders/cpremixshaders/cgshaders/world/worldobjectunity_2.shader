Shader "CpRemix/World/WorldObject" {
	Properties {
		_Diffuse ("Diffuse Texture", 2D) = "" {}
		[HideInspector] _BlobShadowTex ("Blob Shadow Tex", 2D) = "white" {}
	}
	SubShader {
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			GpuProgramID 22095
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float3 color : COLOR0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 texcoord6 : TEXCOORD6;
				UNITY_FOG_COORDS(2)
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
			sampler2D _Diffuse;
			sampler2D _BlobShadowTex;
			
			// Keywords: 
v2f vert(appdata_full v)
{
    v2f o;

    // Use Unity's built-in function to transform the vertex position from object space to clip space
    o.position = UnityObjectToClipPos(v.vertex);
    
    // Copy the color and texture coordinates directly
    o.color.xyz = v.color.xyz;
    o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.texcoord.xy = v.texcoord.xy;

    // Calculate the world-space position using the built-in function
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

    // Shadow plane-related calculations (unchanged, but streamlined)
    worldPos.xz -= _ShadowPlaneWorldPos.xz;
    o.texcoord6.z = worldPos.y;
    worldPos.y = _ShadowPlaneDim * 0.5;
    worldPos.xy = worldPos.xz / worldPos.yy;
    worldPos.z = _ShadowPlaneDim / _ShadowTextureDim;
    worldPos.z /= _ShadowPlaneDim;
    worldPos.xy += float2(1.0, 1.0) + worldPos.zz;

    // Assign the result to texcoord6.xy
    o.texcoord6.xy = worldPos.xy * 0.5;

    // Handle fog
    UNITY_TRANSFER_FOG(o, o.position);

    return o;
}

			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = tex2D(_BlobShadowTex, inp.texcoord6.xy);
                tmp0.z = inp.texcoord6.z >= tmp0.y;
                tmp0.z = tmp0.z ? 2.0 : 1.0;
                tmp0.y = tmp0.y - inp.texcoord6.z;
                tmp0.y = abs(tmp0.y) * tmp0.z + tmp0.z;
                tmp0.y = tmp0.y - 0.5;
                tmp0.y = max(tmp0.y, 1.0);
                tmp0.x = tmp0.x * tmp0.y;
                tmp0.x = min(tmp0.x, 1.0);
                tmp1 = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, inp.texcoord1.xy);
                tmp0.y = tmp1.w * unity_Lightmap_HDR.x;
                tmp0.yzw = tmp1.xyz * tmp0.yyy;
                tmp1 = tex2D(_Diffuse, inp.texcoord.xy);
                tmp0.yzw = tmp0.yzw * tmp1.xyz;
                tmp0.yzw = tmp0.yzw * inp.color.xyz;
                o.sv_target.xyz = tmp0.xxx * tmp0.yzw;
                o.sv_target.w = 1.0;
				UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
				UNITY_OPAQUE_ALPHA(o.sv_target.w);
                return o;
			}
			ENDCG
		}
	}
}