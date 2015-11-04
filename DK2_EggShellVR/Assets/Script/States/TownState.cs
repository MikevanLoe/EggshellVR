using UnityEngine;

public class TownState : State
{
	private GameObject _player;
	private float _faceRange;

	public TownState (NPCController c, float fr) : base(c)
	{
		_faceRange = fr;
		_player = GameObject.FindGameObjectWithTag ("Player");
	}

	public TownState (NPCController c) : base(c)
	{
		_faceRange = 1;
		_player = GameObject.FindGameObjectWithTag ("Player");
	}

	public override bool Handle()
	{
		//Check if the player is within range
		if (Vector3.Distance (_client.transform.position, _player.transform.position) < _faceRange)
			_client.LookAt(_player.transform);

		return true;
	}
}