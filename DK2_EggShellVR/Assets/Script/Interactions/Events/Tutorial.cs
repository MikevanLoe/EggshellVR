using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Tutorial : SceneEvent
{
	private GameObject _tutorialObject;
	private TextMesh _hintMesh;
	private string _hintText;

	public Tutorial (string name, string text, float time)
	{
		_tutorialObject = GameObject.Find (name);
		if (_tutorialObject != null)
			_tutorialObject.SetActive (false);
		else
			Debug.LogWarning ("Tutorial object \"" + name + "\" not found");
		_hintMesh = GameObject.Find ("HintText").GetComponent<TextMesh>();
		_hintText = text;
		Duration = time;
	}

	public override void Execute()
	{
		if (_tutorialObject != null)
			_tutorialObject.SetActive (true);
		//Light green
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
		if (_tutorialObject != null)
			_tutorialObject.SetActive (false);
	}
}