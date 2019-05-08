using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public enum GameMode { Hiragana, Katakana, Both }
	[HideInInspector] public GameMode currentGameMode;

	bool gameRunning;

	public Text scoreText;
	public Text charText;
	public Text answerText;
	public Text answerRomanjiText;
	public InputField inputField;

	public float fieldReselectDelay = 0.1f;
	Coroutine coroutineSubmit;
	Coroutine coroutineGame;

	//Array of probability to select each character. Higher is more likely.
	float[] hiragArray;
	float[] kataArray;

	int charIndex = 0;
	int score = 0;
	float timeStart;

	void Awake() {

	}

	void StartGame() {
		gameRunning = true;
		StartCoroutine(ScoreUpdate());
		PopulateRandomList();
		charIndex = 0;
		score = 0;
		timeStart = Time.time;
		if (coroutineGame != null) {
			StopCoroutine(coroutineGame);
		}
		coroutineGame = StartCoroutine(GameSeq());
	}

	void PopulateRandomList() {
		CharGroup hiragGrp = MenuController._singleton.hiragGroup;
		CharGroup kataGrp = MenuController._singleton.kataGroup;
		hiragArray = new float[hiragArray.Length];
		kataArray = new float[kataArray.Length];

		//populate list with the latest data
		for (int i = 0; i < hiragGrp.character.Length; i++) {
			//Get probability by totalAttempts / correct attempts. 100% correct = 1, 50% correct = 2 etc.
			if (hiragGrp.timesCorrect[i] == 0) { //prevent divide by zero error. needs a sufficiently large number.
				hiragArray[i] = 10;
			} else {
				hiragArray[i] = hiragGrp.timesAttempted[i] / hiragGrp.timesCorrect[i];
			}
		}

		for (int i = 0; i < kataGrp.character.Length; i++) {
			//Get probability by totalAttempts / correct attempts. 100% correct = 1, 50% correct = 2 etc.
			if (kataGrp.timesCorrect[i] == 0) { //prevent divide by zero error. needs a sufficiently large number.
				kataArray[i] = 10;
			} else {
				kataArray[i] = kataGrp.timesAttempted[i] / kataGrp.timesCorrect[i];
			}
		}
	}

	void DisplayScoreText() {
		float timer = Time.time - timeStart;
		int minutes = Mathf.FloorToInt(timer / 60F);
		int seconds = Mathf.FloorToInt(timer - minutes * 60);
		string niceTime = string.Format("{0:0}:{1:00}",minutes,seconds);
		int totalqns = MenuController._singleton.wordsPerGame;
		scoreText.text = "Word: " + charIndex + "/" + totalqns + "\nScore: " + score + "/" + totalqns + "\nTime: " + niceTime;
	}

	void SaveProgress() {
		print("Savegame");
	}

	public void SubmitAnswer() {
		if (coroutineSubmit == null && inputField.text != "") {
			print("ANSWER SUBMITTED");
			inputField.text = "";
			coroutineSubmit = StartCoroutine(SubmitAnswerCoroutine());
		}
	}

	IEnumerator SubmitAnswerCoroutine() {
		yield return new WaitForSecondsRealtime(fieldReselectDelay);
		inputField.ActivateInputField();
		inputField.Select();
		coroutineSubmit = null;
	}

	IEnumerator ScoreUpdate() {
		DisplayScoreText();
		while (gameRunning) {
			yield return new WaitForSeconds(.5f);
			DisplayScoreText();
		}
	}

	string submittedAns;
	IEnumerator GameSeq() {
		submittedAns = "";
		charIndex = 0;
		while (gameRunning) {
			//Pull a random character.
			//If in BOTH mode, pull hiragana on evens, and kat on odds.

			yield return new WaitUntil(() => submittedAns != "");

			yield return null;
		}
	}

	//int PullRandomIndex(bool hirag) {

	//}

	private void OnApplicationFocus(bool focus) {
		if (!focus) {
			SaveProgress();
		}
	}

	private void OnEnable() {
		inputField.text = "";
		coroutineSubmit = null;
		inputField.Select();
		inputField.ActivateInputField();

		StartGame();
	}

	private void OnDisable() {
		SaveProgress();
		gameRunning = false;
		if (coroutineGame != null) {
			StopCoroutine(coroutineGame);
		}
	}

}

public class CharGroup {

	public string[] character;
	public string[] romanji;
	public int[] timesAttempted;
	public int[] timesCorrect;

	public CharGroup(int size) {
		character = new string[size];
		romanji = new string[size];
		timesAttempted = new int[size];
		timesCorrect = new int[size];
	}
}