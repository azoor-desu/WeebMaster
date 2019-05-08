using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour {

	public RectTransform mainMenu;
	public RectTransform chooseMenu;
	public RectTransform settingsMenu;
	public RectTransform gameScreen;
	public RectTransform chartScreen;

	public static MenuController _singleton;
	GameController gameCtrl;

	void Awake() {
		if (_singleton == null) {
			 _singleton = this;
		}

		gameCtrl = gameScreen.GetComponent<GameController>();

		ChangeScreen(mainMenu);
	}

	void Update() {

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

}