using UnityEngine;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
	void OnGUI ()
	{
		int w = 250;
		GUI.Box (new Rect (Screen.width / 2 - w / 2, 10, w, 25), "Melt Your Mesh Demo");
	}
}
