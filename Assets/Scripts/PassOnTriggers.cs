using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassOnTriggers : MonoBehaviour {

    //Pass trigger events to other scripts

    [SerializeField]
    private List<EnableOnTags> m_EnableOnTagObjs = new List<EnableOnTags>();

    private void OnTriggerEnter(Collider other)
    {
        foreach (EnableOnTags et in m_EnableOnTagObjs)
        {
            et.HandleTriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (EnableOnTags et in m_EnableOnTagObjs)
        {
            et.HandleTriggerExit(other);
        }
    }
}
