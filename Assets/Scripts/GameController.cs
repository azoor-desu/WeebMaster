using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public enum GameMode { Hiragana, Katakana }
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

	//Stores all the probabilities for each character, indexed to selectedGrps
	float[] probabilityArray;

	int score = 0;
	int wordsElapsed = 0;
	float timeStart;

	void Awake() {
		rightWrong = rightWrongText.transform.parent.gameObject;
	}

	void StartGame() {
		gameRunning = true;
		StartCoroutine(ScoreUpdate());
		score = 0;
		timeStart = Time.time;
		rightWrong.SetActive(false);

		inputField.interactable = true;
		inputField.Select();
		inputField.ActivateInputField();

		if (coroutineGame != null) {
			StopCoroutine(coroutineGame);
		}
		coroutineGame = StartCoroutine(GameSeq());
	}

	IEnumerator GameSeq() {
		yield return null; //wait for 1 frame for GameMode to be udpated
		submittedAns = "";
		int prevIndex = -1;
		wordsElapsed = 0;

		while (gameRunning) {
			RefreshProbabilityArray();
			ShowAnswer(prevIndex);
			int sctItmIndx = 0;
			string currAns = "";

			//Pull a random character
			sctItmIndx = RandomProbabilityArrayIndex(probabilityArray);
			currAns = MenuController._singleton.GetRomanji(MenuController._singleton.selectedItems[sctItmIndx]);

			SetCurrentCharacter(MenuController._singleton.selectedItems[sctItmIndx]);

			yield return new WaitUntil(() => submittedAns != "");
			if (submittedAns == currAns) {
				RightWrong(true);
				score++;
				if (currentGameMode == GameMode.Hiragana) {
					MenuController._singleton.charMemory.hirag[MenuController._singleton.selectedItems[sctItmIndx]].correct++;
				} else {
					MenuController._singleton.charMemory.kata[MenuController._singleton.selectedItems[sctItmIndx]].correct++;
				}
			} else {
				RightWrong(false);
			}

			//Reset
			submittedAns = "";
			wordsElapsed++;
			prevIndex = sctItmIndx;
			if (currentGameMode == GameMode.Hiragana) {
				MenuController._singleton.charMemory.hirag[MenuController._singleton.selectedItems[sctItmIndx]].attempts++;
			} else {
				MenuController._singleton.charMemory.kata[MenuController._singleton.selectedItems[sctItmIndx]].attempts++;
			}

			if (MenuController._singleton.wordsPerGame != 0 && wordsElapsed >= MenuController._singleton.wordsPerGame) {
				gameRunning = false;
				ShowAnswer(prevIndex);
				nextCharText.text = "-";
				wordsElapsed = MenuController._singleton.wordsPerGame;

				inputField.DeactivateInputField();
				inputField.interactable = false;
			}
		}
	}

	//Re-populates probability array with values from charMemory
	void RefreshProbabilityArray() {
		probabilityArray = new float[MenuController._singleton.selectedItems.Count];
		for (int i = 0; i < probabilityArray.Length; i++) {
			if (currentGameMode == GameMode.Hiragana) {
				//Probability value = 1 / (correct / total)
				//if total = 0, divide by zero. set probability to 100.
				if (MenuController._singleton.GetHiragAttempts(MenuController._singleton.selectedItems[i]) == 0) {
					probabilityArray[i] = 100;
				} else {
					//Probability value = 1 / (correct / total)
					probabilityArray[i] = 1f / (MenuController._singleton.GetHiragCorrect(MenuController._singleton.selectedItems[i]) / MenuController._singleton.GetHiragAttempts(MenuController._singleton.selectedItems[i]));
				}
			} else { //KATAKANA

				//Probability value = 1 / (correct / total)
				//if total = 0, divide by zero. set probability to 100.
				if (MenuController._singleton.GetKataAttempts(MenuController._singleton.selectedItems[i]) == 0) {
					probabilityArray[i] = 100;
				} else {
					//Probability value = 1 / (correct / total)
					probabilityArray[i] = 1f / (MenuController._singleton.GetKataCorrect(MenuController._singleton.selectedItems[i]) / MenuController._singleton.GetKataAttempts(MenuController._singleton.selectedItems[i]));
				}
			}
		}
	}

	void DisplayScoreText() {
		float timer = Time.time - timeStart;
		int minutes = Mathf.FloorToInt(timer / 60F);
		int seconds = Mathf.FloorToInt(timer - minutes * 60);
		string niceTime = string.Format("{0:0}:{1:00}",minutes,seconds);
		int totalqns = MenuController._singleton.wordsPerGame;
		if (totalqns < 0) {
			totalqns = wordsElapsed;
		}
		scoreText.text = "Word: " + wordsElapsed + "/" + totalqns + "\nScore: " + score + "/" + totalqns + "\nTime: " + niceTime;
	}

	void SaveProgress() {
		if (Time.time > 1) {
			print("Savegame");
			MenuController._singleton.WriteSaveFile();
		}
	}

	public void SubmitAnswer() {
		if (gameRunning && coroutineSubmit == null && inputField.text != "") {
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
			yield return new WaitForSeconds(.2f);
			DisplayScoreText();
		}
	}

	string submittedAns;

	void SetCurrentCharacter(int index) {
		if (currentGameMode == GameMode.Hiragana) {
			nextCharText.text = MenuController._singleton.GetHirag(index);
		} else {
			nextCharText.text = MenuController._singleton.GetKata(index);
		}
	}

	void ShowAnswer(int _prevIndex) {

		if (_prevIndex == -1) {
			answerText.text = "-";
			answerAccText.text = "Acc: -";
		} else {
			if (currentGameMode == GameMode.Hiragana) {
				answerText.text = MenuController._singleton.charMemory.hirag[MenuController._singleton.selectedItems[_prevIndex]].character + " (" + MenuController._singleton.charMemory.romanji[_prevIndex] + ")";
				answerAccText.text = "Acc: " + ((float)MenuController._singleton.charMemory.hirag[MenuController._singleton.selectedItems[_prevIndex]].correct / (float)MenuController._singleton.charMemory.hirag[MenuController._singleton.selectedItems[_prevIndex]].attempts * 100f).ToString("F2") + "%";
			} else {
				answerText.text = MenuController._singleton.charMemory.kata[MenuController._singleton.selectedItems[_prevIndex]].character + " (" + MenuController._singleton.charMemory.romanji[_prevIndex] + ")";
				answerAccText.text = "Acc: " + ((float)MenuController._singleton.charMemory.kata[MenuController._singleton.selectedItems[_prevIndex]].correct / (float)MenuController._singleton.charMemory.kata[MenuController._singleton.selectedItems[_prevIndex]].attempts * 100f).ToString("F2") + "%";
			}
		}
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

	//Put in an array of probabilities and it will return a index. Higher number gets higher chance.
	int RandomProbabilityArrayIndex(float[] _probabilityArray) {
		//Sum all numbers.
		float sum = 0;
		foreach (float item in _probabilityArray) {
			sum += item;
		}
		
		//Randomnize a float
		float random = Random.Range(0,sum);
		//Find corresponding index with random number
		float currentSum = 0;
		for (int i = 0; i < _probabilityArray.Length; i++) {
			currentSum += _probabilityArray[i];
			if (random <= currentSum) {
				return i;
			}
		}
		//If fails, return a complete random index.
		Debug.LogWarning("Could not find a proper random array! Anyhow picking."); 
		return Random.Range(0,_probabilityArray.Length);
	}

	private void OnApplicationFocus(bool focus) {
		if (!focus) {
			SaveProgress();
		}
	}

	private void OnEnable() {
		inputField.text = "";
		coroutineSubmit = null;

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

public class CharMemory {
	public string[] romanji;
	public CharItem[] hirag;
	public CharItem[] kata;

	public CharMemory(int size) {
		romanji = new string[size];
		hirag = new CharItem[size];
		kata = new CharItem[size];
	}
}

[System.Serializable]
public struct CharItem {
	public string character;
	public int correct, attempts;
}