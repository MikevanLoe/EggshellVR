using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class UnlockPlayer : SceneEvent
{
	public UnlockPlayer (float time = 0)
	{
		Duration = time;
	}
	
	public override void Execute()
	{
		var player = GameObject.FindGameObjectWithTag ("Player").GetComponent<RigidbodyFirstPersonController> ();
		player.LockedInput = false;
	}
}