using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;


public class CreateSceneManagerHolder : MonoBehaviour{

	[MenuItem("Custom Editor/Create Scene List")]
	static void CreateSceneList(){

		//資料 Asset 路徑
		string holderAssetPath = "Assets/Settings/";

		if(!Directory.Exists(holderAssetPath)) Directory.CreateDirectory(holderAssetPath);

		//建立實體
		SceneNameList holder = ScriptableObject.CreateInstance<SceneNameList> ();

		//使用 holder 建立名為 dataHolder.asset 的資源
		AssetDatabase.CreateAsset(holder, holderAssetPath + "SceneList.asset");
	}
}
