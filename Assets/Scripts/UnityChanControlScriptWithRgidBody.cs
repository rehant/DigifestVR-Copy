//
// 2014/03/13 N.Kobyasahi
//
// Add Flying by Fumi.Iseki
// 2015/05/01 
// 2015/07/11
//

using UnityEngine;
using System.Collections;

namespace UnityChan
{
	// 必要なコンポーネントの列記
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]
	
	public class UnityChanControlScriptWithRgidBody : MonoBehaviour
	{
        public bool  showInteraction = true;
		public float animSpeed = 1.5f;			// アニメーション再生速度設定
		public float lookSmoother = 3.0f;		// a smoothing setting for camera motion
		public bool  useCurves = true;			// Mecanimでカーブ調整を使うか設定する. このスイッチが入っていないとカーブは使われない
		public float useCurvesHeight = 0.5f;	// カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）
		
		public float flyingSpeed   = 10.0f;		// 飛行速度
		public float runningSpeed  = 5.0f;		// 走る速度
		public float forwardSpeed  = 2.5f;		// 歩く速度
		public float backwardSpeed = 2.0f;		// 後退速度
		public float rotateSpeed   = 0.8f;		// 旋回速度
		public float jumpPower     = 3.0f;		// ジャンプ力
		
		private CapsuleCollider cldr;			// キャラクターコントローラ（カプセルコライダ）の参照
		private Rigidbody rbdy;
		private Vector3 velocity;				// キャラクターコントローラ（カプセルコライダ）の移動量
		private float orgColHight;
		private Vector3 orgVectColCenter;
		private Animator anim;					// キャラにアタッチされるアニメーターへの参照
		private AnimatorStateInfo animState;	// base layerで使われる、アニメーターの現在の状態の参照
		private GameObject cameraObject;		// メインカメラへの参照

		// アニメーター各ステートへの参照
		static int idleState  = Animator.StringToHash ("Base Layer.Idle");
		static int runState   = Animator.StringToHash ("Base Layer.Running");
		static int jumpState  = Animator.StringToHash ("Base Layer.Jump");
		static int restState  = Animator.StringToHash ("Base Layer.Rest");
		//static int walkState  = Animator.StringToHash ("Base Layer.Walking");
		//static int hoverState = Animator.StringToHash ("Base Layer.Hovering");
		//static int upState    = Animator.StringToHash ("Base Layer.Up");
		//static int downState  = Animator.StringToHash ("Base Layer.Down");
		
		private int   flying  = 0;		// 0: Land, 1: Flaying
		private int   forward = 0;		// 0: Stop, 1: forwarding, 2:Running
		private uint  clicktm = 0;

		private bool  uparrowKey = false;
		private bool  homeKey    = false;
		private bool  jumpKey    = false;


		//
		// 初期化
		void Start ()
		{
			anim = GetComponent<Animator> ();			// Animatorコンポーネントを取得する
			cldr = GetComponent<CapsuleCollider> ();	// CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
			rbdy = GetComponent<Rigidbody> ();
			orgColHight = cldr.height;					// CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
			orgVectColCenter = cldr.center;
			cameraObject = GameObject.FindWithTag ("MainCamera");
		}


		void Update()
		{
			if (Input.GetKeyDown (KeyCode.UpArrow)) uparrowKey = true;
			if (Input.GetKeyDown (KeyCode.Home)) homeKey = true;
			if (Input.GetKeyDown (KeyCode.Space)) jumpKey = true;
		}


		//
		// 以下、メイン処理.リジッドボディと絡めるので、FixedUpdate内で処理を行う.
		void FixedUpdate ()
		{
			float h = Input.GetAxis ("Horizontal");
			float v = Input.GetAxis ("Vertical");

			if (flying==0) {
				if (uparrowKey) {
					if (clicktm <= 20) {
						//Debug.Log("Run! "+clicktm);
						forward = 2;
						anim.SetBool ("Running", true);
						anim.SetBool ("Walking", false);
					}
					uparrowKey = false;
					clicktm = 0;
				}
				//
				else {
					if (forward == 0 && v>0.1f) {
						//Debug.Log("Walk!");
						forward = 1;
						anim.SetBool ("Walking", true);
						anim.SetBool ("Running", false);
					}
					//
					else if ((forward == 1 || forward == 2) && v <=0.1f) {
						//Debug.Log("Stop!");
						forward = 0;
						anim.SetBool ("Running", false);
						anim.SetBool ("Walking", false);
					}
				}

				// to Fly
				if (homeKey && forward==0) {
					flying = 1;
					rbdy.useGravity = false;
					rbdy.velocity = new Vector3 (0, 0, 0);
					transform.localPosition += Vector3.up * 0.2f;
					anim.SetBool ("Hovering", true);
					homeKey = false;
				}

				if (Input.GetKey(KeyCode.PageUp)) {
					flying = 1;
					forward = 0;
					rbdy.useGravity = false;
					rbdy.velocity = new Vector3 (0, 0, 0);
					transform.localPosition += Vector3.up * 0.2f;
					anim.SetBool ("Walking", false);
					anim.SetBool ("Running", false);
					anim.SetBool ("Up", true);
					anim.SetBool ("Hovering", true);
					v = 0.0f;
				}
				//
				if (jumpKey) {		
					if (forward==2) {
						if (!anim.IsInTransition (0)) {
							rbdy.AddForce (Vector3.up * jumpPower, ForceMode.VelocityChange);
							anim.SetBool ("Jump", true);
						}
					}
					else if (forward==0) {
						if (!anim.IsInTransition (0)) {
							anim.SetBool ("Rest", true);
						}
					}
					jumpKey = false;
				}
			}
			//
			else if (flying==1) {
				anim.SetBool ("Up", false);
				anim.SetBool ("Down", false);
				//
				if (homeKey && forward==0) {
					flying = 0;
					anim.SetBool ("Hovering", false);
					rbdy.useGravity = true;
					homeKey = false;
					v = 0.0f;
				}
				//
				if (Input.GetKey (KeyCode.PageUp)) {
					transform.localPosition += Vector3.up * Time.fixedDeltaTime * 3.5f;
					anim.SetBool ("Up", true);
					v = 0.0f;
				}
				//
				else if (Input.GetKey (KeyCode.PageDown)) {
					anim.SetBool ("Down", true);
					transform.localPosition += Vector3.down * Time.fixedDeltaTime*3.5f;
					v = 0.0f;
				}
			}

			//
			// 以下、Animatorの各ステート中での処理
			animState = anim.GetCurrentAnimatorStateInfo (0);
			// Run中の処理
			if (animState.fullPathHash == runState) {
				if (useCurves) {
					resetCollider ();
				}
			}
			// Jump中の処理
			else if (animState.fullPathHash == jumpState) {
				if (!anim.IsInTransition (0)) {
					anim.SetBool ("Jump", false);
				}
			}
			// Idle中の処理
			else if (animState.fullPathHash == idleState) {
				if (useCurves) {
					resetCollider ();
				}
			}
			// Rest中の処理
			else if (animState.fullPathHash == restState) {
				cameraObject.SendMessage("setCameraPositionFrontView");
				if (!anim.IsInTransition (0)) {
					anim.SetBool ("Rest", false);
					v = 0.0f;
					h = 0.0f;
				}
			}

			//
			// 移動処理
			anim.SetFloat ("Speed", v);							// Animator側で設定している"Speed"パラメタにvを渡す
			anim.SetFloat ("Direction", h); 					// Animator側で設定している"Direction"パラメタにhを渡す
			anim.speed = animSpeed;								// Animatorのモーション再生速度に animSpeedを設定する
			
			velocity = transform.TransformDirection (new Vector3 (0, 0, v));
			if (v > 0.1) {
				if (flying==1) {
					velocity *= flyingSpeed;
				}
				else if (forward == 1) {
					velocity *= forwardSpeed;
				}
				else if (forward == 2) {
					velocity *= runningSpeed;
				}
			}
			else if (v < -0.1) {
				velocity *= backwardSpeed;
			}
			//
			transform.localPosition += velocity * Time.fixedDeltaTime;	// 上下のキー入力でキャラクターを移動させる
			transform.Rotate (0, h * rotateSpeed, 0);					// 左右のキー入力でキャラクタをY軸で旋回させる

			//
			uparrowKey = false;
			homeKey = false;
			jumpKey = false;
			clicktm++;
		}


		//
		// メニュー
		void OnGUI ()
		{
            Rect rect = new Rect(10, Screen.height - 60, 400, 30);
            showInteraction = GUI.Toggle(rect, showInteraction, "Show Interaction");

            if (showInteraction)
            {
                GUI.Box(new Rect(Screen.width - 260, 10, 250, 165), "Interaction");
                GUI.Label(new Rect(Screen.width - 245, 30, 250, 30), "Up Arrow : Walk");
                GUI.Label(new Rect(Screen.width - 245, 50, 250, 30), "Up Arrow x2 : Run");
                GUI.Label(new Rect(Screen.width - 245, 70, 250, 30), "Home : Fly / Land");
                GUI.Label(new Rect(Screen.width - 245, 90, 250, 30), "PageUp / PageDown : Up / Down");
                GUI.Label(new Rect(Screen.width - 245, 110, 250, 30), "Left Control : Front Camera");
                GUI.Label(new Rect(Screen.width - 245, 130, 250, 30), "Mouse Wheel : Zoom");
                GUI.Label(new Rect(Screen.width - 245, 150, 250, 30), "Space Bar : Jump (Run) / Rest (Idle)");
            }
		}


		//
		// キャラクターのコライダーサイズのリセット関数
		void resetCollider ()
		{
			cldr.height = orgColHight;
			cldr.center = orgVectColCenter;
		}	
	}
}
