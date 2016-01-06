using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Finished : SceneEvent
{
	private string _subject;

	public Finished (string subject)
	{
		_subject = subject;
	}
	
	public override void Execute()
	{
		var npc = GameObject.Find (_subject).GetComponent<NPCController> ();
		npc.SetSwitch("Finished", true);
	}
}