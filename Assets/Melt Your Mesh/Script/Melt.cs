using UnityEngine;
using System.Collections.Generic;

public class Melt : MonoBehaviour
{
	[Range(0f, 1f)] public float m_MeltDownPercent = 0f;
	public Vector2 m_MeltDownHeight = new Vector2 (0f, 2f);    // x-low y-high
	public Vector2 m_MeltDownFalling = new Vector2 (0.5f, 0f); // same as up
	[Header("Material")]
	[Range(-2f, 2f)] public float m_MeltY = 0f;
	[Range(1f, 4f)] public float m_MeltDistance = 1f;
	[Range(1f, 10f)] public float m_MeltCurve = 2f;
	public Color m_MeltColor = Color.white;
	[Range(0f, 1f)] public float m_MeltGlossiness = 0.5f;
	[Range(0f, 1f)] public float m_MeltMetallic = 0f;
	[Range(1f, 16f)] public float m_MeltWaveScaleX = 4f;
	[Range(1f, 16f)] public float m_MeltWaveScaleZ = 5f;
	[Range(0f, 1f)] public float m_MeltWaveAmplitude = 0.15f;
	[Range(0f, 1f)] public float m_MeltVertical = 0f;
	public bool m_UseTessellation = false;
	private Renderer m_Rd;
	private Shader m_SdrTess;
	private Shader m_SdrNoTess;
	
	void Start ()
	{
		m_Rd = GetComponent<Renderer> ();
		m_SdrTess = Shader.Find ("Melt Your Mesh/Tessellation");
		m_SdrNoTess = Shader.Find ("Melt Your Mesh/Default");
	}
	void Update ()
	{
		Vector3 origp = transform.position;
		float h = Mathf.Lerp (m_MeltDownHeight.x, m_MeltDownHeight.y, m_MeltDownPercent);
		transform.position = new Vector3 (origp.x, h, origp.z);

		m_MeltVertical = Mathf.Lerp (m_MeltDownFalling.x, m_MeltDownFalling.y, m_MeltDownPercent);
		
		Material[] mats = m_Rd.materials;
		for (int i = 0; i < mats.Length; i++)
		{
			if (m_UseTessellation)
				mats[i].shader = m_SdrTess;
			else
				mats[i].shader = m_SdrNoTess;
			mats[i].SetFloat ("_MeltY", m_MeltY);
			mats[i].SetFloat ("_MeltDistance", m_MeltDistance);
			mats[i].SetFloat ("_MeltCurve", m_MeltCurve);
			mats[i].SetColor ("_MeltColor", m_MeltColor);
			mats[i].SetFloat ("_MeltGlossiness", m_MeltGlossiness);
			mats[i].SetFloat ("_MeltMetallic", m_MeltMetallic);
			mats[i].SetFloat ("_MeltWaveScaleX", m_MeltWaveScaleX);
			mats[i].SetFloat ("_MeltWaveScaleZ", m_MeltWaveScaleZ);
			mats[i].SetFloat ("_MeltWaveAmplitude", m_MeltWaveAmplitude);
			mats[i].SetFloat ("_MeltVertical", m_MeltVertical);
		}
	}
}
