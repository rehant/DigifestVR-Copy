Shader "Melt Your Mesh/Default" {
	Properties {
		_Color            ("Color", Color) = (1, 1, 1, 1)
		_MainTex          ("Albedo", 2D) = "white" {}
		_Glossiness       ("Smoothness", Range(0, 1)) = 0.5
		_Metallic         ("Metallic", Range(0, 1)) = 0
		_MeltY            ("Melt Y", Float) = 0.0
		_MeltDistance     ("Melt Distance", Float) = 1
		_MeltCurve        ("Melt Curve", Range(1, 10)) = 2
		_MeltColor        ("Melt Color", Color) = (1, 1, 1, 1)
		_MeltGlossiness   ("Melt Smoothness", Range(0, 1)) = 0
		_MeltMetallic     ("Melt Metallic", Range(0, 1)) = 0
		_MeltWaveScaleX   ("Melt Wave Scale X", Float) = 4
		_MeltWaveScaleZ   ("Melt Wave Scale Z", Float) = 5
		_MeltWaveAmplitude("Melt Wave Amplitude", Float) = 0.15
		_MeltVertical     ("Melt Vertical", Float) = 0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }	
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#include "Core.cginc"
		ENDCG
	}
	FallBack "Diffuse"
}
