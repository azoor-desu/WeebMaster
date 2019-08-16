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

	//Load both hirag and kata into memory to use during gameplay.
	public CharMemory charMemory;
	public Toggle[] allGrps;
	public List<int> selectedGrps = new List<int>();

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

	public void ButtBoth() {
		ChangeScreen(chooseGrp);
		gameCtrl.currentGameMode = GameController.GameMode.Both;
	}

	public void ButtConfirmGrps() {
		selectedGrps.Clear();
		foreach (Toggle item in allGrps) {
			if (item.isOn) {
				switch (item.name) {
					case "Grp 0":
					selectedGrps.Add(0);
					selectedGrps.Add(1);
					selectedGrps.Add(2);
					selectedGrps.Add(3);
					selectedGrps.Add(4);
					break;
					case "Grp 1":
					selectedGrps.Add(5);
					selectedGrps.Add(6);
					selectedGrps.Add(7);
					selectedGrps.Add(8);
					selectedGrps.Add(9);
					break;
					case "Grp 2":
					selectedGrps.Add(10);
					selectedGrps.Add(11);
					selectedGrps.Add(12);
					selectedGrps.Add(13);
					selectedGrps.Add(14);
					break;
					case "Grp 3":
					selectedGrps.Add(15);
					selectedGrps.Add(16);
					selectedGrps.Add(17);
					selectedGrps.Add(18);
					selectedGrps.Add(19);
					break;
					case "Grp 4":
					selectedGrps.Add(20);
					selectedGrps.Add(21);
					selectedGrps.Add(22);
					selectedGrps.Add(23);
					selectedGrps.Add(24);
					break;
					case "Grp 5":
					selectedGrps.Add(25);
					selectedGrps.Add(26);
					selectedGrps.Add(27);
					selectedGrps.Add(28);
					selectedGrps.Add(29);
					break;
					case "Grp 6":
					selectedGrps.Add(30);
					selectedGrps.Add(31);
					selectedGrps.Add(32);
					selectedGrps.Add(33);
					selectedGrps.Add(34);
					break;
					case "Grp 7":
					selectedGrps.Add(38);
					selectedGrps.Add(39);
					selectedGrps.Add(40);
					selectedGrps.Add(41);
					selectedGrps.Add(42);
					break;
					case "Grp 8":
					selectedGrps.Add(35);
					selectedGrps.Add(36);
					selectedGrps.Add(37);
					selectedGrps.Add(43);
					selectedGrps.Add(44);
					selectedGrps.Add(45);
					break;
					case "Grp 9":
					selectedGrps.Add(46);
					selectedGrps.Add(47);
					selectedGrps.Add(48);
					selectedGrps.Add(49);
					selectedGrps.Add(50);
					break;
					case "Grp 10":
					selectedGrps.Add(51);
					selectedGrps.Add(52);
					selectedGrps.Add(53);
					selectedGrps.Add(54);
					selectedGrps.Add(55);
					break;
					case "Grp 11":
					selectedGrps.Add(56);
					selectedGrps.Add(57);
					selectedGrps.Add(58);
					selectedGrps.Add(59);
					selectedGrps.Add(60);
					break;
					case "Grp 12":
					selectedGrps.Add(61);
					selectedGrps.Add(62);
					selectedGrps.Add(63);
					selectedGrps.Add(64);
					selectedGrps.Add(65);
					break;
					case "Grp 13":
					selectedGrps.Add(66);
					selectedGrps.Add(67);
					selectedGrps.Add(68);
					selectedGrps.Add(69);
					selectedGrps.Add(70);
					break;
				}
			}
		}

	}

	public void ButtBackChoose() {
		ChangeScreen(chooseMenu);
		foreach (Toggle item in allGrps) {
			item.isOn = false;
		}
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