using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CloseMenu : SceneEvent
{
	private PlayerController _player;

	public CloseMenu ()
	{
		var playerObj = GameObject.FindGameObjectWithTag ("Player");
		_player = playerObj.GetComponent<PlayerController> ();
	}

	public override void Execute()
	{
		_player.ForceOpenInv (true, true);
	}
}