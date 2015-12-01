using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class LockPlayer : SceneEvent
{
	public LockPlayer (float time)
	{
		Duration = time;
	}

	public override void Execute()
	{
		var player = GameObject.FindGameObjectWithTag ("Player").GetComponent<RigidbodyFirstPersonController> ();
		player.LockedInput = true;
	}
}