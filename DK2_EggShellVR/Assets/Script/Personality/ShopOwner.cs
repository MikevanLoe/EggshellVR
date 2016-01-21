using UnityEngine;

public class ShopOwner : Personality
{
	public ShopOwner (NPCController c) : base(c)
	{
		Demand = 6;
	}
	
	public override void LookedAt()
	{
		if (_client.GetSwitch("Finished"))
			return;
		if (_client.NPCStateMachine.GetCurState ().Name == "TownState") {
			var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
			if (cutscenecont.IsPlaying ())
				return;
			cutscenecont.PlayCutscene ("ShopOwnerIntro");
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddItem(new ItemModel("Vishaak", 1, "Dit is een vishaak gemaakt van ijzer.  De vishaak is \nvan uitzonderlijk goede kwaliteit en lijkt nooit \neerder gebruikt te zijn."));
		}
	}
}