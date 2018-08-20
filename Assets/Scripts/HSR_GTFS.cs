using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HSR_GTFS : MonoBehaviour 
{
	// URLs
	private const string GTFSHamiltonURL = "http://opendata.hamilton.ca/GTFS-RT/";
	private const string ServiceAlertsURL = "http://opendata.hamilton.ca/GTFS-RT/GTFS_ServiceAlerts.pb";				// URL for GTFS_ServiceAlerts.pb
	private const string TripUpdatesURL = "http://opendata.hamilton.ca/GTFS-RT/GTFS_TripUpdates.pb";					// URL for GTFS_TripUpdates.pb
	private const string VehiclePositionsURL = "http://opendata.hamilton.ca/GTFS-RT/GTFS_VehiclePositions.pb";			// URL for GTFS_VehiclePositions.pb

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
		UnityWebRequest www1 = new UnityWebRequest (GTFSHamiltonURL);
		www1.downloadHandler = new DownloadHandlerBuffer ();
		yield return www1.Send ();

		if (www1.isError) {
			Debug.Log (www1.error);
		} else {
			Debug.Log (www1.downloadHandler.text);
			byte[] GTFSHamilton = www1.downloadHandler.data;
		}

	}

}