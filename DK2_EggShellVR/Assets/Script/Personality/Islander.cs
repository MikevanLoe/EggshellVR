using UnityEngine;

public class Islander : Personality
{	
	private Transform _player;
	private bool _done;

	public Islander (NPCController c) : base(c)
	{
		_player = GameObject.FindGameObjectWithTag ("Player").transform;
		_demand = Mathf.Infinity;
	}
	
	public override void Update()
	{
		if (_done)
			return;
		//When the player gets close
		if (Vector3.Distance(_player.position, _client.transform.position) < 2.5f) {
			var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
			cutscenecont.PlayCutscene ("IslandIntro");
			_done = true;
		}
	}
}