using UnityEngine;

public class Couple : Personality
{
	public Couple (NPCController c) : base(c)
	{
		Demand = Mathf.Infinity;
		Demand = 8;
	}
	
	public override void LookedAt()
	{
		if (_client.GetSwitch("Finished"))
			return;
		if (_client.NPCStateMachine.GetCurState ().Name == "TownState") {
			var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
			if(!cutscenecont.IsPlaying())
				cutscenecont.PlayCutscene ("CoupleScene");
		}
	}
}