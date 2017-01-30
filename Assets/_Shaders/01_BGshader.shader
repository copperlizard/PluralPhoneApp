Shader "Custom/01_BGshader" {
	Properties {
		_FoamColor ("FoamColor", Color) = (1,1,1,1)
		_WaterColor1("WaterColor1", Color) = (1,1,1,1)
		_WaterColor2("WaterColor2", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM

		#include "UnityCG.cginc"

		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _FoamColor;
		fixed4 _WaterColor1;
		fixed4 _WaterColor2;

		// 2D Random
		float random(in float2 st) 
		{
			return frac(sin(dot(st.xy,	float2(12.9898, 78.233)))	* 43758.5453123);
		}

		// 2D Noise based on Morgan McGuire @morgan3d
		// https://www.shadertoy.com/view/4dS3Wd
		float noise(in float2 st) 
		{
			float2 i = floor(st);
			float2 f = frac(st);

			// Four corners in 2D of a tile
			float a = random(i);
			float b = random(i + float2(1.0, 0.0));
			float c = random(i + float2(0.0, 1.0));
			float d = random(i + float2(1.0, 1.0));

			// Smooth Interpolation

			// Cubic Hermine Curve.  Same as SmoothStep()
			float2 u = f * f * (3.0 - 2.0 * f);
			// u = smoothstep(0.,1.,f);

			// Mix 4 coorners porcentages
			return lerp(a, b, u.x) + (c - a)* u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = _WaterColor1;

			fixed4 sTime = _Time * 0.1;

			//Water accent
			float h = noise(IN.uv_MainTex * (12.0) + fixed2(4.0, sTime.y * 0.01));
			float w = 0.4 * _CosTime.x;
			c += _WaterColor2 * smoothstep(0.47 + w, 0.5 + w, h);

			//Foam
			h = noise((IN.uv_MainTex * 12.0) + _Time.xy * 0.1);
			//w = 0.4 * _CosTime.x;	
			w = 0.4 * cos(sTime.x);
			c += _FoamColor * (smoothstep(0.47 + w, 0.5 + w, h) - smoothstep(0.5 + w, 0.53 + w, h));		
			w = 0.4 * _SinTime.x;
			h = noise(((IN.uv_MainTex + fixed2(16.0, sTime.x) + _Time.xx * 0.01) * 16.0));
			c += _FoamColor * (smoothstep(0.47 + w, 0.5 + w, h) - smoothstep(0.5 + w, 0.53 + w, h));
						
			o.Albedo = c.rgb;
			
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
