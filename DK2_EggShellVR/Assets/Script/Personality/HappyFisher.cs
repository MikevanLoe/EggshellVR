using UnityEngine;

public class HappyFisher : Personality
{	
	private const int DelayTime = 5;
	private float _delayTime;

	public HappyFisher (NPCController c) : base(c)
	{
		_voiceClips.Add(_gameController.GetClip ("visser_01"));
		_voiceClips.Add(_gameController.GetClip ("visser_02"));
		_voiceClips.Add(_gameController.GetClip ("visser_03"));
		_voiceClips.Add(_gameController.GetClip ("visser_04"));
		_voiceClips.Add(_gameController.GetClip ("visser_05"));
		_voiceClips.Add(_gameController.GetClip ("visser_06"));

		_demand = 50;
	}

	public override void LookedAt()
	{
		//If a clip was still playing from the update, stop it
		if (_audioSource.isPlaying) {
			if(!_client.GetSwitch ("LookedAt"))
				_audioSource.Stop();
			else
				return;
		}

		if(!_client.GetSwitch("LookedAt"))
			_client.SetSwitch ("LookedAt", true);
		var clip = SelectClipLookAt ();
		
		if (clip == _voiceClips [4])
			_client.SetSwitch ("DidIntro", true);
		_audioSource.clip = clip;
		_audioSource.Play();
	}

	AudioClip SelectClipLookAt ()
	{
		AudioClip clip = null;
		//Hele intro
		clip = _voiceClips [4];
		if (_client.GetSwitch ("DidIntro")) {
			//TODO: Remind player of goal
			clip = null;
		}
		if (_gameController.GetSwitch ("GotMoney")) {
			//Je hebt het geld? Let's go!
			clip = _voiceClips [5];
		}
		return clip;
	}

	public override void Update()
	{
		//Don't do anything until the guard is finished with his lines
		if (!_gameController.GetSwitch ("GuardFinished"))
			return;
		//Only do stuff before it's been looked at
		if (_client.GetSwitch ("LookedAt"))
			return;
		if (Time.time < _delayTime)
			return;
		if (_audioSource.isPlaying)
			return;
		//Hallo, vreemdeling!
		AudioClip clip = _voiceClips [0];
		if (_client.GetSwitch ("Greeted")) {
			//Ik sta hier!
			clip = _voiceClips[Random.Range(1, 4)];
		}

		if (clip == _voiceClips [0])
			_client.SetSwitch ("Greeted", true);
		
		_audioSource.clip = clip;
		_audioSource.Play();

		_delayTime = Time.time + DelayTime;
	}
}