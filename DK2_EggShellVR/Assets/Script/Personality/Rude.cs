using UnityEngine;

public class Rude : Personality
{	
	private PlayerController _player;
	private bool _done;

	public Rude (NPCController c) : base(c)
	{
		_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		Demand = Mathf.Infinity;
		Demand = 22;
	}

	public override void LookedAt ()
	{
		
		if (_client.GetSwitch("Finished"))
			return;
		if (_client.NPCStateMachine.GetCurState ().Name == "TownState") {
			var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
			
			if(cutscenecont.IsPlaying())
				return;
			//Only talk if the player has the stick
			if (!_player.HasItem("Stok"))
			{
				cutscenecont.PlayCutscene ("RudeIgnore");
			}
			else
				cutscenecont.PlayCutscene ("RudeDude");
		}
	}
}