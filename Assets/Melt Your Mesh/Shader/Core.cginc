#ifndef CORE_INCLUDED
#define CORE_INCLUDED

// uniforms ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
sampler2D _MainTex;
half _Glossiness, _Metallic;
fixed4 _Color;
half _MeltY, _MeltDistance, _MeltCurve, _MeltWaveScaleX, _MeltWaveScaleZ, _MeltWaveAmplitude, _MeltVertical;
float _Tess;
half _MeltGlossiness, _MeltMetallic;
fixed4 _MeltColor;

// struct ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
struct appdata
{
	float4 vertex : POSITION;
	float4 tangent : TANGENT;
	float3 normal : NORMAL;
	float2 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
};
struct Input
{
	float2 uv_MainTex;
	float3 worldPos;
};

// vertex shader ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
float4 calcNewP (float4 objP, float3 objN)
{
	float4 wldP = mul(unity_ObjectToWorld, objP);
	float4 wldN = mul(unity_ObjectToWorld, float4(objN, 0));

	float melt = (wldP.y - _MeltY) / _MeltDistance;
	melt = 1 - saturate(melt);
	melt = pow(melt, _MeltCurve);

	// expand on xz plane
	wldP.xz += wldN.xz * melt;

	// melt down from top
	float dist = distance(float2(0, 0), float2(objP.x, objP.z));
	wldP.y -= _MeltVertical * dist;

	return mul(unity_WorldToObject, wldP);
}
void vert (inout appdata v)
{
	float4 newpos = calcNewP(v.vertex, v.normal);

	float4 bitangent = float4(cross(v.normal, v.tangent), 0);
	float offset = 0.01;

	float4 v1 = calcNewP(v.vertex + v.tangent * offset, v.normal);
	float4 v2 = calcNewP(v.vertex + bitangent * offset, v.normal);

	float4 newtan = v1 - newpos;
	float4 newbin = v2 - newpos;
	v.normal = cross(newtan, newbin);
	v.vertex = newpos;
}

// fragment shader ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
float getMelt (float3 wldpos)
{
	float4 objpos = mul(unity_WorldToObject, float4(wldpos, 0));
	float melt = (wldpos.y - _MeltY) / _MeltDistance;
	melt = 1 - saturate(melt);
	float wave = sin(objpos.x * _MeltWaveScaleX + objpos.z * _MeltWaveScaleZ) * _MeltWaveAmplitude;
	return step(0.5, melt + wave);
}
void surf (Input IN, inout SurfaceOutputStandard o)
{
	float f = getMelt(IN.worldPos);
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = lerp(c.rgb, _MeltColor.rgb, f);
	o.Metallic = lerp(_Metallic, _MeltMetallic, f);
	o.Smoothness = lerp(_Glossiness, _MeltGlossiness, f);
	o.Alpha = c.a;
}
		
#endif