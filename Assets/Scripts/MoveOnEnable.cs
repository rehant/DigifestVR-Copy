using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnEnable : MonoBehaviour {

    [SerializeField]
    private GameObject moveObj;

	// Use this for initialization
	void OnEnable () {
        if (moveObj != null) moveObj.transform.position = transform.position;
	}
}
