Shader "CpRemix/World/Wave Osc Depth (Vertex Alpha)"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OscDir ("World Osc  Dir", Vector) = (1,0,0,1)
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
                float4 position : SV_POSITION;
                float3 color : COLOR0;
                float2 texcoord : TEXCOORD0;
                float2 texcoord4 : TEXCOORD4;
                float3 texcoord2 : TEXCOORD2;
                float3 texcoord3 : TEXCOORD3;
                float2 texcoord1 : TEXCOORD1;
                UNITY_FOG_COORDS(5)
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
            
            v2f vert(appdata_full v)
            {
                v2f o;

                // Oscillation axis transformation
                float3 worldOscAxis = mul((float3x3)unity_WorldToObject, _OscAxis.xyz);
                float oscFactor = dot(v.vertex.xyz, worldOscAxis) * _OscAxis.w;
                oscFactor = sin(_Time.y * _OscSpeed + oscFactor);

                // Apply scaling and oscillation direction
                float3 worldOscDir = mul((float3x3)unity_WorldToObject, _OscDir);
                float oscAmount = (1.0 - v.color.w) * oscFactor;
                float3 oscillatedVertex = v.vertex.xyz + oscAmount * worldOscDir;

                // Transform the oscillated vertex from object to world space
                float4 worldPos = mul(unity_ObjectToWorld, float4(oscillatedVertex, 1.0));

                // Calculate position in clip space
                o.position = UnityObjectToClipPos(worldPos);

                // Calculate dynamic surface texture coordinates
                float2 surfaceMovement = _DynSurfaceTexTile * float2(_SurfaceVelocityX, _SurfaceVelocityZ) * _Time.x;
                float3 transformedVertex = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
                o.texcoord4 = transformedVertex.xz * _DynSurfaceTexTile - surfaceMovement;

                // Pass main texture coordinates
                o.texcoord = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                // Ensure all texture coordinates are initialized properly
                o.texcoord2 = float3(0, 0, 0); // Initialize texcoord2
                o.texcoord3 = float3(0, 0, 0); // Initialize texcoord3
                o.texcoord1 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                // Calculate depth factor for coloring
                float depthFactor = saturate((transformedVertex.y - _DeepestYCoord) / (_SurfaceYCoord - _DeepestYCoord));
                depthFactor = 1.0 - depthFactor;
                float depthMultiplier = depthFactor * _DepthMultiply;
                o.texcoord2 = _DepthColor * depthMultiplier + (1.0 - depthMultiplier);

                // Surface reflection calculation
                float surfaceYDiff = max(0.0, _SurfaceYCoord - transformedVertex.y);
                float reflectionFactor = min(surfaceYDiff, 1.0) * _DynSurfaceMultiplier * 0.5;
                o.texcoord3 = reflectionFactor * _SurfaceReflectionColor;

                // Normal transformation using world-to-object matrix
                float3 worldNormal = normalize(mul((float3x3)unity_WorldToObject, v.normal));

                // Handle fog
                UNITY_TRANSFER_FOG(o, o.position);

                // Set color
                o.color = v.color.rgb;

                return o;
            }

            struct fout
            {
                float4 sv_target : SV_Target;
            };

            fout frag(v2f inp)
            {
                fout o;
                float4 lightmapTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_Lightmap, unity_Lightmap, inp.texcoord1.xy);
                lightmapTex.w *= unity_Lightmap_HDR.x;
                lightmapTex.rgb *= lightmapTex.a;
                float4 mainTex = tex2D(_MainTex, inp.texcoord);
                mainTex.rgb *= inp.color;

                lightmapTex.rgb *= mainTex.rgb;
                float4 surfaceReflections = tex2D(_SurfaceReflectionsRGB, inp.texcoord4);
                surfaceReflections.rgb *= inp.texcoord3;

                o.sv_target.rgb = lightmapTex.rgb * inp.texcoord2.rgb + surfaceReflections.rgb;
                o.sv_target.a = 1.0;
                UNITY_APPLY_FOG(inp.fogCoord, o.sv_target);
                UNITY_OPAQUE_ALPHA(o.sv_target.a);
                return o;
            }
            ENDCG
        }
    }
}
