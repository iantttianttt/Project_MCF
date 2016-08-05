using UnityEngine;
using System.Collections;

public class EnemyCreater : MonoBehaviour {

	public GameObject enemy;
	public GameObject bullet;
	public bool go;
	public float time;

	private int createArea;
	private Vector3 createPos;

	void Start () {
		ObjectPool.Preload (enemy, 20);
		StartCoroutine (Test ());
	}
	

	void Update () {
		if (Input.GetKeyDown (KeyCode.Q)) {
			CreateEnemy ();
		}
	}


	void CreateEnemy () {
		createArea = Random.Range (1, 4);
		switch (createArea) {
		case 1:
			float yy1 = Random.Range (-1f, 5.5f);
			createPos = new Vector3 (-8.3f, yy1 , 0f);
			break;
		case 2:
			float xx2 = Random.Range (-8.3f, 8.3f);
			createPos = new Vector3 (xx2, 5.5f , 0f);
			break;
		case 3:
			float yy3 = Random.Range (-1f, 5.5f);
			createPos = new Vector3 (8.3f, yy3 , 0f);
			break;
		}
		ObjectPool.Spawn (enemy, createPos, Quaternion.identity);
	}


	IEnumerator Test () {
		while (go == true) {
			yield return new WaitForSeconds (time);
			CreateEnemy ();
		}

		StartCoroutine (Test ());
		yield break;
	}
}
