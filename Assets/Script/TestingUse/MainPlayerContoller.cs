using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainPlayerContoller : MonoBehaviour {

	public enum AttackState
	{
		Attacking,
		AttackStay,
		Backing,
		Falling,
		Stay
	}

	public enum AttackType
	{
		Stay,
		Move
	}


	public AttackState attackState;
	public AttackType attackType;
	public Transform targetEnemyPos;
	public Transform basicPos;
	public Transform mousePos;
	public float attackTime = 0.3f;
	public float attackStayTime = 0.3f;
	public float moveSmoothTime = 0.05f;
	public float staySmoothTime = 0.05f;
	public float fallingSpeed = 0.01f;
	public float groundHigh;
	public bool isMouseOnPlayer;
	public List<GameObject> enemyList;
	public GameObject mainCamera;
	public bool groundCheak;
	public bool isGrounded;

	private Animator anim;
	private SpriteRenderer spriteRenderer;
	private float distance;
	private Vector3 curMusePos;
	private float newPosX;
	private float newPosY;
	private float xVelocity = 0.0f;
	private float yVelocity = 0.0f;
	private Ray r;
	private RaycastHit hit;
	private float disToGround = 0f;


	void Awake(){
		mousePos = GameObject.Find ("_MouseTrackingPostion").transform;
		anim = GameObject.Find ("MainPlayerAnim").GetComponent<Animator> ();
		spriteRenderer =  GameObject.Find ("MainPlayerAnim").GetComponent<SpriteRenderer> ();
	}

	void Start () {
		attackState = AttackState.Stay;
		enemyList.Clear ();
		disToGround = GetComponent<Collider>().bounds.extents.y;
		attackState = AttackState.Falling;
		groundCheak = true;
	}


	void Update () {
		CheckGrounded ();
		PlayerInput ();
		TargetMousePointAdjust ();

		if (attackState == AttackState.Stay) {
			anim.SetBool ("IsIdle", true);
		} else {
			anim.SetBool ("IsIdle", false);
		}
	}

	void LateUpdate () {
		AnimFlipAdjust ();
		PlayerMotion ();
	}




	#region Update Fuction

	void AnimFlipAdjust () {

		if (targetEnemyPos == null) {
			if (curMusePos.x > this.transform.position.x) {
				spriteRenderer.flipX = true;
			} else if (curMusePos.x < this.transform.position.x) {
				spriteRenderer.flipX = false;
			}
		} else {
			if (targetEnemyPos.position.x > this.transform.position.x) {
				spriteRenderer.flipX = true;
			} else if (targetEnemyPos.position.x < this.transform.position.x) {
				spriteRenderer.flipX = false;
			}
		}

			
	}

	void TargetMousePointAdjust () {
		if (curMusePos.y <= groundHigh) {
			isGrounded = true;
			groundCheak = false;
			curMusePos.y = groundHigh;
		} else {
			groundCheak = true;
		}

		if (basicPos.position.y <= groundHigh) {
			isGrounded = true;
			groundCheak = false;
			basicPos.position = new Vector3 (basicPos.position.x, groundHigh, basicPos.position.z);
		} else {
			groundCheak = true;
		}
	}

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
		if (Input.GetKeyDown (KeyCode.Mouse0) && (attackState == AttackState.Stay || attackState == AttackState.AttackStay || attackState == AttackState.Falling)) {
			if (isMouseOnPlayer == true) {
				EnemyScanner ();
				if (targetEnemyPos == null) {
					return;
				}
				StopCoroutine("AttackEnemy");
				attackType = AttackType.Stay;
				StartCoroutine ("AttackEnemy");
			} else if (isMouseOnPlayer == false) {
				StopCoroutine("AttackEnemy");
				curMusePos = new Vector3 (mousePos.position.x, mousePos.position.y, mousePos.position.z);
				attackType = AttackType.Move;
				StartCoroutine ("AttackEnemy");
			}

		}
		#endif
	}

	void CheckGrounded () {
		if (groundCheak == false) {
			return;
		}
		RaycastHit hitG;
		if (Physics.Raycast (this.transform.position, -Vector3.up, out hitG, disToGround + 0.1f)) {
			if (hitG.collider.tag == "Ground") {
				isGrounded = true;
				if (attackState == AttackState.Falling) {
					attackState = AttackState.Stay;
				}
			} else {
				isGrounded = false;
			}
		} else {
			isGrounded = false;
		}

	}

	void PlayerMotion () {
		switch (attackType) {
		case AttackType.Move:
			switch (attackState) {
			case AttackState.Attacking:
				newPosX = Mathf.SmoothDamp (transform.position.x, curMusePos.x, ref xVelocity, moveSmoothTime);
				newPosY = Mathf.SmoothDamp (transform.position.y, curMusePos.y, ref yVelocity, moveSmoothTime);
				transform.position = new Vector3 (newPosX, newPosY, transform.position.z);
				break;
			case AttackState.AttackStay:
				basicPos.position = this.transform.position;
				break;
			case AttackState.Backing:
				Debug.Log ("What the fuck?");
				break;
			case AttackState.Falling:
				this.transform.Translate (-Vector3.up * fallingSpeed, Space.Self);
				break;
			case AttackState.Stay:
				basicPos.position = this.transform.position;
				break;
			}
			break;
		case AttackType.Stay:
			switch (attackState) {
			case AttackState.Attacking:
				if (targetEnemyPos == null) {
					return;
				}
				newPosX = Mathf.SmoothDamp (transform.position.x, targetEnemyPos.position.x, ref xVelocity, staySmoothTime);
				newPosY = Mathf.SmoothDamp (transform.position.y, targetEnemyPos.position.y, ref yVelocity, staySmoothTime);
				transform.position = new Vector3 (newPosX, newPosY, transform.position.z);
				break;
			case AttackState.Backing:
				newPosX = Mathf.SmoothDamp (transform.position.x, basicPos.position.x, ref xVelocity, staySmoothTime);
				newPosY = Mathf.SmoothDamp (transform.position.y, basicPos.position.y, ref yVelocity, staySmoothTime);
				transform.position = new Vector3 (newPosX, newPosY, transform.position.z);
				break;
			case AttackState.Falling:
				this.transform.Translate (-Vector3.up * fallingSpeed, Space.Self);
				break;
			case AttackState.Stay:
				basicPos.position = this.transform.position;
				break;
			}
			break;
		}
	}

	#endregion


	#region Logic
	 
	IEnumerator AttackEnemy () {
		switch (attackType) {
		case AttackType.Move:
			attackState = AttackState.Attacking;
			yield return new WaitForSeconds (attackTime);
			attackState = AttackState.AttackStay;
			yield return new WaitForSeconds (attackStayTime);
			if (isGrounded == true) {
				attackState = AttackState.Stay;
				yield break;
			} else if (isGrounded == false) {
				attackState = AttackState.Falling;
			}
			while (isGrounded == false) {
				yield return null;
			}
			break;
		case AttackType.Stay:
			if (isGrounded == true) {
				attackState = AttackState.Attacking;

				while (targetEnemyPos != null) {
					yield return null;
				}
				attackState = AttackState.Backing;

				while (this.transform.position != basicPos.position) {
					yield return null;
				}
				attackState = AttackState.Stay;
				groundCheak = true;
				yield break;
			} else if (isGrounded == false) {
				attackState = AttackState.Attacking;

				while (targetEnemyPos != null) {
					yield return null;
				}
				attackState = AttackState.Backing;

				while (this.transform.position != basicPos.position) {
					yield return null;
				}
				attackState = AttackState.AttackStay;
				yield return new WaitForSeconds (attackStayTime);

				attackState = AttackState.Falling;
				Debug.Log ("A");
				while (isGrounded == false) {
					yield return null;
				}
				yield break;
			}
			break;
		}

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

	#endregion


}
