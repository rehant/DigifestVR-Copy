using UnityEngine;
using System;
using System.Collections;

[Serializable]
public enum CType 
{
	NormalCar,
	SportCar,
	Truck,
	Police,
	Custom
}

public enum WDriveType
{
	RearWheelDrive,
	FrontWheelDrive,
	Wheeled8x8,
	AllWheelDrive
}

public enum Rays
{
	No,
	Yes
}

public class ISMART : MonoBehaviour
{
	[Header("Car Mode")]
	public CType CarType;
	public WDriveType driveType;
	public Rays rayMod;
	public LayerMask HitLayer = -1;

	public bool Restart = false;
	public int StackTime = 1000;
	private int stack = 0;
	private float AngleStack;

	[Header("Car Settings")]
	public AnimationCurve power = AnimationCurve.Linear(0.0f, 5000.0f, 8000f, 0.0f);
	[Range(100f, 5000f)]
	public float torque = 300f;
	private float tmptorque = 0;
	[Range(5f, 100f)]
	public float maxSpeed = 100f;
	[Range(20f, 65f)]
	public float maxAngle = 45f;
	[Range(1000f, 10000f)]
	public float brakeTorque = 5000f;
	[Range(0.1f, 1f)]
	public float SmoothSteerAngle = 0.3F;
	[Range(5f, 25f)]
	public float RaySize = 10F;
	[Range(10f, 75f)]
	public float RayAngle = 35F;
	public GameObject BrakeLights ;
	public WheelCollider[] m_Wheels;
	public GameObject[] m_WheelMeshes;

	public GameObject wheelShape;
	private Rigidbody rb;
	public AudioClip EngineAudio;
	private AudioSource audioS;

	[Header("Car Info")]
	[Range(-10f, 175f)]
	public float Speed = 0F;
	[Range(-90f, 90f)]
	public float angle = 0F;
//	[Range(1000f, 3000f)]
	private float Rpm = 0;
	[Range(0f, 10000f)]
	public float MotorRPM;
	private float criticalSpeed = 5f;
	private int stepsBelow = 5;
	private int stepsAbove = 1;
	private bool Brake ;
	private bool Slow ;

	[Header("Waypoints Setting")]
	public bool FindWaypoint ;
	public WayRoad Waypoints;
	public int currentWaypoint = 0;
	private int currentWaypoint2 = 0;
	public bool Parking ;
	public bool ParkingOnEnd ;
	private float alt = 0;

	[Header("Police Addons")]
	[SerializeField]
	public PoliceAddons policeAddons;
	[System.Serializable]
	public class PoliceAddons {
		public int ChaseRange = 25;
		public GameObject PoliceLights ;
		public GameObject Target ; 
		[HideInInspector]
		public GameObject Target2 ; 
		public float EnemyRange ;
		[HideInInspector]
		public GameObject[] Enemys ;
		[HideInInspector]
		public GameObject[] Enemys2 ;
		public bool Chase ;
	}

	[Header("Police Addons")]


	[Header("Debug")]
	private int i= 0;
	private bool RayC ;
	private bool RayLF ;
	private bool RayLN ;
	private bool RayRN ;
	private bool RayRF ;



	void Start()
	{
		m_Wheels = GetComponentsInChildren<WheelCollider>();
		rb = GetComponent<Rigidbody>();


		if(EngineAudio)
		{
		    if(!audioS)
		    	{
			    audioS = this.gameObject.AddComponent<AudioSource>();
				audioS.enabled = false;
			    }
		       else
		     	{
				audioS = GetComponent<AudioSource>();
		        }
			audioS.clip = EngineAudio;
			audioS.loop = true;
			audioS.volume = 0;
			audioS.enabled = true;
			audioS.spatialBlend = 1;
	    }

		if(Waypoints == null && FindWaypoint == true)
			Waypoints = FindObjectOfType(typeof(WayRoad)) as WayRoad;


		for (int i = 0; i < m_Wheels.Length; ++i) 
		{
			var wheel = m_Wheels [i];

			if (wheelShape != null)
			{
				var ws = Instantiate (wheelShape);
				ws.transform.parent = wheel.transform;

			}
			else
			{	
				var ws = Instantiate (Resources.Load("FamilyCarTyre") as GameObject);
				wheelShape = ws;
			    ws.transform.parent = wheel.transform;

			}
		}

		if (CarType == CType.NormalCar)
		{
			driveType = WDriveType.FrontWheelDrive;
			torque = 350f;
			maxSpeed = 40f;
			brakeTorque = 5000f;
			rayMod = Rays.No;
		}
		if (CarType == CType.Police)
		{
			driveType = WDriveType.FrontWheelDrive;
			torque = 350f;
			maxSpeed = 40f;
			brakeTorque = 5000f;
			rayMod = Rays.No;
			if(policeAddons.PoliceLights !=null)
				policeAddons.PoliceLights.SetActive (false);

		}
		if (CarType == CType.SportCar)
		{
			driveType = WDriveType.RearWheelDrive;
			torque = 1000f;
			maxSpeed = 135f;
			brakeTorque = 10000f;
			rayMod = Rays.Yes;
		}
		if (CarType == CType.Truck)
		{

			driveType = WDriveType.AllWheelDrive;
			torque = 500f;
			maxSpeed = 40f;
			brakeTorque = 7500f;
			rayMod = Rays.No;
		}
		tmptorque = torque;

		FindClosestWay ();
	}


	void  FixedUpdate (){

		Navigation();

		if(audioS)
		{	
		audioS.volume = Speed * 0.01f;
		float pitchRpm = (MotorRPM/10000);
		audioS.pitch = pitchRpm;

			if ( audioS.pitch > 2f )
				audioS.pitch = 2f;

			if ( pitchRpm < 1f )
				audioS.pitch = 1f;

			
		}



	if (CarType == CType.Police)
		{
			
		FindClosestEnemy ();

			if(policeAddons.EnemyRange > policeAddons.ChaseRange && Parking == false)
			{
				policeAddons.Chase = false;
				torque = 350f;
				tmptorque = torque;
				maxSpeed = 40f;
				brakeTorque = 5000f;
				rayMod = Rays.No;

				if(policeAddons.PoliceLights !=null)
					policeAddons.PoliceLights.SetActive (false);

			}
			if(policeAddons.EnemyRange < policeAddons.ChaseRange )
			{
				policeAddons.Chase = true;
				torque = 1000f;
				tmptorque = torque;
				maxSpeed = 125f;
				brakeTorque = 10000f;
				rayMod = Rays.Yes;
				Parking = false;

				if(policeAddons.PoliceLights !=null)
					policeAddons.PoliceLights.SetActive (true);

			}

	    }
	}

	void Update()
	{
		m_Wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);
		rb = GetComponent<Rigidbody>();
		rb.drag = rb.velocity.magnitude / 200;
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
		//rb.AddForce(-transform.up * rb.velocity.magnitude);
		Speed = (rb.velocity.magnitude ) * 3.6f;
		var ray = transform.TransformDirection (Vector3.forward) * RaySize ;
		var rayL = transform.TransformDirection (Vector3.forward) * RaySize * Speed ;
		float F = (Speed * 0.01f + 0.5f);
		var rayF = transform.TransformDirection (Vector3.forward) * RaySize * F;

		MotorRPM = power.Evaluate(torque) * Speed *10;

		foreach (WheelCollider wheel in m_Wheels)
		{

		if (wheel.transform.localPosition.z > 0)
		{
				wheel.steerAngle = angle;
				Rpm = m_Wheels [0].rpm * 3.14f;

			if (driveType == WDriveType.Wheeled8x8)
			{
					m_Wheels [0].steerAngle = angle;
					m_Wheels [1].steerAngle = angle;
					m_Wheels [2].steerAngle = angle;
					m_Wheels [3].steerAngle = angle;
			} 
		}




			for (int i = 0; i < m_Wheels.Length; ++i) 
			{


			if (wheelShape) 
			{
				Quaternion q;
				Vector3 p;
				m_Wheels[i].GetWorldPose (out p, out q);

				Transform shapeTransform = m_Wheels[i].transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;

				//shapeTransform.gameObject.transform.GetChild (0).rotation = q;

				}
			}


			if (Parking == false && Brake != true)
			{

			if (driveType == WDriveType.FrontWheelDrive)
			{
				m_Wheels [0].motorTorque = torque;
				m_Wheels [1].motorTorque = torque;

			}
			if (driveType == WDriveType.RearWheelDrive)
			{
				m_Wheels [2].motorTorque = torque;
				m_Wheels [3].motorTorque = torque;
			}
			if (driveType == WDriveType.AllWheelDrive)
			{
				m_Wheels [0].motorTorque = torque;
				m_Wheels [1].motorTorque = torque;
				m_Wheels [2].motorTorque = torque;
				m_Wheels [3].motorTorque = torque;

			}
				if (driveType == WDriveType.Wheeled8x8)
				{
				m_Wheels [0].motorTorque = torque;
				m_Wheels [1].motorTorque = torque;
				m_Wheels [2].motorTorque = torque;
				m_Wheels [3].motorTorque = torque;

				}


		}

		if (Slow == true)
			{
				wheel.brakeTorque = brakeTorque;

			}
			else
			{
				wheel.brakeTorque = 0;	
				Slow = false;
			}


		if (Parking == true)
			{

				torque = 0 ;
				wheel.brakeTorque = brakeTorque;	
				wheel.steerAngle = 0;
				Brake = true;
			}
		else
			{
				torque = tmptorque ;

				if (BrakeLights != null)
					BrakeLights.SetActive (false);


				Parking = false;
				wheel.brakeTorque = 0;	
		}


		if (Brake == true)
			{
				if (BrakeLights != null)
					BrakeLights.SetActive (true);
				
				wheel.brakeTorque = brakeTorque;	
			}

		if (Brake == false)
			{
				torque = tmptorque ;

				if (BrakeLights != null)
					BrakeLights.SetActive (false);

				Brake = false;
				wheel.brakeTorque = 0;	
		}
				

			RaycastHit hit;
			Ray downRay = new Ray(transform.position, Vector3.up);

			Debug.DrawRay (transform.position, rayF, Color.yellow);	
			if (Physics.Raycast (transform.position, rayF, RaySize*F  ,HitLayer)) 
			{
				Debug.DrawRay (transform.position, rayF, Color.red);
				wheel.brakeTorque = brakeTorque;
				Brake = true;
				stack++;
				RayC = true;

				//if (stack >= 200 && rayMod == Rays.Yes )
				if (stack >= StackTime )
				{
				torque = -1500 ;
				wheel.brakeTorque = 0;	
				Brake = false;

				if (stack >= StackTime + 500)
				  {
						torque = tmptorque ;
						stack =  0;
			        }
		    	}
			}
			else
			{
				Brake = false;
				RayC = false;

			}

			if(Restart == true && stack > StackTime)
			{
				transform.Rotate(Vector3.up, AngleStack * Time.deltaTime);
			    stack = 0;
			}


			if (rayMod == Rays.Yes )
			{
			Debug.DrawRay (transform.position, Quaternion.AngleAxis(RayAngle/2, transform.up) * ray , Color.white);     
				if (Physics.Raycast (transform.position, Quaternion.AngleAxis(RayAngle/2, transform.up) * ray , RaySize ,HitLayer))
			{
					Debug.DrawRay (transform.position, Quaternion.AngleAxis(RayAngle/2, transform.up) * ray , Color.red);
			
					m_Wheels [0].steerAngle = -(7 * (SmoothSteerAngle * Speed/2));
					m_Wheels [1].steerAngle = -(7 * (SmoothSteerAngle * Speed/2));

					if (driveType == WDriveType.Wheeled8x8)
					{
						m_Wheels [0].steerAngle = -(7 * (SmoothSteerAngle * Speed/2));
						m_Wheels [1].steerAngle = -(7 * (SmoothSteerAngle * Speed/2));
						m_Wheels [2].steerAngle = -(7 * (SmoothSteerAngle * Speed/2));
						m_Wheels [3].steerAngle = -(7 * (SmoothSteerAngle * Speed/2));
					} 

					RayRN = true;

			} 
				else
					RayRN = false;

				Debug.DrawRay (transform.position, Quaternion.AngleAxis(RayAngle, transform.up) * ray , Color.white);     
				if (Physics.Raycast (transform.position, Quaternion.AngleAxis(RayAngle, transform.up) * ray , RaySize ,HitLayer))
			{
					Debug.DrawRay (transform.position, Quaternion.AngleAxis(RayAngle, transform.up) * ray , Color.red);

					m_Wheels [0].steerAngle = -(12 * (SmoothSteerAngle * Speed/2));
					m_Wheels [1].steerAngle = -(12 * (SmoothSteerAngle * Speed/2));

					if (driveType == WDriveType.Wheeled8x8)
					{
						m_Wheels [0].steerAngle = -(12 * (SmoothSteerAngle * Speed/2));
						m_Wheels [1].steerAngle = -(12 * (SmoothSteerAngle * Speed/2));
						m_Wheels [2].steerAngle = -(12 * (SmoothSteerAngle * Speed/2));
						m_Wheels [3].steerAngle = -(12 * (SmoothSteerAngle * Speed/2));
					}

					RayRF = true;

			}    
				else
					RayRF = false;

				Debug.DrawRay (transform.position, Quaternion.AngleAxis(-RayAngle/2, transform.up) * ray , Color.white);     
				if (Physics.Raycast (transform.position, Quaternion.AngleAxis(-RayAngle/2, transform.up) * ray , RaySize,HitLayer))
			{
					Debug.DrawRay (transform.position, Quaternion.AngleAxis(-RayAngle/2, transform.up) * ray , Color.red);

					m_Wheels [0].steerAngle = (7 * (SmoothSteerAngle * Speed/2));
					m_Wheels [1].steerAngle = (7 * (SmoothSteerAngle * Speed/2));

					if (driveType == WDriveType.Wheeled8x8)
					{
						m_Wheels [0].steerAngle = (7 * (SmoothSteerAngle * Speed/2));
						m_Wheels [1].steerAngle = (7 * (SmoothSteerAngle * Speed/2));
						m_Wheels [2].steerAngle = (7 * (SmoothSteerAngle * Speed/2));
						m_Wheels [3].steerAngle = (7 * (SmoothSteerAngle * Speed/2));
					}
					RayLN = true;

			} 
				else
					RayLN = false;

				Debug.DrawRay (transform.position, Quaternion.AngleAxis(-RayAngle, transform.up) * ray , Color.white);     
				if (Physics.Raycast (transform.position, Quaternion.AngleAxis(-RayAngle, transform.up) * ray , RaySize ,HitLayer))
			{
					Debug.DrawRay (transform.position, Quaternion.AngleAxis(-RayAngle, transform.up) * ray , Color.red);

					m_Wheels [0].steerAngle = (12 * (SmoothSteerAngle * Speed/2));
					m_Wheels [1].steerAngle = (12 * (SmoothSteerAngle * Speed/2));

					if (driveType == WDriveType.Wheeled8x8)
					{
						m_Wheels [0].steerAngle = (12 * (SmoothSteerAngle * Speed/2));
						m_Wheels [1].steerAngle = (12 * (SmoothSteerAngle * Speed/2));
						m_Wheels [2].steerAngle = (12 * (SmoothSteerAngle * Speed/2));
						m_Wheels [3].steerAngle = (12 * (SmoothSteerAngle * Speed/2));
					}


					RayLF = true;

			} 
				else
					RayLF = false;
		}
	}     
}

	void Navigation (){

		if (Waypoints != null ) {
			
			
			if (currentWaypoint >= Waypoints.waypoints.Length) {
				currentWaypoint = 0;
			}

			if (currentWaypoint == Waypoints.waypoints.Length-1 && ParkingOnEnd == true) {
				Parking = true;
			}

			if (policeAddons.Chase == false) {	
				Vector3 nextWaypointPosition = transform.InverseTransformPoint (new Vector3 (Waypoints.waypoints [currentWaypoint].position.x, transform.position.y, Waypoints.waypoints [currentWaypoint].position.z));

				if (nextWaypointPosition.x < maxAngle || nextWaypointPosition.x > -maxAngle)
					angle = nextWaypointPosition.x;

				if (angle > maxAngle)
					angle = maxAngle;
	
		

				if (angle < -maxAngle)
					angle = -maxAngle;





				if (nextWaypointPosition.magnitude <  Speed / 4 )
				{
					currentWaypoint++;
				    Slow = true;
				} 

				FindClosestWay ();
			} 
			else
			{
				Vector3 nextWaypointPosition = transform.InverseTransformPoint (new Vector3 (policeAddons.Target.transform.position.x, transform.position.y, policeAddons.Target.transform.position.z));
				angle = nextWaypointPosition.x;


				policeAddons.Enemys2 = GameObject.FindGameObjectsWithTag ("Way");
				GameObject closest2;
				var distance2 = Mathf.Infinity;
				var position = transform.position;

				foreach (GameObject go in policeAddons.Enemys2) {
					var diff = (go.transform.position - position);
					var curDistance2 = diff.sqrMagnitude;
					if (curDistance2 < distance2 && alt > 0) {
						closest2 = go;
						policeAddons.Target2 = go; 
						distance2 = curDistance2;
						int index = closest2.transform.GetSiblingIndex ();

						if(currentWaypoint2 <= index)
						currentWaypoint2 = index;
						currentWaypoint = currentWaypoint2;
					}
				}


			}

		} else
			Parking = true;
}

	void FindClosestEnemy (){

		    
	     	policeAddons.Enemys = GameObject.FindGameObjectsWithTag("Enemy");
			GameObject closest ;
			var distance = Mathf.Infinity;
			var position = transform.position;
		      foreach(GameObject go in policeAddons.Enemys)  {
				var diff = (go.transform.position - position);
				var curDistance = diff.sqrMagnitude;
				if (curDistance < distance) {
					closest = go;
				    policeAddons.Target = go; 
					distance = curDistance;
				}
			}

		policeAddons.EnemyRange = Vector3.Distance(this.transform.position, policeAddons.Target.transform.position);

	}

	void FindClosestWay (){
		
		if(currentWaypoint == currentWaypoint2)
		{
			policeAddons.Enemys2 = GameObject.FindGameObjectsWithTag("Way");
		GameObject closest2 ;
		var distance2 = Mathf.Infinity;
		var position = transform.position;

		foreach(GameObject go in policeAddons.Enemys2)  {
			var diff = (go.transform.position - position);
			var alt = (go.transform.position.y - position.y);

			var curDistance2 = diff.sqrMagnitude;
			if (curDistance2 < distance2 && alt > 0 ) {
				closest2 = go;
				policeAddons.Target2 = go; 
				distance2 = curDistance2;
				int index = closest2.transform.GetSiblingIndex();

				if(currentWaypoint2 <= index)
				currentWaypoint2 = index ;
				currentWaypoint = currentWaypoint2;
			}
         }
      }
   }


}