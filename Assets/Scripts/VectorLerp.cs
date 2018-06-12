using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorLerp : MonoBehaviour {

    [SerializeField]
    private GameObject m_objTrack;

    [SerializeField]
    private float m_LerpSpd = 0.2f;

    private Vector3 startOff = new Vector3(0f, 0f, 0f);

	// Use this for initialization
	void Start () {
        if (m_objTrack != null) startOff = transform.position - m_objTrack.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_objTrack != null)
        {
            transform.position = startOff + Vector3.Lerp(transform.position - startOff, m_objTrack.transform.position, m_LerpSpd * Time.deltaTime);
        }
	}
}
