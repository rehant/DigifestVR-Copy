﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSystem
{

    public class Pedestrian : MonoBehaviour
    {

        public enum MovementType
        {
            WALK, // will walk
            RUN // will run
        }
        [Tooltip("Movement type of the pedestrian")]
        public MovementType movementType;

        [Tooltip("Target to which player will move and face towards")]
        public Transform target;

        [Tooltip("Pedestrian will walk with this speed")]
        [Header("Values")]
        public float walkSpeed = 0.2f;
        [Tooltip("Pedestrian will run with this speed")]
        public float runSpeed = 0.2f;
        [Tooltip("Pedestrian will rotate with this speed")]
        public float rotationSpeed = 1;

        bool isDestroyed = false;

        //MODIFIED FOR MACGRID
        [SerializeField]
        private bool m_RayHitGround = false; //Raycast the ground?
        [SerializeField]
        private LayerMask m_GroundMask; //What is ground?
        [SerializeField]
        private Transform m_RayPos; //The position to raycast from?
        [SerializeField]
        private Vector3 m_HitOff; //Hit offset; //The offset to go to

        private bool m_ReversePath = false; //Go backwards?

        [SerializeField]
        private bool m_CanGo = false; //Can we go right now?

        [SerializeField]
        private GameObject pedStop; //A pedestrian stopper

        [SerializeField]
        private bool overlapPedestrian = false; //Do we overlap pedestrian?

        [SerializeField]
        private List<Animator> m_Animators = new List<Animator>();
        [SerializeField]
        private List<Animation> m_Animations = new List<Animation>();

        //set animator value of pedestrian according to the state choosen
        void Start()
        {

            if (m_RayPos == null) m_RayPos = transform; //MODIFIED FOR MACGRID

            Animator anim = this.GetComponent<Animator>();

            switch (movementType)
            {

                case MovementType.WALK:

                    anim.SetInteger("MoveState", 1);

                    return;

                case MovementType.RUN:

                    anim.SetInteger("MoveState", 2);

                    return;
            }

        }

        void Update()
        {

            PedestrianMovement();

            // if target is assinged. Rotate toward it accordingly
            if (target)
            {

                Quaternion targetRotation = Quaternion.LookRotation(target.position - this.transform.position, this.transform.up);
                targetRotation.x = 0; targetRotation.z = 0;
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if (pedStop != null && !pedStop.activeInHierarchy)
            {
                pedStop = null; //Done with it for now

                foreach (Animator a in m_Animators)
                {
                    a.enabled = true;
                }

                foreach (Animation a in m_Animations)
                {
                    a.enabled = true;
                }
            }

            if (pedStop != null)
            {
                foreach (Animator a in m_Animators)
                {
                    a.enabled = false;
                }

                foreach (Animation a in m_Animations)
                {
                    a.enabled = false;
                }
            }

        }

        //stay on the ground! MODIFIED FOR MACGRID
        void RayCastGround()
        {
            RaycastHit hit;

            //Debug.DrawRay(m_RayPos.position, Vector3.down * 20f, Color.yellow);

            if (Physics.Raycast(m_RayPos.position, Vector3.down, out hit, 20f, m_GroundMask)) //Should never be more than 20f off the ground!
            {
                //We hit!
                transform.position = new Vector3(transform.position.x, hit.point.y + m_HitOff.y, transform.position.z);
            }
        }

        //movement acfording to movement type
        void PedestrianMovement()
        {
            //MODIFIED FOR MACGRID
            if (pedStop != null && pedStop.activeInHierarchy) return; //Do nothing

            //MODIFIED FOR MACGRID
            if (overlapPedestrian && Random.Range(0.0f, 1000f) > 500f)
            {
                //Stutter, so that the eventually part ways
                if (Random.Range(0f, 10f) < 9f)
                {
                    this.transform.position += this.transform.forward * Time.deltaTime * walkSpeed * 0.5f;
                    if (m_RayHitGround) RayCastGround(); //MODIFIED FOR MACGRID
                    return;
                }
                else
                {
                    //Try a step backwards
                    this.transform.position -= this.transform.forward * Time.deltaTime * walkSpeed * 0.5f;
                    if (m_RayHitGround) RayCastGround(); //MODIFIED FOR MACGRID
                    return;
                }
            }

            switch (movementType)
            {

                case MovementType.WALK:

                    this.transform.position += this.transform.forward * Time.deltaTime * walkSpeed;
                    if (m_RayHitGround) RayCastGround(); //MODIFIED FOR MACGRID
                    return;

                case MovementType.RUN:

                    this.transform.position += this.transform.forward * Time.deltaTime * runSpeed;
                    if (m_RayHitGround) RayCastGround(); //MODIFIED FOR MACGRID
                    return;
            }
        }

        //properly destroy current pedestrian
        public void DestroyPedestrian(PedestrianSystemManager pedestrianSystem)
        {

            if (!isDestroyed)
            {

                isDestroyed = true;

                pedestrianSystem.curPedestiansSpawned--;
                Destroy(this.gameObject);
            }
        }

        //move to next waypoint once hit the target waypoint
        public void OnTriggerEnter(Collider col)
        {

            if (col.CompareTag("Waypoint"))
            {

                if (col.gameObject == target.gameObject)
                    target = col.GetComponent<Waypoint>().GetNextWaypoint(m_ReversePath);// MODIFIED FOR MACGRID
            }

            //MODIFIED FOR MACGRID
            if (col.CompareTag("PedStop") && !m_ReversePath)
            {
                pedStop = col.gameObject;
            }

            if (col.CompareTag("PedStopB") && m_ReversePath)
            {
                pedStop = col.gameObject;
            }

            //MODIFIED FOR MACGRID
            if (col.CompareTag("Pedestrian"))
            {
                overlapPedestrian = true;
            }
        }

        //MODIFIED FOR MACGRID
        public void OnTriggerExit(Collider col)
        {
            if (col.CompareTag("Pedestrian"))
            {
                overlapPedestrian = false; //Done overlapping
            }
        }

        // MODIFIED FOR MACGRID
        public void setBackwards(bool _tf)
        {
            m_ReversePath = _tf;
        }

        public bool getBackwards()
        {
            return m_ReversePath;
        }
    }
}
