using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotTowards : MonoBehaviour {

    [SerializeField]
    private Transform m_Target;

    private Quaternion startRot;

	// Use this for initialization
	void Start () {
        startRot = Quaternion.identity * Quaternion.Inverse(transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		if (m_Target != null)
        {
            transform.LookAt(m_Target);
            transform.rotation *= startRot;
        }
	}
}
