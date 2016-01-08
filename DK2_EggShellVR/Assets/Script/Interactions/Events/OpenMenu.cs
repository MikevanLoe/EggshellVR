using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class OpenMenu : SceneEvent
{
	private PlayerController _player;

	public OpenMenu ()
	{
		var playerObj = GameObject.FindGameObjectWithTag ("Player");
		_player = playerObj.GetComponent<PlayerController> ();
	}

	public override void Execute()
	{
		_player.ForceOpenInv (true, true);
	}
}