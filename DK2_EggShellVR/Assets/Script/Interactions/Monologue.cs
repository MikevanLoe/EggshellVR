using System;
using UnityEngine;

public class Monologue : Interaction
{
	private TextMesh _subtitlesMesh;
	private string _subtitlesText;
	private string _voiceKey;
	private AudioClip _clip;

	public Monologue (string key, string sub, float dur)
	{
		if (key == null)
			Debug.LogWarning ("Monologue had no valid voice info attached.");
		_voiceKey = key;
		_subtitlesText = sub;
		Duration = dur;
		
		if (_voiceKey != null) 
		{
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
		//Get the monologue audio source and its Audio Source
		var asObj = GameObject.FindGameObjectWithTag ("Player").transform.FindChild("Monologue");
		if(asObj == null)
			throw new Exception("Player has no audio source attached");
		var aSource = asObj.GetComponent<AudioSource> ();

		//Play the clip from the NPC's audio source
		aSource.PlayOneShot (_clip);
		
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