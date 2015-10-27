using UnityEngine;
using System.Collections.Generic;

public abstract class Personality
{
	protected NPCController _client;
	protected AudioSource _audioSource;
	protected List<AudioClip> _voiceClips;
	protected GameController _gameController;

	public Personality (NPCController c)
	{
		_client = c;
		_audioSource = _client.GetComponent<AudioSource> ();
		_voiceClips = new List<AudioClip>();
		if (_audioSource == null)
			throw new UnityException ("NPC has no audio source attached.");
		var gameObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameObject == null)
			throw new UnityException ("No game controller found in scene.");
		_gameController = gameObject.GetComponent<GameController> ();
	}

	public abstract void LookedAt();

	public abstract void Update();
}
