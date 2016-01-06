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
		if (name == null || key == null)
			Debug.LogWarning ("NPC line had no valid voice info attached.");
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

		if (_NPCName != null && _voiceKey != null) {
			//Get the Game Controller and the required audio clip
			var GC = GameObject.FindGameObjectWithTag ("GameController");
			if (GC == null)
				throw new Exception ("Cutscene requires game controller in scene");
			var cont = GC.GetComponent<GameController> ();
			if (cont == null)
				throw new Exception ("Game Controller has no GameController component attached");
			AudioClip clip = cont.GetClip (_voiceKey);
			//Play the clip from the NPC's audio source
			aSource.PlayOneShot (clip);
		}

		_subtitlesMesh = GameObject.Find("HintText").GetComponent<TextMesh>();
		_subtitlesMesh.color = Color.white;
		_subtitlesMesh.text = _subtitlesText;
	}
	
	public override void Cancel()
	{
		Finish ();
	}

	public override void Finish()
	{
		_subtitlesMesh = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<TextMesh>();
		_subtitlesMesh.text = "";
	}
}