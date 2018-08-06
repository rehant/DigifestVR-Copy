using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnTags : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> m_EnableObjs = new List<GameObject>(); //Objects to enable if conditions met
    [SerializeField]
    private List<GameObject> m_DisableObjs = new List<GameObject>(); //Objects to disable if conditions met

    List<TagObj> detectedObjs = new List<TagObj>(); //The objects we've detected

    [SerializeField]
    private List<string> m_IncludeTags = new List<string>(); //Tags of objects to include
    [SerializeField]
    private List<string> m_ExcludeTags = new List<string>(); //Tags of objects to exclude

    [SerializeField]
    private bool m_TriggerOnce = false; //Trigger once?

    private bool triggered = false; //Are we triggered?
    private bool wasTriggered = false; //Were we triggered?

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        triggered = false;
        
        if (detectedObjs.Count > 0)
        {
            //We have objects and need to check if we can trigger
            foreach (TagObj t in detectedObjs)
            {
                //Check what we got
                if (m_IncludeTags.Count == 0 || m_IncludeTags.Contains(t.tag)) triggered = true; //So far so good!

                if (m_ExcludeTags.Count > 0 && m_ExcludeTags.Contains(t.tag))
                {
                    triggered = false; //Can't trigger now!                
                    break; //Check again next time
                }
            }
        }

        if (triggered)
        {
            //Do our actions!
            Activate();
        }
        else if (wasTriggered)
        {
            //Untrigger
            Deactivate();
        }
    }

    public void Activate()
    {
        //Do what we do
        if (!wasTriggered)
        {
            //Trigger once
            foreach (GameObject obj in m_EnableObjs)
            {
                obj.SetActive(true);
            }

            foreach (GameObject obj in m_DisableObjs)
            {
                obj.SetActive(false);
            }

            wasTriggered = true;
        }
    }

    public void Deactivate()
    {
        //Undo what we do
        foreach (GameObject obj in m_EnableObjs)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in m_DisableObjs)
        {
            obj.SetActive(true);
        }

        if (!m_TriggerOnce) wasTriggered = false;
    }

    public void HandleTriggerEnter(Collider col)
    {
        //Something entered
        TagObj t = new TagObj();
        t.ID = col.gameObject.GetInstanceID();
        t.obj = col.gameObject;
        t.tag = col.gameObject.tag;

        //We keep this object!
        bool alreadyHave = false; //Do we already have it?

        foreach (TagObj other in detectedObjs)
        {
            if (other.ID == t.ID)
            {
                alreadyHave = true;
                break;
            }
        }

        if (!alreadyHave) detectedObjs.Add(t);
    }

    public void HandleTriggerExit(Collider col)
    {
        //Something exited
        TagObj t = new TagObj();
        t.ID = col.gameObject.GetInstanceID();
        t.obj = col.gameObject;
        t.tag = col.gameObject.tag;

        //We ditch this object!

        for (int i = detectedObjs.Count - 1; i >= 0; i--)
        {
            if (detectedObjs[i].ID == t.ID)
            {
                detectedObjs.RemoveAt(i); //Remove it
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleTriggerExit(other);
    }
}

[System.Serializable]
public class TagObj
{
    public int ID;
    public GameObject obj;
    public string tag;
}
