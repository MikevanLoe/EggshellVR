using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MovePresDude : SceneEvent
{
	public MovePresDude ()
	{
	}
	
	public override void Execute()
	{
		var PresDude = GameObject.Find ("PresDude");
		var PresDudeCont = PresDude.GetComponent<NPCController> ();
		PresDudeCont.Travel (new Vector3 (784.0358f, 23.227f, 739.5342f), "TownState");
	}
}