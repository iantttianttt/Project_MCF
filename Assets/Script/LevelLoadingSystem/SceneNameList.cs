using UnityEngine;
using System.Collections;



public class SceneNameList : ScriptableObject {

	public string initializationScene;
	public SceneNameHolder[] scenes;

	[System.Serializable]
	public class SceneNameHolder{
		public string sceneName;
		public int sceneID;
		public string loadingSceneName;
		public bool isAdditiveLoading;
	}
}

