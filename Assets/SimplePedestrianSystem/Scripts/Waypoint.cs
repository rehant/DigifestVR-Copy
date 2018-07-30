using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSystem{

	public class Waypoint : MonoBehaviour {

		[Tooltip("Pedestrian System Manager Reference")]
		public PedestrianSystemManager manager;

		[Tooltip("Next waypoint Reference. Target will move to this point once hit this waypoint")]
		public Waypoint nextWaypoint;

        // MODIFIED FOR MACGRID
        [Tooltip("Previous waypoint Reference. Targets moving in reverse will move to this point once hit this waypoint")]
        public Waypoint prevWaypoint;

        //Instantiate a pedestrian at this waypoint
        public void Instatiate_Pedestrian(){
		
			if (manager.CanSpawnPedi ()) {
			
				//instantiate pedestrian
				GameObject pedi = (GameObject)Instantiate (manager.Get_Pedestrian (), this.transform.position, this.transform.rotation);

                //assign target to pedestrian. It will moove toward its target // MODIFIED FOR MACGRID
                Pedestrian pediScript = pedi.GetComponent<Pedestrian>();
                pediScript.setBackwards((Random.Range(0f, 100f) >= 50)); //Randomly assign movement direction // MODIFIED FOR MACGRID
                pediScript.target = GetNextWaypoint (pediScript.getBackwards());

				//increment spawned pedestrians
				manager.curPedestiansSpawned++;

			} else {
			
				//Debug.Log ("FULL"); //MODIFIED FOR MACGRID
			}
		}

        //return the transform of next waypoint // MODIFIED FOR MACGRID
        public Transform GetNextWaypoint(bool backwards = false){

            if ((nextWaypoint && !backwards) || !prevWaypoint)
                return this.nextWaypoint.transform;
            else if (prevWaypoint && backwards)
                return this.prevWaypoint.transform;
            else
                return null;
		}
	}
}