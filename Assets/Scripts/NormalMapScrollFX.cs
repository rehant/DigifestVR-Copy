using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMapScrollFX : MonoBehaviour {

    [SerializeField]
    private Material theMat;

    [SerializeField]
    private string theTex = "_bump";

    [SerializeField]
    private float m_XSpd;
    [SerializeField]
    private float m_YSpd;

    [SerializeField]
    private float m_XOff = 0;
    [SerializeField]
    private float m_YOff = 0;

    [SerializeField]
    private bool m_RandomOffAtStart = true;

	// Use this for initialization
	void Start () {
	    if (theMat != null && m_RandomOffAtStart)
        {
            m_XOff = Random.Range(-1f * m_XOff, 1f * m_XOff);
            m_YOff = Random.Range(-1f * m_YOff, 1f * m_YOff);
            theMat.SetTextureOffset(theTex, new Vector2(m_XOff, m_YOff));
        }	
	}
	
	// Update is called once per frame
	void Update () {
		if (theMat != null)
        {
            m_XOff += m_XSpd * Time.deltaTime;
            m_YOff += m_YSpd * Time.deltaTime;

            theMat.SetTextureOffset(theTex, new Vector2(m_XOff, m_YOff));
        }
	}
}
