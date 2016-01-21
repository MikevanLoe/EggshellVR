using System;
using UnityEngine;

public class NPCLine : Interaction
{
	private TextMesh _subtitlesMesh;
	private string _subtitlesText;
	private string _NPCName;
	private string _voiceKey;
	private AudioClip _clip;
	private Animator _anim;

	public NPCLine (string name, string key, string sub, float dur)
	{
		if (name == null || key == null)
			Debug.LogWarning ("NPC line had no valid voice info attached.");
		_NPCName = name;
		_voiceKey = key;
		_subtitlesText = sub;
		Duration = dur;
		
		if (_NPCName != null && _voiceKey != null) {
			//Get the Game Controller and the required audio clip
			var GC = GameObject.FindGameObjectWithTag ("GameController");
			if (GC == null)
				throw new Exception ("Cutscene requires game controller in scene");
			var cont = GC.GetComponent<GameController> ();
			if (cont == null)
				throw new Exception ("Game Controller has no GameController component attached");
			_clip = cont.GetClip (_voiceKey);
			if(Duration >= 0)
				Duration = _clip.length + 0.2f;
		}
	}

	public override void Execute()
	{
		//Get the NPC and its Audio Source
		var npc = GameObject.Find (_NPCName);
		if(npc == null)
			throw new Exception("Cutscene NPC name not found in scene:" + _NPCName);
		_anim = npc.GetComponent<Animator> ();
		if(_anim == null)
			throw new Exception("Cutscene NPC has no animator attached");
		var aSource = npc.GetComponent<AudioSource> ();
		if(aSource == null)
			throw new Exception("Cutscene NPC has no audio source attached");
		//Play the clip from the NPC's audio source
		aSource.clip = _clip;
		aSource.Play ();

		_anim.SetBool ("Talk", true);
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
		_anim.SetBool ("Talk", false);
	}
}