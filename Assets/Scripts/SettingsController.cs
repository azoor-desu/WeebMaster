using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {

	public InputField wordsPGame;

	void Awake () {
		
	}

	private void OnEnable() {
		wordsPGame.text = MenuController._singleton.wordsPerGame.ToString();
	}

	#region
	public void WordsPerGame() {
		MenuController._singleton.wordsPerGame = int.Parse(wordsPGame.text);
	}

	public void ResetStats() {
		RenewSavefile("hiragana");
		RenewSavefile("katakana");
		MenuController._singleton.ReloadSaveFiles();
	}
	#endregion

	void RenewSavefile(string fileName) {
		CharGroup newGrp = MenuController._singleton.LoadBlankFile(fileName,(Resources.Load(fileName) as TextAsset).ToString());
		string tosave = JsonUtility.ToJson(newGrp);
		System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".json",tosave);
	}
}