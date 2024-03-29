﻿//
// Unityちゃん用の三人称カメラ
// 
// 2013/06/07 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
	public class ThirdPersonCamera : MonoBehaviour
	{
		public float smooth = 3f;		// カメラモーションのスムーズ化用変数
		Transform standardPos;			// the usual position for the camera, specified by a transform in the game
		Transform frontPos;			    // Front Camera locater

		//[SerializeField]
		//private Vector3 focus = Vector3.zero;

		// スムーズに繋がない時（クイック切り替え）用のブーリアンフラグ
		bool bQuickSwitch = false;	//Change Camera Position Quickly
	
	
		void Start ()
		{
			// 各参照の初期化
			standardPos = GameObject.Find ("CamPos").transform;
		
			if (GameObject.Find ("FrontPos"))
				frontPos = GameObject.Find ("FrontPos").transform;

			//カメラをスタートする
			transform.position = standardPos.position;	
			transform.forward = standardPos.forward;	
		}
	
		void FixedUpdate ()	// このカメラ切り替えはFixedUpdate()内でないと正常に動かない
		{
			mouseEvent ();

			//if (Input.GetButton (KeyCode.LeftControl/*"Fire1"*/)) {	// left Ctlr	
			if (Input.GetKey (KeyCode.LeftControl)) {	// left Ctlr	
					// Change Front Camera
				setCameraPositionFrontView ();
			} else {	
				// return the camera to standard position and direction
				setCameraPositionNormalView ();
			}
		}

		void setCameraPositionNormalView ()
		{
			if (bQuickSwitch == false) {
				// the camera to standard position and direction
				transform.position = Vector3.Lerp (transform.position, standardPos.position, Time.fixedDeltaTime * smooth);	
				transform.forward  = Vector3.Lerp (transform.forward,  standardPos.forward,  Time.fixedDeltaTime * smooth);
			} else {
				// the camera to standard position and direction / Quick Change
				transform.position = standardPos.position;	
				transform.forward = standardPos.forward;
				bQuickSwitch = false;
			}
		}
	
		void setCameraPositionFrontView ()
		{
			// Change Front Camera
			bQuickSwitch = true;
			transform.position = frontPos.position;	
			transform.forward = frontPos.forward;
		}

		void mouseEvent()
		{
			float delta = Input.GetAxis("Mouse ScrollWheel");
			if (delta != 0.0f) {
				Vector3 post = standardPos.forward * delta * 2.0f;
				if (post.magnitude > 0.01) standardPos.position += post;
			}
	
			return;
		}
	}
}