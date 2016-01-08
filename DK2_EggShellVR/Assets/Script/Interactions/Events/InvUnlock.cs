using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class InvUnlock : SceneEvent
{
	public InvUnlock ()
	{
	}
	
	public override void Execute()
	{
		var player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		player.InvLock = false;
	}
}