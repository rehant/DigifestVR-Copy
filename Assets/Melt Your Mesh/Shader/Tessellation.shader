Shader "Melt Your Mesh/Tessellation" {
	Properties {
		_Color            ("Color", Color) = (1, 1, 1, 1)
		_MainTex          ("Albedo", 2D) = "white" {}
		_Glossiness       ("Smoothness", Range(0, 1)) = 0.5
		_Metallic         ("Metallic", Range(0, 1)) = 0
		_MeltY            ("Melt Y", Float) = 0.0
		_MeltDistance     ("Melt Distance", Float) = 1
		_MeltCurve        ("Melt Curve", Range(1, 10)) = 2
		_Tess             ("Melt Tess Amount", Range(1, 32)) = 10
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
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow tessellate:tessDistance
		#pragma target 4.6   // use tessellate will trigger unity auto compiled into shader model 4.6
		#include "Tessellation.cginc"
		#include "Core.cginc"

		// tessellation shader ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        float MeltCalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess)
		{
			float3 wpos = mul(unity_ObjectToWorld,vertex).xyz;
			float dist = distance (wpos, _WorldSpaceCameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0);

			float melt = (( wpos.y - _MeltY ) / _MeltDistance);
			if( melt < -0.1 || melt > 1.1 )
			{
				f = 0.01;
			}
			return f  * tess;
		}
		float4 MeltDistanceBasedTess (float4 v0, float4 v1, float4 v2, float minDist, float maxDist, float tess)
		{
			float3 f;
			f.x = MeltCalcDistanceTessFactor(v0, minDist, maxDist, tess);
			f.y = MeltCalcDistanceTessFactor(v1, minDist, maxDist, tess);
			f.z = MeltCalcDistanceTessFactor(v2, minDist, maxDist, tess);
			return UnityCalcTriEdgeTessFactors(f);
		}
		float4 tessDistance (appdata v0, appdata v1, appdata v2)
        {
            float minDist = 10.0;
            float maxDist = 25.0;
            return MeltDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
        }

		ENDCG
	}
	FallBack "Diffuse"
}
