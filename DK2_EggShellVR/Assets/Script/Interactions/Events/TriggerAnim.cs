using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class TriggerAnim : SceneEvent
{
	private string _subject;
	private string _trigger;

	public TriggerAnim (string subject, string trigger)
	{
		_subject = subject;
		_trigger = trigger;
	}
	
	public override void Execute()
	{
		var anim = GameObject.Find (_subject).GetComponent<Animator> ();
		anim.SetTrigger (_trigger);
	}
}