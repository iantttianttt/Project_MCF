using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {


	public Transform playerPos;
	public float moveSpeed;
	public float disToPlayer;


	private MainPlayerContoller mainPlayerContoller;

	void Awake () {
		mainPlayerContoller = GameObject.Find ("MainPlayer").GetComponent<MainPlayerContoller> ();
		playerPos = mainPlayerContoller.gameObject.transform;
	}

	void OnEnable(){
		mainPlayerContoller.enemyList.Add (this.gameObject);
	}

	void Start () {

	}
	

	void Update () {
		this.transform.LookAt (playerPos);
		transform.localPosition += transform.forward * moveSpeed * Time.deltaTime;
		disToPlayer = Vector3.Distance (playerPos.position, this.transform.position);

	}

	void OnTriggerEnter (Collider target_obj){
		if (target_obj.tag == "Player") {
//			Debug.Log ("Hit!");
			if (mainPlayerContoller.attackState == MainPlayerContoller.AttackState.Attacking) {
//				Debug.Log ("Enemy Die!");
				DieFunction ();
			} else if (mainPlayerContoller.attackState != MainPlayerContoller.AttackState.Attacking) {
				Debug.Log ("Player Die!");
				DieFunction ();
			} else {
				Debug.Log ("What the fuck?");
			}
		}
	}

	void DieFunction () {
		mainPlayerContoller.targetEnemyPos = null;
		mainPlayerContoller.enemyList.Remove (this.gameObject);
		ObjectPool.Despawn (this.gameObject);
	}
}
