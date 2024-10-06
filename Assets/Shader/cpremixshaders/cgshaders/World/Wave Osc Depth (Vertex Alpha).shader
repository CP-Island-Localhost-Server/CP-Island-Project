Shader "CpRemix/World/Wave Osc Depth (Vertex Alpha)"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OscDir ("World Osc Dir", Vector) = (1,0,0,1)
        _OscAxis ("World Osc Axs (w = wave freq)", Vector) = (0,1,0,1)
        _OscSpeed ("Osc Speed", Float) = 1
        _DepthMultiply ("DepthMultiply", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            Tags { "RenderType" = "Opaque" }
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
                float2 texcoord4 : TEXCOORD4;
                float3 texcoord2 : TEXCOORD2;
                float3 texcoord3 : TEXCOORD3;
                float2 texcoord1 : TEXCOORD1;
                UNITY_FOG_COORDS(5)
            };

            struct fout
            {
                float4 sv_target : SV_Target0;
            };

            float4 _MainTex_ST;
            float3 _OscDir;
            float4 _OscAxis;
            float _OscSpeed;
            float _DepthMultiply;
            float _SurfaceYCoord;
            float _DeepestYCoord;
            float3 _DepthColor;
            float3 _SurfaceReflectionColor;
            float _DynSurfaceTexTile;
            float _DynSurfaceMultiplier;
            float _SurfaceVelocityX;
            float _SurfaceVelocityZ;

            sampler2D _MainTex;
            sampler2D _SurfaceReflectionsRGB;

            // Simplified Vertex Shader
            v2f vert(appdata_full v)
            {
                v2f o;

                // Oscillation axis transformation and sine wave calculation
                float3 worldOscAxis = mul((float3x3)unity_WorldToObject, _OscAxis.xyz);
                float oscFactor = dot(v.vertex.xyz, worldOscAxis) * _OscAxis.w;
                oscFactor = sin(_Time.y * _OscSpeed + oscFactor);

                // Apply scaling and oscillation direction
                float oscAmount = (1.0 - v.color.w) * oscFactor;
                float3 worldOscDir = mul((float3x3)unity_WorldToObject, _OscDir);
                float3 oscillatedVertex = v.vertex.xyz + oscAmount * worldOscDir;

                // Transform to clip space
                o.position = UnityObjectToClipPos(float4(oscillatedVertex, 1.0));

                // Dynamic surface texture coordinates
                float2 surfaceMovement = _DynSurfaceTexTile.xx * float2(_SurfaceVelocityX, _SurfaceVelocityZ) * _Time.xx;
                float3 transformedVertex = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
                o.texcoord4.xy = transformedVertex.xz * _DynSurfaceTexTile.xx - surfaceMovement;

                // Main texture coordinates
                o.texcoord.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                // Depth factor for coloring
                float depthFactor = saturate((transformedVertex.y - _DeepestYCoord) / (_SurfaceYCoord - _DeepestYCoord));
                depthFactor = 1.0 - depthFactor;
                float depthMultiplier = depthFactor * _DepthMultiply;
                o.texcoord2.xyz = _DepthColor * depthMultiplier + (1.0 - depthMultiplier);

                // Surface reflection calculation
                float surfaceYDiff = max(0.0, _SurfaceYCoord - transformedVertex.y);
                float reflectionFactor = min(surfaceYDiff, 1.0) * _DynSurfaceMultiplier * 0.5;
                o.texcoord3.xyz = reflectionFactor * _SurfaceReflectionColor;

                // Lightmap and fog
                o.texcoord1.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                UNITY_TRANSFER_FOG(o, o.position);

                o.color = v.color.xyz;
                return o;
            }

            fout frag(v2f i)
            {
                fout o;

                // Lightmap
                float4 lightmapTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, i.texcoord1.xy);
                lightmapTex.w *= unity_Lightmap_HDR.x;
                lightmapTex.xyz *= lightmapTex.w;

                // Main texture
                float4 mainTex = tex2D(_MainTex, i.texcoord.xy);
                mainTex.xyz *= i.color.xyz;
                lightmapTex.xyz *= mainTex.xyz;

                // Surface reflections
                float4 reflectionTex = tex2D(_SurfaceReflectionsRGB, i.texcoord4.xy);
                reflectionTex.xyz *= i.texcoord3.xyz;

                // Final color output
                o.sv_target.xyz = lightmapTex.xyz * i.texcoord2.xyz + reflectionTex.xyz;
                o.sv_target.w = 1.0;

                UNITY_APPLY_FOG(i.fogCoord, o.sv_target);
                UNITY_OPAQUE_ALPHA(o.sv_target.w);

                return o;
            }

            ENDCG
        }
    }
}
