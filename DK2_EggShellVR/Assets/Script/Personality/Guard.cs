using UnityEngine;

public class Guard : Personality
{
	private const int DestroyTime = 30;
	private const int MoveSpeed = 3;
	private float _delayTime;
	private AudioSource _playerAudioSource;
	
	public Guard (NPCController c) : base(c)
	{
		_voiceClips.Add(_gameController.GetClip ("speler_01"));
		_voiceClips.Add(_gameController.GetClip ("guard_01"));

		var Player = GameObject.FindGameObjectWithTag ("Player");
		_playerAudioSource = Player.GetComponent<AudioSource> ();
		MonoBehaviour.Destroy (_client.gameObject, DestroyTime);

		_demand = Mathf.Infinity;
	}
	
	public override void LookedAt()
	{
		//Do nothing
	}
	
	public override void Update()
	{
		//Make the boatman slowly leave
		_client.transform.position += -_client.transform.forward * MoveSpeed * Time.deltaTime;
		if (!_client.GetSwitch ("PlayerLine")) {
			//Player: Laat me hier niet achter!!
			_playerAudioSource.clip = _voiceClips [0];
			_playerAudioSource.Play ();
			_client.SetSwitch ("PlayerLine", true);
			return;
		} 
		if (_audioSource.isPlaying || _playerAudioSource.isPlaying)
			return;
		if (_gameController.GetSwitch ("GuardFinished"))
			return;
		if (_client.GetSwitch ("Finished")) 
		{
			_gameController.SetSwitch ("GuardFinished", true);
			return;
		}
		
		_audioSource.clip = _voiceClips[1];
		_audioSource.Play();
		_client.SetSwitch ("Finished", true);
	}
}