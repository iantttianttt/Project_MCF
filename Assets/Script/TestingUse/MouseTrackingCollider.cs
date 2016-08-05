using UnityEngine;
using System.Collections;

public class MouseTrackingCollider : MonoBehaviour {


	public Transform playerPos;

	private MainPlayerContoller mainPlayerContoller;


	void Start () {
		mainPlayerContoller = GameObject.Find ("MainPlayer").GetComponent<MainPlayerContoller> ();
		playerPos = mainPlayerContoller.gameObject.transform;
	}
	

	void Update () {
	
	}

	void OnTriggerEnter (Collider target_obj){
		if (target_obj.tag == "Player") {
			mainPlayerContoller.isMouseOnPlayer = true;
		}
	}

	void OnTriggerExit (Collider target_obj){
		if (target_obj.tag == "Player") {
			mainPlayerContoller.isMouseOnPlayer = false;
		}
	}

}
