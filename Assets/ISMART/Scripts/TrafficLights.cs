using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLights : MonoBehaviour {
	
	public GameObject Red;
	public GameObject Yellow;
	public GameObject Green;

	public float RedTime = 10; 
	public float GreenTime = 10; 
	public float DelayTime = 1; 


	void  Start ()
	{
		Red.SetActive (true);
		Yellow.SetActive (false);
		Green.SetActive (false);

	    StartCoroutine(Pos_1());

	} 


	IEnumerator Pos_1()
	{
		Red.SetActive (true);
		yield return new WaitForSeconds(RedTime);
		Red.SetActive (false);
		Yellow.SetActive (true);
		yield return new WaitForSeconds(DelayTime);
		Yellow.SetActive (false);
		Green.SetActive (true);
		yield return new WaitForSeconds(GreenTime);
		Green.SetActive (false);
		yield return new WaitForSeconds(DelayTime);
		Green.SetActive (true);
		yield return new WaitForSeconds(DelayTime);
		Green.SetActive (false);
		yield return new WaitForSeconds(DelayTime);
		Green.SetActive (true);
		yield return new WaitForSeconds(DelayTime);
		Green.SetActive (false);

		StartCoroutine(Pos_1());
	} 


} 
