using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainPlayerContoller1 : MonoBehaviour {

	public enum PlayerState {
		Idle = 0,
		Ultimate,
		SlowMotion,
		Attack,
		AttackDelay,
		DisplacementFix,
		FinalFix,
		Falling
	}

	public enum AttackType {
		None,
		DashAttack,
		SpecialAttack,
		Ultimate
	}
		
	#region Public Variable
	//--------basic Component--------//
	public PlayerState playerState;
	public AttackType attackType;
	//--------Attack Value--------//
	public List<GameObject> enemyList;
	#endregion

	#region Private Variable
	//--------basic Component--------//
	private Animator anim;
	//--------Control Value--------//
	private bool isGrounded;

	//--------Player Motion Value--------//
	public Transform mousePos;
	private Vector3 curMusePos;
	private float disToGround = 0f;
	private float distance;
	private Transform targetEnemyPos;

	//--------Attack Value--------//
	private bool attackLock;
	//Dash Attack
	private bool canDash;
	private bool dashAttackCD;
	private Vector3 dashPos;
	private float dashAttackComboTime;
	//Special Attack
	private bool canSpecialAttack;
	private bool isSpecialAttack;
	private float specialAttackEnergy;
	private float specialAttackRangeCD;
	private float specialAttackComboTime;
	//Ultimate Attack
	private bool canUseUltimate;
	private float ultimateCharge;
	#endregion



//	public Transform basicPos;

//	public float attackTime = 0.3f;
//	public float attackStayTime = 0.3f;
//	public float moveSmoothTime = 0.05f;
//	public float staySmoothTime = 0.05f;
//	public float fallingSpeed = 0.01f;
//	public float groundHigh;
//	public bool isMouseOnPlayer;

//	public GameObject mainCamera;
//	public bool groundCheak;


//	private float newPosX;
//	private float newPosY;
//	private float xVelocity = 0.0f;
//	private float yVelocity = 0.0f;
//	private Ray r;
//	private RaycastHit hit;



	void Awake(){
		anim = GameObject.Find ("MainPlayerAnim").GetComponent<Animator> ();
		mousePos = GameObject.Find ("_MouseTrackingPostion").transform;
	}

	void Start () {
		Initialization ();
	}
		
	void Update () {
		PlayerInput ();
//		TargetMousePointAdjust ();
	}

	void LateUpdate () {
//		PlayerMotion ();
	}




	#region ----------------  Update Fuction  ----------------
	void PlayerInput () {
		#if !UNITY_EDITOR && UNITY_ANDROID

		for(int i = 0; i < Input.touchCount; ++i){
		Touch touch = Input.GetTouch(i);
		if(touch.phase == TouchPhase.Began && (attackState == AttackState.Stay || attackState == AttackState.AttackStay || attackState == AttackState.Falling)){
		r = mainCamera.GetComponent<Camera> ().ScreenPointToRay(Input.touches[Input.touchCount -1].position);
		if(Physics.Raycast(r,out hit,100.0f)){
		if(hit.collider.tag == "PlayerStayRange"){
		EnemyScanner ();
		if (targetEnemyPos == null) {
		return;
		}
		StopCoroutine("AttackEnemy");
		attackType = AttackType.Stay;
		StartCoroutine ("AttackEnemy");
		}else{
		StopCoroutine("AttackEnemy");
		curMusePos = new Vector3 (mousePos.position.x, mousePos.position.y, mousePos.position.z);
		attackType = AttackType.Move;
		StartCoroutine ("AttackEnemy");
		}
		}
		}
		}

		#else
		if (Input.GetKey (KeyCode.Mouse0) && canDash == true) {
			Debug.Log("Go Dash Attack");
			switch(attackType){
			case AttackType.None:
				Debug.Log("Whan Idle");
				attackType = AttackType.DashAttack;
				ReSetPlayerLogic();
				curMusePos = new Vector3 (mousePos.position.x, mousePos.position.y, mousePos.position.z);
				CheckGrounded();
				StartCoroutine ("AttackEnemy");
				break;

			case AttackType.DashAttack:
				Debug.Log("Whan Dash Attack Over");
				attackType = AttackType.DashAttack;
				ReSetPlayerLogic();
				curMusePos = new Vector3 (mousePos.position.x, mousePos.position.y, mousePos.position.z);
				CheckGrounded();
				StartCoroutine ("AttackEnemy");
				break;

			case AttackType.SpecialAttack:
				Debug.Log("Whan Special Attack Over");
				attackType = AttackType.DashAttack;
				ReSetPlayerLogic();
				curMusePos = new Vector3 (mousePos.position.x, mousePos.position.y, mousePos.position.z);
				CheckGrounded();
				StartCoroutine ("AttackEnemy");
				break;

			case AttackType.Ultimate:
				Debug.Log("That not right! WTF Man?");
				break;
			}
		}else{
			SearchDashPosOver();
		}

		if(Input.GetKey(KeyCode.Space) && specialAttackEnergy != 0f && canSpecialAttack == true){
			Debug.Log("Go Special Attack");
			switch(attackType){
			case AttackType.None:
				Debug.Log("Whan Idle");
				attackType = AttackType.SpecialAttack;
				ReSetPlayerLogic();
				CheckGrounded();
				StartCoroutine ("AttackEnemy");
				break;

			case AttackType.DashAttack:
				Debug.Log("Whan Dash Attack Over");
				attackType = AttackType.SpecialAttack;
				ReSetPlayerLogic();
				CheckGrounded();
				StartCoroutine ("AttackEnemy");
				break;

			case AttackType.SpecialAttack:
				Debug.Log("Keep Special Attack");
				isSpecialAttack = true;
				break;

			case AttackType.Ultimate:
				Debug.Log("That not right! WTF Man?");
				break;
			}
		}else{
			isSpecialAttack = false;
		}

		if(Input.GetKeyDown (KeyCode.Mouse1) && canUseUltimate == true){
			Debug.Log("Go Ultimate");
			attackType = AttackType.Ultimate;
			ReSetPlayerLogic();
			CheckGrounded();
			StartCoroutine ("AttackEnemy");
		}

		#endif
	}
		
	void PlayerMotion () {
//		switch (attackType) {
//		case AttackType.Move:
//			switch (attackState) {
//			case AttackState.Attacking:
//				newPosX = Mathf.SmoothDamp (transform.position.x, curMusePos.x, ref xVelocity, moveSmoothTime);
//				newPosY = Mathf.SmoothDamp (transform.position.y, curMusePos.y, ref yVelocity, moveSmoothTime);
//				transform.position = new Vector3 (newPosX, newPosY, transform.position.z);
//				break;
//			case AttackState.AttackStay:
//				basicPos.position = this.transform.position;
//				break;
//			case AttackState.Backing:
//				Debug.Log ("What the fuck?");
//				break;
//			case AttackState.Falling:
//				this.transform.Translate (-Vector3.up * fallingSpeed, Space.Self);
//				break;
//			case AttackState.Stay:
//				basicPos.position = this.transform.position;
//				break;
//			}
//			break;
//		case AttackType.Stay:
//			switch (attackState) {
//			case AttackState.Attacking:
//				if (targetEnemyPos == null) {
//					return;
//				}
//				newPosX = Mathf.SmoothDamp (transform.position.x, targetEnemyPos.position.x, ref xVelocity, staySmoothTime);
//				newPosY = Mathf.SmoothDamp (transform.position.y, targetEnemyPos.position.y, ref yVelocity, staySmoothTime);
//				transform.position = new Vector3 (newPosX, newPosY, transform.position.z);
//				break;
//			case AttackState.Backing:
//				newPosX = Mathf.SmoothDamp (transform.position.x, basicPos.position.x, ref xVelocity, staySmoothTime);
//				newPosY = Mathf.SmoothDamp (transform.position.y, basicPos.position.y, ref yVelocity, staySmoothTime);
//				transform.position = new Vector3 (newPosX, newPosY, transform.position.z);
//				break;
//			case AttackState.Falling:
//				this.transform.Translate (-Vector3.up * fallingSpeed, Space.Self);
//				break;
//			case AttackState.Stay:
//				basicPos.position = this.transform.position;
//				break;
//			}
//			break;
//		}
	}
	#endregion


	#region ----------------  Logic  ----------------

	IEnumerator PlayerAttackLogic () {
		switch (attackType) {
		case AttackType.None:
			Debug.Log ("that not right! WTF man?!");
			break;

		case AttackType.DashAttack:
			switch (isGrounded) {
			case true:
				attackLock = true;

				playerState = PlayerState.SlowMotion;
				StopCoroutine ("SearchDashPos");
				yield return StartCoroutine ("SearchDashPos");

				playerState = PlayerState.Attack;
				while (this.transform.position != dashPos) {
					yield return null;
				}

				playerState = PlayerState.AttackDelay;
				attackLock = false;
				yield return new WaitForSeconds (dashAttackComboTime);

				playerState = PlayerState.DisplacementFix;

				playerState = PlayerState.FinalFix;

				playerState = PlayerState.Idle;
				break;

			case false:
				attackLock = true;

				playerState = PlayerState.SlowMotion;
				StopCoroutine ("SearchDashPos");
				yield return StartCoroutine ("SearchDashPos");

				playerState = PlayerState.Attack;
				while (this.transform.position != dashPos) {
					yield return null;
				}

				playerState = PlayerState.AttackDelay;
				attackLock = false;
				yield return new WaitForSeconds (dashAttackComboTime);

				playerState = PlayerState.DisplacementFix;

				playerState = PlayerState.FinalFix;

				playerState = PlayerState.Falling;
				while (isGrounded == false) {
					CheckGrounded();
				}

				playerState = PlayerState.Idle;
				break;
			}

			break;

		case AttackType.SpecialAttack:
			switch (isGrounded) {
			case true:
				attackLock = true;

				playerState = PlayerState.Attack;
				while (isSpecialAttack == true) {
					SpecialAttackLogic ();
					yield return new WaitForSeconds (specialAttackRangeCD);
				}

				playerState = PlayerState.AttackDelay;
				attackLock = false;
				yield return new WaitForSeconds (specialAttackComboTime);

				playerState = PlayerState.DisplacementFix;

				playerState = PlayerState.FinalFix;

				playerState = PlayerState.Idle;
				break;

			case false:
				attackLock = true;

				playerState = PlayerState.Attack;
				while (isSpecialAttack == true) {
					SpecialAttackLogic ();
					yield return new WaitForSeconds (specialAttackRangeCD);
				}

				playerState = PlayerState.AttackDelay;
				attackLock = false;
				yield return new WaitForSeconds (specialAttackComboTime);

				playerState = PlayerState.DisplacementFix;

				playerState = PlayerState.FinalFix;

				playerState = PlayerState.Falling;
				while (isGrounded == false) {
					CheckGrounded();
				}

				playerState = PlayerState.Idle;
				break;

			}

			break;

		case AttackType.Ultimate:
			switch (isGrounded) {
			case true:
				Debug.Log("Use ground ultimate.");
				break;

			case false:
				Debug.Log("Use sky ultimate.");
				break;
			}

			break;

		}
	}

	IEnumerator SearchDashPos () {
		
	}
		
	void SpecialAttackLogic () {
		
	}

	#endregion


	#region ----------------  Math  ----------------
	void Initialization () {
		playerState = PlayerState.Idle;
		enemyList.Clear ();
		disToGround = GetComponent<Collider>().bounds.extents.y;
	}

	void CheckGrounded () {
		RaycastHit hitG;
		if (Physics.Raycast (this.transform.position, -Vector3.up, out hitG, disToGround + 0.1f)) {
			if (hitG.collider.tag == "Ground") {
				isGrounded = true;
				if (playerState == PlayerState.Falling) {
					playerState = PlayerState.Idle;
				}
			} else {
				isGrounded = false;
			}
		} else {
			isGrounded = false;
		}

	}

	void ReSetPlayerLogic () {
		StopCoroutine("AttackEnemy");
		playerState = PlayerState.Idle;
	}

	void EnemyScanner () {
		distance = Mathf.Infinity;
		foreach(GameObject enemy_Obj in enemyList){
			float dis = (enemy_Obj.transform.position - this.transform.position).sqrMagnitude;
			if (dis < distance) {
				targetEnemyPos = enemy_Obj.transform;
				distance = dis;
			}
		}
		if (targetEnemyPos != null) {
			float aa = Vector3.Distance (targetEnemyPos.position, this.transform.position);
			if(aa > 5){
				targetEnemyPos = null;
			}
		}
	}

	void SearchDashPosOver () {
	}

	//	void TargetMousePointAdjust () {
	//		if (curMusePos.y <= groundHigh) {
	//			isGrounded = true;
	//			groundCheak = false;
	//			curMusePos.y = groundHigh;
	//		} else {
	//			groundCheak = true;
	//		}
	//
	//		if (basicPos.position.y <= groundHigh) {
	//			isGrounded = true;
	//			groundCheak = false;
	//			basicPos.position = new Vector3 (basicPos.position.x, groundHigh, basicPos.position.z);
	//		} else {
	//			groundCheak = true;
	//		}
	//	}
	#endregion

}
