using UnityEngine;
using System.Collections;


public class LoadingScene : MonoBehaviour {

	IEnumerator Start () {

		yield return new WaitForSeconds(3);

		GameSceneManager.ins.StartLoadTargetScene();
	}

}


