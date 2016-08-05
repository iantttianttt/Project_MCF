using UnityEngine;
using System.Collections;

public class MouseTracking : MonoBehaviour {


	public GameObject mainCamera;
	public float distance;

	private Ray r;
	private Vector3 targetPos;

	void Start () {
	
	}
	

	void Update () {

		#if !UNITY_EDITOR && UNITY_ANDROID

		Input.multiTouchEnabled = true;
		if(Input.touchCount > 0){
			r = mainCamera.GetComponent<Camera> ().ScreenPointToRay(Input.touches[Input.touchCount -1].position);
		}else{
			Debug.Log("No touches");
		}


		#else
		r = mainCamera.GetComponent<Camera> ().ScreenPointToRay(Input.mousePosition);
		#endif


		targetPos = r.GetPoint (distance);
		this.transform.position = targetPos;

		Debug.DrawRay (r.origin, r.direction *distance , Color.red);

	}
}
