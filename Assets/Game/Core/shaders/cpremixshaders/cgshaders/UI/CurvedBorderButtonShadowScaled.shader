Shader "CpRemix/UI/CurvedBorderButtonShadowScaled"
{
	Properties
	{
	  [PerRendererData] _MainTex("Sprite Texture", 2D) = "black" {}
	  _Color("Tint", Color) = (1,1,1,1)
	  _Centre("Centre Color", Color) = (0,0.372,0.792,1)
	  _Border("Border Color", Color) = (1,1,1,1)
	  _BorderSize("Border Size", float) = 0.15
	  _AAliasSize("Anti-Aliasing Size", float) = 0.03
	  _Roundness("Roundness", float) = 1
	  _ShadowVec("Shadow Vector", Vector) = (-0.05,0.15,0,0)
	  _ScaleBox("Scale For Shadow", float) = 1.2
	  _ScaleImage("Scale Image", float) = 0.7
	  _ImageAttenuation("Image Attenuation", float) = 1
	  _StencilComp("Stencil Comparison", float) = 8
	  _Stencil("Stencil ID", float) = 0
	  _StencilOp("Stencil Operation", float) = 0
	  _StencilWriteMask("Stencil Write Mask", float) = 255
	  _StencilReadMask("Stencil Read Mask", float) = 255
	  _ColorMask("Color Mask", float) = 15
	}
	SubShader
	{
		Tags
		{
			"PreviewType" = "Plane"
			"QUEUE" = "Transparent"
		}
		Pass
		{
			Tags
			{
				"PreviewType" = "Plane"
				"QUEUE" = "Transparent"
			}
			ZWrite Off
			Cull Off
			Stencil
			{
				Ref[_Stencil]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
				Pass[_StencilOp]
				Comp[_StencilComp]
				Fail Keep
				ZFail Keep
				PassFront Keep
				FailFront Keep
				ZFailFront Keep
				PassBack Keep
				FailBack Keep
				ZFailBack Keep
			}
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Color;
			float _ScaleBox;
			float _ScaleImage;
			float4 _Centre;
			float4 _Border;
			float _AAliasSize;
			float _BorderSize;
			float _Roundness;
			float2 _ShadowVec;
			float _ImageAttenuation;
			sampler2D _MainTex;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float2 texcoord0 : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

				// Scale the image texture coordinates and shadow texture coordinates
				float scaleImageInv = 1.0 / _ScaleImage;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color * _Color;

				// Adjust texture coordinates
				o.texcoord0 = (v.texcoord.xy * scaleImageInv) + (1.0 - scaleImageInv) / 2.0;
				o.texcoord1 = ((v.texcoord.xy * 2.0) - 1.0) * _ScaleBox;

				return o;
			}

			struct FragOutput
			{
				float4 color : SV_Target;
			};

			FragOutput frag(v2f i)
			{
				FragOutput o;

				// Anti-aliasing setup
				float aliasFactorInv = 1.0 / _AAliasSize;
				float aliasRange = 1.0 - _AAliasSize;

				// Calculate border and shadow properties
				float2 texcoordShadow = pow(abs(i.texcoord1), _Roundness);
				float2 texcoordWithShadow = pow(abs(i.texcoord1 + _ShadowVec), _Roundness);

				float borderLimit = pow((1.0 - _BorderSize), _Roundness);
				float borderLimitAA = borderLimit - _AAliasSize;

				// Border and shadow calculations
				float distToBorder = sqrt(dot(texcoordShadow, texcoordShadow));
				float fadeBorder = 1.0 - (clamp(distToBorder, aliasRange, 1.0) - aliasRange) * aliasFactorInv;
				float shadowEdge = 1.0 - (clamp(sqrt(dot(texcoordWithShadow, texcoordWithShadow)), borderLimitAA, borderLimit) - borderLimitAA) * aliasFactorInv;
				float fadeCenter = 1.0 - (clamp(sqrt(dot(texcoordWithShadow, texcoordWithShadow)), aliasRange, 1.0) - aliasRange) * aliasFactorInv;
				float borderOpacity = min(fadeBorder * 1000.0, 1.0);

				// Texture sampling
				float4 sampledImage = tex2D(_MainTex, i.texcoord0);

				// Adjust image attenuation based on texture coordinates
				float2 texDiff = abs((i.texcoord0 - 0.5) * 2.0);
				float texAttenuation = max(texDiff.x, texDiff.y);
				float imageAttenuation = max(sampledImage.w - (float(texAttenuation >= 1.0) * _ImageAttenuation * texAttenuation), 0.0);

				// Combining the border, center, and shadow effect
				float fadeShadow = (shadowEdge < 0.9) ? 0.8 + (shadowEdge * 0.2) : 1.0;
				float innerShadow = (fadeCenter > 0.5) ? fadeCenter * 0.2 : 0.0;

				float4 resultColor = (((_Centre * fadeShadow) * (1.0 - imageAttenuation)) + ((sampledImage * imageAttenuation) * fadeShadow)) * borderOpacity;
				resultColor.xyz = resultColor.xyz + (_Border * (1.0 - fadeShadow) * borderOpacity).xyz;

				resultColor.w = max(fadeBorder, innerShadow);

				o.color = resultColor * i.color;

				return o;
			}

			ENDCG
		}
	}
	FallBack Off
}
