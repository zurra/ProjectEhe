Shader "PBS Metallic Emission2" { 
	Properties { 
		[Header (Textures and Bumpmaps)]_Albedo("Albedo", 2D) = "white" {}
		[Header (Colors)]_MainColor("MainColor", Color) = (1,1,1,1)
		_GlowColor("GlowColor", Color) = (1,1,1,1)
		[Header (Variables)]_Metallic("Metallic", float) = 1
		_Smoothness("Smoothness", float) = 0
		_Speed("Speed", float) = 50
		_Amplitude("Amplitude", float) = 1
		_GlowAmount("GlowAmount", float) = 1
	}
	SubShader {
		LOD 300
		Cull Off
		ZWrite  On
		ColorMask RGBA
		Offset 0, -200



		CGPROGRAM 
		#pragma surface surf Standard vertex:vert 
		#pragma target 4.0
		#include "UnityCG.cginc"

		sampler2D _Albedo;
		float4 _MainColor;
		float4 _GlowColor;
		float _Metallic;
		float _Smoothness;
		float _Speed;
		float _Amplitude;
		float _GlowAmount;
		float3 _p0_pi0_nc5_o4;
		float _p0_pi0_nc6_o0;
		float _p0_pi0_nc7_o0;
		float _p0_pi0_nc8_o1;
		float _p0_pi0_nc9_o1;
		float3 _p0_pi0_nc11_o1;
		float _p0_pi0_nc14_o0;
		float _p0_pi0_nc15_o2;
		float _p0_pi0_nc16_o0;
		float3 _p0_pi0_nc19_o2;
		float _p0_pi0_nc21_o1;
		float2 _p0_pi0_nc26_o0;
		float _p0_pi0_nc27_o1;
		float _p0_pi0_nc27_o2;
		float _p0_pi0_nc28_o1;
		float3 _p0_pi0_nc29_o1;
		float3 _p0_pi0_nc31_o2;
		float _p0_pi0_nc34_o0;
		float3 _p0_pi0_nc35_o2;
		float3 _p0_pi0_nc36_o2;

		struct appdata{
			float4 vertex    : POSITION;  // The vertex position in model space.
			float3 normal    : NORMAL;    // The vertex normal in model space.
			float4 texcoord  : TEXCOORD0; // The first UV coordinate.
			float4 texcoord1 : TEXCOORD1; // The second UV coordinate.
			float4 texcoord2 : TEXCOORD2; // The third UV coordinate.
			float4 tangent   : TANGENT;   // The tangent vector in Model Space (used for normal mapping).
			float4 color     : COLOR;     // Per-vertex color.
		};

		struct Input{
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			float2 uv_Albedo;
			float4 position : POSITION;
			float4 color : COLOR;

			INTERNAL_DATA
		};

		void vert (inout appdata v, out Input data){
			UNITY_INITIALIZE_OUTPUT(Input,data);
			data.position = mul(UNITY_MATRIX_MVP, v.vertex);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			_p0_pi0_nc5_o4 = tex2D(_Albedo, IN.uv_Albedo).rgb;
			_p0_pi0_nc6_o0 = _Metallic;
			_p0_pi0_nc7_o0 = _Smoothness;
			_p0_pi0_nc8_o1 = clamp(_p0_pi0_nc6_o0, 0, 1);
			_p0_pi0_nc9_o1 = clamp(_p0_pi0_nc7_o0, 0, 1);
			_p0_pi0_nc11_o1 = _GlowColor.rgb;
			_p0_pi0_nc14_o0 = _Time.y;
			_p0_pi0_nc16_o0 = _Speed;
			_p0_pi0_nc15_o2 = (_p0_pi0_nc14_o0 * _p0_pi0_nc16_o0);
			_p0_pi0_nc21_o1 = sin(_p0_pi0_nc15_o2);
			_p0_pi0_nc28_o1 = clamp(_p0_pi0_nc21_o1, 0, 1);
			_p0_pi0_nc26_o0 = float2(0,100);
			_p0_pi0_nc27_o1 = (_p0_pi0_nc26_o0).x;
			_p0_pi0_nc27_o2 = (_p0_pi0_nc26_o0).y;
			_p0_pi0_nc34_o0 = _GlowAmount;
			_p0_pi0_nc29_o1 = _MainColor.rgb;
			_p0_pi0_nc31_o2 = (_p0_pi0_nc5_o4 * _p0_pi0_nc29_o1);
			_p0_pi0_nc35_o2 = _p0_pi0_nc11_o1 * _p0_pi0_nc34_o0;
			_p0_pi0_nc36_o2 = (_p0_pi0_nc5_o4 * _p0_pi0_nc35_o2);
			_p0_pi0_nc19_o2 = _p0_pi0_nc36_o2 * _p0_pi0_nc28_o1;
			o.Albedo = _p0_pi0_nc31_o2*4;
			o.Metallic = _p0_pi0_nc8_o1;
			o.Smoothness = _p0_pi0_nc9_o1;
			o.Emission = _p0_pi0_nc19_o2*IN.color;
		}
		ENDCG

	}
	FallBack "Diffuse"
}
