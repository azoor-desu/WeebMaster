using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	public RectTransform mainMenu;
	public RectTransform chooseMenu;
	public RectTransform chooseGrp;
	public Transform chooseGrpContent;
	public RectTransform settingsMenu;
	public RectTransform gameScreen;
	public RectTransform chartScreen;

	public static MenuController _singleton;
	GameController gameCtrl;

	public CharMemory charMemory;   //Load both hirag and kata into memory to use during gameplay.
	public Toggle[] allGrps; //Toggle buttons
	public List<int> selectedItems = new List<int>(); //index of items that have been selected for gameplay

	public int wordsPerGame = 0;

	void Awake() {
		Application.targetFrameRate = 24; //for the best cinematic experience
		if (_singleton == null) {
			_singleton = this;
		}
		gameCtrl = gameScreen.GetComponent<GameController>();
		ChangeScreen(mainMenu);
		LoadSaveFile();

		allGrps = new Toggle[chooseGrpContent.childCount];
		for (int i = 0; i < chooseGrpContent.childCount; i++) {
			allGrps[i] = chooseGrpContent.GetChild(i).GetComponent<Toggle>();
		}
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

	//Returns a blank state template for writing to disk
	public CharMemory LoadBlankFile() {
		//load in the character list from disk
		string[] charArrayRaw = (Resources.Load("DefaultList") as TextAsset).ToString().Split('\n');

		List<string> rawArray = new List<string>();
		foreach (string item in charArrayRaw) {
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

	public void ButtConfirmGrps() {
		selectedItems.Clear();
		foreach (Toggle item in allGrps) {
			if (item.isOn) {
				switch (item.name) {
					case "Grp 0":
					selectedItems.Add(0);
					selectedItems.Add(1);
					selectedItems.Add(2);
					selectedItems.Add(3);
					selectedItems.Add(4);
					break;
					case "Grp 1":
					selectedItems.Add(5);
					selectedItems.Add(6);
					selectedItems.Add(7);
					selectedItems.Add(8);
					selectedItems.Add(9);
					break;
					case "Grp 2":
					selectedItems.Add(10);
					selectedItems.Add(11);
					selectedItems.Add(12);
					selectedItems.Add(13);
					selectedItems.Add(14);
					break;
					case "Grp 3":
					selectedItems.Add(15);
					selectedItems.Add(16);
					selectedItems.Add(17);
					selectedItems.Add(18);
					selectedItems.Add(19);
					break;
					case "Grp 4":
					selectedItems.Add(20);
					selectedItems.Add(21);
					selectedItems.Add(22);
					selectedItems.Add(23);
					selectedItems.Add(24);
					break;
					case "Grp 5":
					selectedItems.Add(25);
					selectedItems.Add(26);
					selectedItems.Add(27);
					selectedItems.Add(28);
					selectedItems.Add(29);
					break;
					case "Grp 6":
					selectedItems.Add(30);
					selectedItems.Add(31);
					selectedItems.Add(32);
					selectedItems.Add(33);
					selectedItems.Add(34);
					break;
					case "Grp 7":
					selectedItems.Add(38);
					selectedItems.Add(39);
					selectedItems.Add(40);
					selectedItems.Add(41);
					selectedItems.Add(42);
					break;
					case "Grp 8":
					selectedItems.Add(35);
					selectedItems.Add(36);
					selectedItems.Add(37);
					selectedItems.Add(43);
					selectedItems.Add(44);
					selectedItems.Add(45);
					break;
					case "Grp 9":
					selectedItems.Add(46);
					selectedItems.Add(47);
					selectedItems.Add(48);
					selectedItems.Add(49);
					selectedItems.Add(50);
					break;
					case "Grp 10":
					selectedItems.Add(51);
					selectedItems.Add(52);
					selectedItems.Add(53);
					selectedItems.Add(54);
					selectedItems.Add(55);
					break;
					case "Grp 11":
					selectedItems.Add(56);
					selectedItems.Add(57);
					selectedItems.Add(58);
					selectedItems.Add(59);
					selectedItems.Add(60);
					break;
					case "Grp 12":
					selectedItems.Add(61);
					selectedItems.Add(62);
					selectedItems.Add(63);
					selectedItems.Add(64);
					selectedItems.Add(65);
					break;
					case "Grp 13":
					selectedItems.Add(66);
					selectedItems.Add(67);
					selectedItems.Add(68);
					selectedItems.Add(69);
					selectedItems.Add(70);
					break;
				}
			}
		}
		ChangeScreen(gameScreen);
	}

	public void ButtBackChoose() {
		foreach (Toggle item in allGrps) {
			item.isOn = false;
		}
		ChangeScreen(chooseMenu);
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
	public string GetRomanji(int index) {
		return charMemory.romanji[index];
	}
	public string GetHirag(int index) {
		return charMemory.hirag[index].character;
	}
	public int GetHiragAttempts(int index) {
		return charMemory.hirag[index].attempts;
	}
	public int GetHiragCorrect(int index) {
		return charMemory.hirag[index].correct;
	}
	public string GetKata(int index) {
		return charMemory.kata[index].character;
	}
	public int GetKataAttempts(int index) {
		return charMemory.kata[index].attempts;
	}
	public int GetKataCorrect(int index) {
		return charMemory.kata[index].correct;
	}
}