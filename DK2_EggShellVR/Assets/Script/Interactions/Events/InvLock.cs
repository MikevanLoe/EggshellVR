using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class InvLock : SceneEvent
{
	public InvLock ()
	{
	}
	
	public override void Execute()
	{
		var player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		player.InvLock = true;
	}
}