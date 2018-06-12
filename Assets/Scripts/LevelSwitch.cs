using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSwitch : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    public List<GameObject> MyChildren = new List<GameObject>();

    [SerializeField]
    Dropdown theDropdown;
    
	void Start () {
        if (MyChildren.Count == 0)
        {
            foreach (Transform child in transform)
            {
                MyChildren.Add(child.gameObject);
            }
        }

        NewLevel();
	}
	
	public void NewLevel()
    {
        int index = theDropdown.value;

        for (int i = 0; i < MyChildren.Count; i++)
        {
            if (index == i)
            {
                MyChildren[i].SetActive(true);
            }
            else
            {
                MyChildren[i].SetActive(false);
            }
        }
    }
}
