using UnityEngine;

public class ShopOwner : Personality
{
	public ShopOwner (NPCController c) : base(c)
	{
		_demand = 6;
	}
	
	public override void LookedAt()
	{
		if (_client.GetSwitch("Finished"))
			return;
		if (_client.NPCStateMachine.GetCurState ().Name == "TownState") {
			var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
			cutscenecont.PlayCutscene ("ShopOwnerIntro");
			if(!cutscenecont.IsPlaying())
				GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddItem(new ItemModel("Vishaak", 1, "Dit is een vishaak gemaakt van ijzer.  De vishaak is \nvan uitzonderlijk goede kwaliteit en lijkt nooit \neerder gebruikt te zijn."));
		}
	}
}