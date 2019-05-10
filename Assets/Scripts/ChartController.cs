using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChartController : MonoBehaviour {

	public RectTransform content;
	public Text hi;
	Text[] texts;
	bool isHi;
	
	void Start () {
		List<Text> temp = new List<Text>();

		//Filter out the no-word boxes.
		for (int i = 0; i < content.childCount; i++) {
			if (i == 36 || i == 38 || i == 46 || i == 47 || i == 48 || i == 51 || i == 52 || i == 53 || i == 54) {
				content.GetChild(i).GetChild(0).GetComponent<Text>().text = "-";
			} else {
				temp.Add(content.GetChild(i).GetChild(0).GetComponent<Text>());
			}
		}

		texts = temp.ToArray();
		print(texts.Length);
		SetChars(true);
		hi.text = "Hiragana";
		isHi = true;
	}

	void SetChars(bool hirag) {
		for (int i = 0; i < texts.Length; i++) {
			if (hirag) {
				texts[i].text = MenuController._singleton.charGroups[0].character[i] + "\n(" + MenuController._singleton.charGroups[0].romanji[i] + ")";
			} else {
				texts[i].text = MenuController._singleton.charGroups[1].character[i] + "\n(" + MenuController._singleton.charGroups[0].romanji[i] + ")";
			}
		}
	}

	public void ButtToggleHi() {
		isHi = !isHi;
		if (isHi) {
			SetChars(true);
			hi.text = "Hiragana";
		} else {
			SetChars(false);
			hi.text = "Katakana";
		}
	}
}