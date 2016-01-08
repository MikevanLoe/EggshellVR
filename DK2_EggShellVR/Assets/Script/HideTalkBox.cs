using UnityEngine;
using System.Collections;

public class HideTalkBox : MonoBehaviour {
	public TextMesh Text;
	public GameObject TextBox;
	public GameObject TalkIcon;
	
	void Update () {
		TextBox.SetActive (Text.text != "");
		if (Text.text == "")
			TalkIcon.SetActive (false);
	}
}
