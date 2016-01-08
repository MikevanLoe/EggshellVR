using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class UnlockPlayer : SceneEvent
{
	public UnlockPlayer ()
	{
	}
	
	public override void Execute()
	{
		var player = GameObject.FindGameObjectWithTag ("Player").GetComponent<RigidbodyFirstPersonController> ();
		player.LockedInput = false;
	}
}