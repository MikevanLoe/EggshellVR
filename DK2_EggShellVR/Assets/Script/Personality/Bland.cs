using UnityEngine;

public class Bland : Personality
{		
	public Bland (NPCController c) : base(c)
	{
		_voiceClips.Add(_gameController.GetClip ("bezoeker_01"));
		_voiceClips.Add(_gameController.GetClip ("bezoeker_02"));
		_voiceClips.Add(_gameController.GetClip ("bezoeker_03"));
		_voiceClips.Add(_gameController.GetClip ("bezoeker_04"));
		
		_demand = Random.Range(1, 10);
	}
	
	public override void LookedAt()
	{
		if (_client.NPCStateMachine.GetCurState ().Name == "TownState") {
			var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
			cutscenecont.PlayCutscene ("MedusaIntro");
		}
	}
}