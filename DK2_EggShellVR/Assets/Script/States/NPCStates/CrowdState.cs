using UnityEngine;
using System.Collections.Generic;

public class CrowdState : State<NPCController>
{
	private const float StareScoreMax = 100;
	private const float StareScoreLeast = 55;
	private const float Speed = 8f;
	private const float PositiveFactor = 4;
	private const float MaxAngle = 40;
	private const float DistractionMax = 100; 
	
	private float _stareScore = 100;
	private float _distraction;
	private float _lookScore;
	private float _lastReaction;
	private bool _buying;
	private bool _animStarted;
	private SpriteRenderer _debugIndicator;
	private List<Sprite> _debugSprites;
	private TextMesh _angleIndicator;
	private PlayerController _playerController;
	private Transform _player;
	private PresentationController _presCont;

	public CrowdState (NPCController c) : base(c) 
	{
		//The player object is used to stare at, and also to check if it's looking at the NPC
		var Player = GameObject.FindGameObjectWithTag ("Player");
		_playerController = Player.GetComponent<PlayerController> ();
		_player = Player.transform;

		//Debug indicators for tracking NPC status
		_debugIndicator = _client.transform.GetComponentInChildren<SpriteRenderer> ();
		_angleIndicator = _client.transform.GetComponentInChildren<TextMesh> ();

		//The presentation controller is used to report interest to.
		_presCont = GameObject.FindGameObjectWithTag ("PresentationController").GetComponent<PresentationController>();

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
	
	public override void Enter()
	{
		_presCont.JoinAudience (this);
		if (_buying) {
			//Calculate how many fishes the player sold based on performance
			float avg = _presCont.GetAvgScore ();
			int BuyPrice = Mathf.RoundToInt (avg * 3);
			
			//Take fish from the player and give money
			_playerController.AddItem (new ItemModel ("Goud", BuyPrice, "Een gouden munt. Met zes hiervan kan ik met de veerboot."));
			_playerController.RemoveItem (new ItemModel ("Vis", 1, ""));
			//Load in the money pickup thingy
			GameObject money = GameObject.Instantiate(Resources.Load<GameObject>("MoneyPickup"));
			var text = money.GetComponentInChildren<TextMesh>();
			text.text = "+" + BuyPrice;
			_client.MyAnimator.SetTrigger("Give");
		}
	}
	
	public override void Exit()
	{
		_presCont.LeaveAudience (this);

		_angleIndicator.text = "";
		_debugIndicator.sprite = null;
	}
	
	public override bool Handle()
	{
		_client.TurnTo (_player.transform.position);
		if(_buying)
		{
			if(_client.MyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Give"))
		   	{
				_animStarted = true;
			}
			else if (_animStarted)
			{
				_buying = false;
				_animStarted = false;
				_client.Travel(_presCont.GetCrowdPosition(), this.Name);
			}
			return true;
		}

		//When in the crowd, stare at the player at all times
		//At least for now. When we don't have extensive NPC reactions yet.
		_client.LookAt(Camera.main.transform);

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


		float prevDistraction = _distraction;
		//If the player isn't staring well enough, the NPC gets distracted
		if (_stareScore > StareScoreLeast)
			_distraction -= Time.deltaTime * Speed;
		else
			_distraction += Time.deltaTime * Speed;

		_distraction = Mathf.Clamp (_distraction, 0, DistractionMax);

		//Play animations when doing bad or good
		var anim = _client.GetComponent<Animator> ();
		if (_distraction == 0 && prevDistraction != 0) {
			anim.SetTrigger ("Positive");
			_lastReaction = Time.time;
		} else if (_distraction >= 55 && prevDistraction < 55) {
			anim.SetTrigger ("Negative");
			_lastReaction = Time.time;
		} else if (Time.time - 10 > _lastReaction) {
			_lastReaction = Time.time;
			if(_distraction < 55)
				anim.SetTrigger("Positive");
			else
				anim.SetTrigger("Negative");
		}

		_presCont.UpdateDistraction (_distraction);

		//DrawDebug (angle);
		
		return true;
	}

	public bool Sell()
	{
		float avg = _presCont.GetAvgScore ();
		float interest = _presCont.TalkTime * avg;
		if (interest > _client.Personality.Demand) {
			_buying = true;
			_client.Travel(_presCont.transform.position + _presCont.transform.forward, this.Name);
			return true;
		}
		return false;
	}

	public bool IsReadyToLeave()
	{
		return !_buying;
	}

	void DrawDebug (float angle)
	{
		if(_debugIndicator == null)
			return;
		//Only show debug indicators when debug is active
		if (Debug.isDebugBuild) 
		{
			_angleIndicator.text = Mathf.Round (_stareScore).ToString () + " - " + Mathf.Round (_distraction).ToString () + " - " + Mathf.Round (angle).ToString ();
		}
		if (_distraction <= 0)
			_debugIndicator.sprite = _debugSprites [0];
		else if (_distraction < 55)
			_debugIndicator.sprite = _debugSprites [1];
		else
			_debugIndicator.sprite = _debugSprites [2];
	}
}