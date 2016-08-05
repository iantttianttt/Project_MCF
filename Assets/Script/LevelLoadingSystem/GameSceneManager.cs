using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameSceneManager : MonoBehaviour {
	public enum Status{
		None,
		Prepare,
		Start,
		Loading,
		Complete
	}

	public static GameSceneManager ins;

	public SceneNameList sceneNames;

	private Status status;
	private AsyncOperation loadOperation;
	private int own;

	public float Progress{
		get{
			if(this.loadOperation == null){
				return 0;
			}else{
				return this.loadOperation.progress;
			}
		}
	}

	public int OwnSecne{
		set{
			this.own  = Mathf.Clamp(value , 0 , this.sceneNames.scenes.Length - 1);
		}
	}

	void Awake(){

		if(ins == null){

			ins = this;
			GameObject.DontDestroyOnLoad(gameObject);

			if(!string.IsNullOrEmpty(this.sceneNames.initializationScene)){
				SceneManager.LoadScene(this.sceneNames.initializationScene);
			}

		}else if(ins != this){

			Destroy(gameObject);
		}
	}

	public void LoadScene(int idx){

		if(this.status != Status.None && this.status != Status.Complete) return;

		if(idx >= this.sceneNames.scenes.Length) return;

		StartCoroutine(this.AsyncLoadScene(this.sceneNames.scenes[idx]));
	}

	public void LoadOwnScene(){

		this.LoadScene(this.own);
	}

	public void StartLoadTargetScene(){

		this.status = Status.Start;
	}

	private IEnumerator AsyncLoadScene(SceneNameList.SceneNameHolder sceneName){
//				Debug.Log("Start");
		yield return StartCoroutine(this.LoadLoadingScene(sceneName));

		yield return StartCoroutine(this.LoadTargetScene(sceneName));
	}

	private IEnumerator LoadLoadingScene(SceneNameList.SceneNameHolder sceneName){

		if(string.IsNullOrEmpty(sceneName.loadingSceneName)) yield break;

		this.status = Status.Prepare;
		Debug.Log(sceneName.loadingSceneName);

		this.loadOperation = sceneName.isAdditiveLoading ? SceneManager.LoadSceneAsync(sceneName.loadingSceneName,LoadSceneMode.Additive) : SceneManager.LoadSceneAsync(sceneName.loadingSceneName,LoadSceneMode.Single);

		yield return this.loadOperation;

		while(this.status == Status.Prepare){

			yield return null;
		}
	}

	private IEnumerator LoadTargetScene(SceneNameList.SceneNameHolder sceneName){

		this.status = Status.Loading;

		if(string.IsNullOrEmpty(sceneName.sceneName)){

			this.status = Status.None;
			yield break;
		}

		this.loadOperation = SceneManager.LoadSceneAsync(sceneName.sceneName,LoadSceneMode.Single);
		yield return this.loadOperation;

		this.status = Status.Complete;
	}
}
