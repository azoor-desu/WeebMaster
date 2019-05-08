using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public enum GameMode { Hiragana, Katakana, Both }
	[HideInInspector] public GameMode currentGameMode;

	public Text score;
	public Text word;
	public InputField inputField;

	public float fieldReselectDelay = 0.05f;
	Coroutine coroutineSubmit;

	void Awake() {

	}

	void Update() {
		
	}

	private void OnEnable() {
		inputField.text = "";
		coroutineSubmit = null;
		inputField.Select();
		inputField.ActivateInputField();
	}

	private void OnDisable() {

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

}