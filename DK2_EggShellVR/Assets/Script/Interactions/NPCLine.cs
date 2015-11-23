using System;
using UnityEngine;

public class NPCLine : Interaction
{
	private string _NPCName;
	private string _voiceKey;

	public NPCLine (string name, string key, float dur)
	{
		_NPCName = name;
		_voiceKey = key;
		Duration = dur;
	}

	public override void Execute()
	{
		//Get the NPC and its Audio Source
		var npc = GameObject.Find (_NPCName);
		if(npc == null)
			throw new Exception("Cutscene NPC name not found in scene");
		var aSource = npc.GetComponent<AudioSource> ();
		if(aSource == null)
			throw new Exception("Cutscene NPC has no audio source attached");

		//Get the Game Controller and the required audio clip
		var GC = GameObject.FindGameObjectWithTag ("GameController");
		if(GC == null)
			throw new Exception("Cutscene requires game controller in scene");
		var cont = GC.GetComponent<GameController> ();
		if(cont == null)
			throw new Exception("Game Controller has no GameController component attached");
		AudioClip clip = cont.GetClip (_voiceKey);

		//Play the clip from the NPC's audio source
		aSource.PlayOneShot (clip);
	}
}