using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class QueueScene : SceneEvent
{
	private string _sceneName;

	public QueueScene (string name)
	{
		_sceneName = name;
	}

	public override void Execute()
	{
		var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
		cutscenecont.QueueScene (_sceneName);
	}
}