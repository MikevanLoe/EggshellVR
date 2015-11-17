using UnityEngine;

public class Smith : Personality
{
	private const int DelayTime = 5;
	private float _delayTime;
	private PlayerController _playerController;
	
	public Smith (NPCController c) : base(c)
	{
		_voiceClips.Add(_gameController.GetClip ("smid_01"));
		_voiceClips.Add(_gameController.GetClip ("smid_02"));
		_voiceClips.Add(_gameController.GetClip ("smid_03"));

		var Player = GameObject.FindGameObjectWithTag ("Player");
		_playerController = Player.GetComponent<PlayerController> ();

		_demand = 40;
	}
	
	public override void LookedAt()
	{
		if (_audioSource.isPlaying) {
			return;
		}
		var clip = SelectClipLookAt ();

		//If we're making the hook, give the player the hook
		if (clip == _voiceClips [1]) {
			_client.SetSwitch ("Fixed", true);
			_playerController.AddItem(new ItemModel("Vishaak", 1));
			_playerController.RemoveItem("Kapotte Vishaak");
		}
		_audioSource.clip = clip;
		_audioSource.Play();
	}
	
	AudioClip SelectClipLookAt ()
	{
		AudioClip clip = null;
		//Ik ben de smid
		clip = _voiceClips [0];
		if (_playerController.HasItem("Kapotte Vishaak")) {
			//Ik fix je haak
			clip = _voiceClips[1];
		}
		if (_client.GetSwitch ("Fixed")) {
			//Nog veel gehad aan die haak?
			clip = _voiceClips[2];
		}
		return clip;
	}
}