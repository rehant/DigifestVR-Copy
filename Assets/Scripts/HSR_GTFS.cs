using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;

public class HSR_GTFS : MonoBehaviour 
{
	// URLs
	private const string _serviceAlertsURL = "http://opendata.hamilton.ca/GTFS-RT/GTFS_ServiceAlerts.pb";				// URL for GTFS_ServiceAlerts.pb
	private const string _tripUpdatesURL = "http://opendata.hamilton.ca/GTFS-RT/GTFS_TripUpdates.pb";					// URL for GTFS_TripUpdates.pb
	private const string _vehiclePositionsURL = "http://opendata.hamilton.ca/GTFS-RT/GTFS_VehiclePositions.pb";			// URL for GTFS_VehiclePositions.pb

	// File Names
	private const string _serviceAlerts = "GTFS_ServiceAlerts.pb";
	private const string _tripUpdates = "GTFS_TripUpdates.pb";
	private const string _vehiclePositions = "GTFS_VehiclePositions.pb";

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

		// Get working game data directory to save files to
		string dataFolder = Application.dataPath + @"/";
		Debug.Log (dataFolder);

		// Download data from GTFS directory on opendata.hamilton.ca locally for transit information
		WebClient client = new WebClient ();
		client.DownloadFile (_serviceAlertsURL, dataFolder + _serviceAlerts);
		client.DownloadFile (_tripUpdatesURL, dataFolder + _tripUpdates);
		client.DownloadFile (_vehiclePositionsURL, dataFolder + _vehiclePositions);

		yield return null;
	}

}