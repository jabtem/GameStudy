Shader "Custom/CustomLambert" 
{

	Properties
	{
		_R("R", float) = 1
		_G("G", float) = 1
		_B("B", float) = 1
		_MainTex("Albedo (RGB)", 2D) = "white" {}

		_lightStrength("Light Strength", Range(0, 5)) = 1
	}

		SubShader
		{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM

		#pragma surface surf MyLambert
		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;

		fixed4 _Color;
		fixed _R;
		fixed _G;
		fixed _B;

		fixed _lightStrength;

		half4 LightingMyLambert(SurfaceOutput s, half3 lightDir, half atten)
		{
			half lDot = dot(s.Normal, lightDir);
			s.Albedo = s.Albedo * lDot * atten * _LightColor0.rgb * _lightStrength;

			return half4( s.Albedo, 1 );
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{
			_Color = fixed4(_R, _G, _B, 1);
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		}

		ENDCG
	}

	FallBack "Diffuse"
}
