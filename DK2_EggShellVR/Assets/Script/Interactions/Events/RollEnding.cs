using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class RollEnding : SceneEvent
{
	public RollEnding ()
	{
	}
	
	public override void Execute()
	{
		var player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		player.InvLock = true;
	}
}