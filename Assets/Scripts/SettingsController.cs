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
		CharMemory newMem = MenuController._singleton.LoadBlankFile();
		string tosave = JsonUtility.ToJson(newMem);
		System.IO.File.WriteAllText(Application.persistentDataPath + "/saves.json",tosave);

		MenuController._singleton.charMemory = newMem;
	}
	#endregion
}