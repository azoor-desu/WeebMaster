using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

	public RectTransform mainMenu;
	public RectTransform chooseMenu;
	public RectTransform settingsMenu;
	public RectTransform gameScreen;
	public RectTransform chartScreen;

	public static MenuController _singleton;
	GameController gameCtrl;

	public string[] defaultTemplates = new string[] { "hiragana" };

	//Groups of characters/words to load into game memory, identifiable by the string fileName
	public List<CharGroup> charGroups = new List<CharGroup>();

	public int wordsPerGame = 20;

	void Awake() {
		Application.targetFrameRate = 24; //for the best cinematic experience

		if (_singleton == null) {
			 _singleton = this;
		}

		gameCtrl = gameScreen.GetComponent<GameController>();

		ChangeScreen(mainMenu);

		ReloadSaveFiles();
	}

	public void ReloadSaveFiles() {
		charGroups.Clear();
		foreach (string item in defaultTemplates) {
			LoadSaveFile(item);
		}
	}

	//Checks if save file exists. if not, create one fresh file.
	void LoadSaveFile(string fileName) {
		if (!System.IO.File.Exists(Application.persistentDataPath + "/" + fileName + ".json")) {
			charGroups.Add(LoadBlankFile(fileName,(Resources.Load(fileName) as TextAsset).ToString()));
			string tosave = JsonUtility.ToJson(charGroups);
			System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".json",tosave);
		} else {
			charGroups.Add(JsonUtility.FromJson<CharGroup>(System.IO.File.ReadAllText(Application.persistentDataPath + "/" + fileName + ".json")));
		}
	}

	CharGroup LoadBlankFile(string groupName, string rawInput) {
		string[] rawInputArray = rawInput.Split('\n');
		List<string> rawInputList = new List<string>();
		foreach (string item in rawInputArray) {
			if (item.Length > 0 && item != "\r") { // Remove all empty lines
				rawInputList.Add(item.Trim((char)13)); //remove stupid invisible characters
			}
		}

		//Put characters and romanji into corresponding vars.
		CharGroup chrgrp = new CharGroup(groupName,rawInputList.Count);
		for (int i = 0; i < rawInputList.Count; i++) {
			string[] cells = rawInputList[i].Split(';');
			chrgrp.character[i] = cells[0];
			chrgrp.romanji[i] = cells[1];
		}
		
		return chrgrp;
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

		newScreen.gameObject.SetActive(true);
	}

	#region buttons
	public void ButtStart() {
		ChangeScreen(chooseMenu);
		//buttback delegate to set to mainmenu
	}

	public void ButtHirag() {
		ChangeScreen(gameScreen);
		gameCtrl.currentGameMode = GameController.GameMode.Hiragana;
	}

	public void ButtBack() {
		ChangeScreen(mainMenu);
	}
	#endregion

	//help functions
	public CharGroup GetCharGrp(string grpName) {
		for (int i = 0; i < charGroups.Count; i++) {
			if (grpName == charGroups[i].groupName) {
				return charGroups[i];
			}
		}
		Debug.LogError("Character Group of name: " + grpName + " NOT FOUND");
		return null;
	}

}