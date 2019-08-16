using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

	public RectTransform mainMenu;
	public RectTransform chooseMenu;
	public RectTransform chooseGrp;
	public RectTransform settingsMenu;
	public RectTransform gameScreen;
	public RectTransform chartScreen;

	public static MenuController _singleton;
	GameController gameCtrl;

	//Load both hirag and kata into memory to use during gameplay.
	public CharMemory charMemory;

	public int wordsPerGame = 0;

	void Awake() {
		Application.targetFrameRate = 24; //for the best cinematic experience

		if (_singleton == null) {
			_singleton = this;
		}

		gameCtrl = gameScreen.GetComponent<GameController>();

		ChangeScreen(mainMenu);

		LoadSaveFile();
	}

	//Checks if save file exists. if not, create one fresh file.
	public void LoadSaveFile() {
		if (!System.IO.File.Exists(Application.persistentDataPath + "/saves.json")) {

			//Create new save file format with katakana + hiragana, then write into the file.
			CharMemory newItem = LoadBlankFile();
			string tosave = JsonUtility.ToJson(newItem);
			System.IO.File.WriteAllText(Application.persistentDataPath + "/saves.json",tosave);

			//also sun bian load into charItems
			charMemory = newItem;

		} else {
			charMemory = JsonUtility.FromJson<CharMemory>(System.IO.File.ReadAllText(Application.persistentDataPath + "/saves.json"));
		}
	}

	public CharMemory LoadBlankFile() {
		//load in the character list from disk
		string[] charArrayRaw = (Resources.Load("DefaultList") as TextAsset).ToString().Split('\n');

		List<string> rawArray = new List<string>();
		foreach (string item in charArrayRaw  ) {
			if (item.Length > 0 && item != "\r") { // Remove all empty lines
				rawArray.Add(item.Trim((char)13)); //remove stupid invisible characters
			}
		}

		//Put characters and romanji into corresponding vars.
		CharMemory chrm = new CharMemory(rawArray.Count);
		for (int i = 0; i < rawArray.Count; i++) {
			string[] cell = rawArray[i].Split(';');
			chrm.hirag[i].character = cell[0];
			chrm.kata[i].character = cell[1];
			chrm.romanji[i] = cell[2];
		}

		return chrm;
	}

	public void WriteSaveFile() {
		string tosave = JsonUtility.ToJson(charMemory);
		System.IO.File.WriteAllText(Application.persistentDataPath + "/saves.json",tosave);
	}

	void ChangeScreen(RectTransform newScreen) {
		if (mainMenu != null) {
			mainMenu.gameObject.SetActive(false);
		}
		if (chooseMenu != null) {
			chooseMenu.gameObject.SetActive(false);
		}
		if (settingsMenu != null) {
			settingsMenu.gameObject.SetActive(false);
		}
		if (gameScreen != null) {
			gameScreen.gameObject.SetActive(false);
		}
		if (chartScreen != null) {
			chartScreen.gameObject.SetActive(false);
		}
		if (chooseGrp != null) {
			chooseGrp.gameObject.SetActive(false);
		}

		newScreen.gameObject.SetActive(true);
	}

	#region buttons
	public void ButtStart() {
		ChangeScreen(chooseMenu);
	}

	public void ButtHirag() {
		ChangeScreen(chooseGrp);
		gameCtrl.currentGameMode = GameController.GameMode.Hiragana;
	}

	public void ButtKata() {
		ChangeScreen(chooseGrp);
		gameCtrl.currentGameMode = GameController.GameMode.Katakana;
	}

	public void ButtBoth() {
		ChangeScreen(chooseGrp);
		gameCtrl.currentGameMode = GameController.GameMode.Both;
	}

	public void ButtBackChoose() {
		ChangeScreen(mainMenu);
	}

	public void ButtSettings() {
		ChangeScreen(settingsMenu);
	}

	public void ButtChart() {
		ChangeScreen(chartScreen);
	}

	public void ButtBack() {
		ChangeScreen(mainMenu);
	}
	#endregion

	//help functions

}