using UnityEngine;
using System.Collections.Generic;

public class CrowdState : State
{
	private const float StareScoreMax = 100;
	private const float StareScoreLeast = 55;
	private const float Speed = 8f;
	private const float PositiveFactor = 4;
	private const float MaxAngle = 20;
	private const float DistractionMax = 100; 
	
	private float _stareScore;
	private float _distraction;
	private SpriteRenderer _debugIndicator;
	private List<Sprite> _debugSprites;
	private TextMesh _angleIndicator;
	private PlayerController _playerController;
	private Transform _player;

	public CrowdState (NPCController c) : base(c) 
	{
		//The player object is used to stare at, and also to check if it's looking at the NPC
		var Player = GameObject.FindGameObjectWithTag ("Player");
		_playerController = Player.GetComponent<PlayerController> ();
		_player = Player.transform;

		//Debug indicators for tracking NPC status
		_debugIndicator = _client.transform.GetComponentInChildren<SpriteRenderer> ();
		_angleIndicator = _client.transform.GetComponentInChildren<TextMesh> ();

		//Sprites that show how the NPC should be feeling (when we actually get emotes later)
		_debugSprites = new List<Sprite> ();
		_debugSprites.Add (Resources.Load<Sprite> ("Interested"));
		_debugSprites.Add (Resources.Load<Sprite>("bored"));
		_debugSprites.Add (Resources.Load<Sprite>("zzz"));
	}
	
	public bool IsPositive()
	{
		return _distraction == 0;
	}
	
	public override bool Handle()
	{
		//When in the crowd, stare at the player at all times
		//At least for now. When we don't have extensive NPC reactions yet.
		_client.LookAt(_player.transform);	

		//Get the angle at which the player is looking at this NPC.
		float angle = _playerController.GetAngle (_client.GetCenterTransform().position);

		//If the angle is too high (not looking directly enough at the NPC then the stare score drops
		if (angle < MaxAngle) 
		{
			//The score raises faster than it drops. Also the increase is higher when the angle is lower.
			_stareScore += Time.deltaTime * Speed * PositiveFactor * (1 - angle / MaxAngle);
		}
		else
			_stareScore -= Time.deltaTime * Speed;

		//Keep the stare score between 0 and 100
		_stareScore = Mathf.Clamp (_stareScore, 0, StareScoreMax);

		//If the player isn't staring well enough, the NPC gets distracted
		if (_stareScore > StareScoreLeast) 
			_distraction -= Time.deltaTime * Speed;
		else
			_distraction += Time.deltaTime * Speed;
		_distraction = Mathf.Clamp (_distraction, 0, DistractionMax);
	
		DrawDebug (angle);
		
		return true;
	}

	void DrawDebug (float angle)
	{
		//Only show debug indicators when debug is active
		if (Debug.isDebugBuild) 
		{
			if(_debugIndicator == null)
				return;
			_angleIndicator.text = Mathf.Round (_stareScore).ToString () + " - " + Mathf.Round (_distraction).ToString () + " - " + Mathf.Round (angle).ToString ();
			if (_distraction <= 0)
				_debugIndicator.sprite = _debugSprites [0];
			else if (_distraction < 55)
				_debugIndicator.sprite = _debugSprites [1];
			else
				_debugIndicator.sprite = _debugSprites [2];
		}
	}
}