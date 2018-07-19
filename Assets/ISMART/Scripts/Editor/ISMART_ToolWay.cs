using UnityEngine;
using UnityEditor;

class ISMART_ToolWay : EditorWindow
{
	int m_AxlesCount = 2;
	bool CloseAfterCreateCar = false;
	Collider m_ObjectCollider;

	[MenuItem ("ISMART/Create Waypoints")]
	public static void  ShowWindow ()
    {
		GetWindow(typeof(ISMART_ToolWay));
	}

	void OnGUI ()
    {


		m_AxlesCount = EditorGUILayout.IntSlider ("Waypoints: ", m_AxlesCount, 2, 30);

		EditorGUILayout.Space();

		if (GUILayout.Button("Create Waypoints")) 
        {
			CreateCar ();
		}
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		CloseAfterCreateCar = EditorGUILayout.Toggle("Close After Create", CloseAfterCreateCar);

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		if (GUILayout.Button("Close")) 
		{
			this.Close();
		}
	}

	void CreateCar()
	{

		var root = new GameObject ("Waypoint");
		root.gameObject.tag = "Way";
		root.transform.localScale = new Vector3(1, 1, 1);


		for (int i = 0; i < m_AxlesCount; ++i) 
		{
			var way1 = new GameObject (string.Format("Way{0}", i));

			//way1.AddComponent<BoxCollider> ();
			//m_ObjectCollider = way1.GetComponent<Collider>();
			//m_ObjectCollider.isTrigger = true;

			way1.transform.parent = root.transform;

			way1.transform.localPosition = new Vector3(i*10,0,0);
		}

		root.AddComponent<WayRoad>();

			if(CloseAfterCreateCar == true)
				this.Close();

	    }


	}
