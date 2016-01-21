using UnityEngine;

public class Islander : Personality
{
	private Transform _player;
	private PlayerController _playerCont;
	private bool _done;
	private bool _done2;

	public Islander (NPCController c) : base(c)
	{
		_player = GameObject.FindGameObjectWithTag ("Player").transform;
		_playerCont = _player.GetComponent<PlayerController> ();
		Demand = Mathf.Infinity;
	}
	
	public override void Update()
	{
		if (_done && (!_playerCont.HasItem("Goud", 6) || _done2))
			return;
		//When the player gets close
		if (Vector3.Distance(_player.position, _client.transform.position) < 2.5f) {
			var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
			if(cutscenecont.IsPlaying())
				return;
			if(_playerCont.HasItem("Goud", 6)){
				cutscenecont.PlayCutscene ("Ferryman");
				_done2 = true;
			}
			else{
				cutscenecont.PlayCutscene ("IslandIntro");
				_done = true;
			}
		}
	}
}