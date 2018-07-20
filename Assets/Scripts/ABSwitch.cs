using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABSwitch : MonoBehaviour {

    [SerializeField]
    private bool m_IsA = true; //Are we A?

    [SerializeField]
    private List<GameObject> m_EnableIfA = new List<GameObject>();
    [SerializeField]
    private List<GameObject> m_DisableIfA = new List<GameObject>();

    private bool lastA = false;

    //Do the opposite for B


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire3")) m_IsA = !m_IsA;

		if (m_IsA != lastA)
        {
            if (m_IsA)
            {
                //A side!
                foreach (GameObject obj in m_EnableIfA)
                {
                    obj.SetActive(true);
                }

                foreach (GameObject obj in m_DisableIfA)
                {
                    obj.SetActive(false);
                }
            }
            else
            {
                //B side!
                foreach (GameObject obj in m_EnableIfA)
                {
                    obj.SetActive(false);
                }

                foreach (GameObject obj in m_DisableIfA)
                {
                    obj.SetActive(true);
                }
            }

            lastA = m_IsA;
        }
	}

    public void ToggleA()
    {
        m_IsA = !m_IsA;
    }

    public void SetA()
    {
        m_IsA = true;
    }

    public void SetB()
    {
        m_IsA = false;
    }
}
