using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Tutorial : SceneEvent
{
	private GameObject _tutorialObject;
	private TextMesh _hintMesh;
	private string _hintText;

	public Tutorial (string name, string text)
	{
		_tutorialObject = GameObject.Find (name);
		if (_tutorialObject == null) {
			Debug.LogWarning ("Tutorial object \"" + name + "\" not found");
			return;
		}
		_tutorialObject.SetActive (false);
		_hintMesh = GameObject.Find ("HintText").GetComponent<TextMesh>();
		_hintText = text;
	}

	public override void Execute()
	{
		_tutorialObject.SetActive (true);
		_hintMesh.color = new Color (0.65f, 1f, 0.62f);
		_hintMesh.text = _hintText;
	}
	
	public override void Cancel()
	{
		Finish ();
	}
	
	public override void Finish()
	{
		_hintMesh.text = "";
		_tutorialObject.SetActive (false);
	}
}