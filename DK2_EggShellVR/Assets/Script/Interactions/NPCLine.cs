using System;
using UnityEngine;

public class NPCLine : Interaction
{
	private TextMesh _subtitlesMesh;
	private string _subtitlesText;
	private string _NPCName;
	private string _voiceKey;

	public NPCLine (string name, string key, string sub, float dur)
	{
		if (name == null)
			throw new UnityException ("JSON format error! No NPC name");
		if (key == null)
			throw new UnityException ("JSON format error! No voice key given");
		_NPCName = name;
		_voiceKey = key;
		_subtitlesText = sub;
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

		_subtitlesMesh = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<TextMesh>();
		_subtitlesMesh.text = _subtitlesText;
		_subtitlesMesh.color = Color.green;

		//Play the clip from the NPC's audio source
		aSource.PlayOneShot (clip);
	}

	public override void Finish()
	{
		_subtitlesMesh = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<TextMesh>();
		_subtitlesMesh.text = "";
		_subtitlesMesh.color = Color.blue;
	}
}