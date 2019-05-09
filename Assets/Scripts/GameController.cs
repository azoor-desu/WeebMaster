using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public enum GameMode { Hiragana, Katakana, Both }
	[HideInInspector] public GameMode currentGameMode;

	bool gameRunning;

	public Text scoreText;
	public Text nextCharText;
	public Text answerText;
	public Text answerAccText;
	GameObject rightWrong;
	public Text rightWrongText;
	public InputField inputField;

	public float fieldReselectDelay = 0.1f;
	Coroutine coroutineSubmit;
	Coroutine coroutineGame;
	Coroutine coroutineRightWrong;

	//Array of probability to select each character. Higher is more likely.
	float[] hiragArray;
	float[] kataArray;

	int charIndex = 0;
	int score = 0;
	float timeStart;

	void Awake() {
		rightWrong = rightWrongText.transform.parent.gameObject;
	}

	void StartGame() {
		gameRunning = true;
		StartCoroutine(ScoreUpdate());
		charIndex = 0;
		score = 0;
		timeStart = Time.time;
		rightWrong.SetActive(false);
		if (coroutineGame != null) {
			StopCoroutine(coroutineGame);
		}
		coroutineGame = StartCoroutine(GameSeq());
	}

	void PopulateRandomList() {
		CharGroup hiragGrp = MenuController._singleton.GetCharGrp("hiragana");
		//CharGroup kataGrp = MenuController._singleton.GetCharGrp("katakana");
		hiragArray = new float[hiragGrp.character.Length];
		//kataArray = new float[kataArray.Length];

		//populate list with the latest data
		for (int i = 0; i < hiragGrp.character.Length; i++) {
			//Get probability by totalAttempts / correct attempts. 100% correct = 1, 50% correct = 2 etc.
			if (hiragGrp.timesCorrect[i] == 0) { //prevent divide by zero error. needs a sufficiently large number.
				hiragArray[i] = 10;
			} else {
				hiragArray[i] = hiragGrp.timesAttempted[i] / hiragGrp.timesCorrect[i];
			}
		}

		//for (int i = 0; i < kataGrp.character.Length; i++) {
		//	//Get probability by totalAttempts / correct attempts. 100% correct = 1, 50% correct = 2 etc.
		//	if (kataGrp.timesCorrect[i] == 0) { //prevent divide by zero error. needs a sufficiently large number.
		//		kataArray[i] = 10;
		//	} else {
		//		kataArray[i] = kataGrp.timesAttempted[i] / kataGrp.timesCorrect[i];
		//	}
		//}
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
			submittedAns = inputField.text;
			inputField.text = "";
			coroutineSubmit = StartCoroutine(SubmitAnswerCoroutine());
		}
		//Save updated accuracy data to charGroups but do not write to file yet.
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
		string prevGrp = "";
		int prevIndex = 0;

		while (gameRunning) {
			PopulateRandomList();

			string currGrpName = "";
			int currIndex = 0;
			string currAns = "";

			CharGroup currGrp = null;

			//Pull a random character.
			//If in BOTH mode, pull hiragana on evens, and kat on odds.
			switch (currentGameMode) {
				case GameMode.Hiragana:
				currGrpName = "hiragana";
				currIndex = RandomProbabilityArrayIndex(hiragArray);
				currGrp = MenuController._singleton.GetCharGrp(currGrpName);
				currAns = currGrp.romanji[currIndex];
				break;

				case GameMode.Katakana:
				break;

				case GameMode.Both:
				break;
			}

			SetCurrentCharacter(currGrp,currIndex);

			yield return new WaitUntil(() => submittedAns != "");
			if (submittedAns == currAns) {
				RightWrong(true);
				score++;
				currGrp.timesCorrect[currIndex]++;
			} else {
				RightWrong(false);
			}
			//Reset
			submittedAns = "";
			charIndex++;
			currGrp.timesAttempted[currIndex]++;
			prevGrp = currGrpName;
			prevIndex = currIndex;

			if (charIndex > MenuController._singleton.wordsPerGame) {
				gameRunning = false;
			}
		}
	}

	void SetCurrentCharacter(CharGroup currGrp,int index) {
		nextCharText.text = currGrp.character[index];
	}

	void RightWrong(bool isCorrect) {
		if (isCorrect) {
			rightWrongText.text = "CORRECT";
		} else {
			rightWrongText.text = "WRONG!";
		}
		if (coroutineRightWrong != null) {
			StopCoroutine(coroutineRightWrong);
		}
		coroutineRightWrong = StartCoroutine(RightWrongCo());
	}

	IEnumerator RightWrongCo() {
		rightWrong.SetActive(true);
		yield return new WaitForSeconds(0.25f);
		rightWrong.SetActive(false);
		coroutineRightWrong = null;
	}

	int RandomProbabilityArrayIndex(float[] inArray) {
		//Sum all numbers.
		float sum = 0;
		foreach (float item in inArray) {
			sum += item;
		}
		//Randomnize a float
		float random = Random.Range(0,sum);
		//Find corresponding index with random number
		float currentSum = 0;
		for (int i = 0; i < inArray.Length; i++) {
			currentSum += inArray[i];
			if (random >= currentSum) {
				return i;
			}
		}
		//If fails, return a complete random index.
		Debug.LogWarning("Could not find a proper random array! Anyhow picking."); 
		return Random.Range(0,inArray.Length);
	}

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
	public string groupName;
	public string[] character;
	public string[] romanji;
	public int[] timesAttempted;
	public int[] timesCorrect;

	public CharGroup(string grpName, int size) {
		groupName = grpName;
		character = new string[size];
		romanji = new string[size];
		timesAttempted = new int[size];
		timesCorrect = new int[size];
	}
}