using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRandomChild : MonoBehaviour {

    [SerializeField]
    private List<GameObject> m_ChildrenToEnableFrom = new List<GameObject>();
    [SerializeField]
    private int m_NumToEnable = 1;

    private int numEnabled = 0;

    private void OnEnable()
    {
        numEnabled = 0;

        foreach (GameObject obj in m_ChildrenToEnableFrom)
        {
            obj.SetActive(false);
        }

        List<int> possibleIndexes = new List<int>();
        
        for (int n = 0; n < m_ChildrenToEnableFrom.Count; n++)
        {
            //List the indexes
            possibleIndexes.Add(n);
        }

        for (int i = 0; i < m_NumToEnable; i++)
        {
            //Keep enabling
            int choice = Random.Range(0, possibleIndexes.Count);

            if (possibleIndexes.Count > 0)
            {
                //Can still pick one!
                m_ChildrenToEnableFrom[possibleIndexes[choice]].SetActive(true);
                possibleIndexes.RemoveAt(choice); //Remove it as a possibility
            }
        }
    }
}
