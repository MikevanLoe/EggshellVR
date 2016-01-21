using UnityEngine;

public class PresTutorial : Personality
{
	private GameController GameCont;

	public PresTutorial (NPCController c) : base(c)
	{
		GameCont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		GameCont.Listen (GlobalStateChanged);
		Demand = 0;
	}

	private void GlobalStateChanged()
	{
		//When the player finished fishing, change routine
		if (GameCont.GetSwitch ("EnoughFish"))
			_client.SetSwitch ("Finished", false);
	}
	
	public override void LookedAt()
	{
		if (_client.GetSwitch("Finished") || _client.GetSwitch("Finished2"))
			return;
		if (_client.NPCStateMachine.GetCurState ().Name == "TownState") {
			var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
			if (cutscenecont.IsPlaying ())
				return;
			if (GameCont.GetSwitch ("EnoughFish"))
			{
				cutscenecont.PlayCutscene ("PresTutorial");
				_client.SetSwitch("Finished2", true);
			}
			else
				cutscenecont.PlayCutscene ("PresIgnore");
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddItem(new ItemModel("Vishaak", 1, "Dit is een vishaak gemaakt van ijzer.  De vishaak is \nvan uitzonderlijk goede kwaliteit en lijkt nooit \neerder gebruikt te zijn."));
		}
	}
}