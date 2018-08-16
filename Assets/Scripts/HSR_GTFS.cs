using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HSR_GTFS : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		StartCoroutine (GetData ());
	}
		
	// Update is called once per frame
	void Update () 
	{
		
	}

	IEnumerator GetData()
	{
		// Get data from GTFS directory on opendata.hamilton.ca for transit information
		using (UnityWebRequest www1 = UnityWebRequest.Get ("http://opendata.hamilton.ca/GTFS-RT/GTFS_ServiceAlerts.pb"))
		{

			// Currently debugging why bytes aren't being downloaded, as of now string test is 0 bytes long!

			yield return www1.Send ();

			byte[] ServiceAlerts = www1.downloadHandler.data;

			string test = www1.downloadHandler.text;
			Debug.Log (test.Length);
			/*
			if (www1.isError) {
				Debug.Log (www1.error);
			} else {
				byte[] ServiceAlerts = www1.downloadHandler.data;
				if (www1.isDone) {Debug.Log ("ServiceAlerts has finished downloading."); Debug.Log (www1.downloadHandler.text);}
			}
			*/			
		}
				
		using (UnityWebRequest www2 = UnityWebRequest.Get("http://opendata.hamilton.ca/GTFS-RT/GTFS_TripUpdates.pb"))
		{
			yield return www2.Send ();

			if (www2.isError) {
				Debug.Log (www2.error);
			} else {
				byte[] TripUpdates = www2.downloadHandler.data;
				if (www2.isDone) {Debug.Log ("TripUpdates has finished downloading."); Debug.Log (www2.downloadHandler.text);}
			}			
		}

		using (UnityWebRequest www3 = UnityWebRequest.Get("http://opendata.hamilton.ca/GTFS-RT/GTFS_VehiclePositions.pb"))
		{
			yield return www3.Send ();

			if (www3.isError) {
				Debug.Log (www3.error);
			} else {
				byte[] VehiclePositions = www3.downloadHandler.data;
				if (www3.isDone) {Debug.Log ("VehiclePositions has finished downloading."); Debug.Log (www3.downloadHandler.text);}
			}			
		}


	}

}