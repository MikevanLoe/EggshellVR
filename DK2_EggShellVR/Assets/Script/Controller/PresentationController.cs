using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class PresentationController : MonoBehaviour {
	public enum PresentationState { Idle, Attracting, Active };
	public PresentationState presState;

	[System.Serializable]
	public class PresentationSettings
	{
		public float CrowdRadius = 3;
		public float CrowdAngle = 15;
		public float Accuracy = 0.2f;
		public float ScorePerLine = 10;
		public float MaxScore = 100;
	}
	public PresentationSettings pSettings = new PresentationSettings();
	
	public Transform MarketStand;
	public GameObject PresMenuObject;
	public float TalkTime;

	private HeartrateReader _HRModule;
	private TextMesh _lineDisplay;
	private TextMesh _scoreDisplay;
	private List<GameObject> _NPCs;
	private List<CrowdState> _audience;
	private VoiceResponse _voiceSystem;
	private Dictionary<string, List<List<Sentence>>> _lines;
	private string _curKey;
	private int _curId;
	private int _curSequence;
	private bool _presentationStarted;
	private float _timerStart;
	private float _partDuration;
	private float _presentationStart;
	private float _partTalkTime;
	private float _lookScore;
	private float _sellMoment;

	void Start ()
	{
		//The voice response object is used to determine when the player is talking
		var gc = GameObject.FindGameObjectWithTag ("GameController");
		_voiceSystem = gc.GetComponent<VoiceResponse> ();
		if (_voiceSystem == null)
			throw new UnityException ("Game Controller has no VoiceResponse script attached to it!");

		_HRModule = gc.GetComponent<HeartrateReader> ();

		GetLinesFromJSON ();
		_NPCs = new List<GameObject> ();
		
		_audience = new List<CrowdState> ();

		_lineDisplay = PresMenuObject.transform.GetChild (1).GetComponent<TextMesh> ();
		_scoreDisplay = PresMenuObject.transform.GetChild (2).GetComponent<TextMesh> ();
	}
	
	/// <summary>
	///Gets all the presentation lines from the JSON file
	/// </summary>
	void GetLinesFromJSON ()
	{
		//Get all the presentation lines
		string json = DataReader.GetAllText ("Merchanted_Data/lines.json");
		//Convert the lines data to a managable format
		JSONNode data = JSON.Parse (json);
		var sentences = data ["lines"];
		_lines = new Dictionary<string, List<List<Sentence>>> ();
		//Loop trough all sentences and place them in the _lines member
		for (int i = 0; i < sentences.Count; i++) 
		{
			var sentence = sentences [i];
			string key = sentence ["key"];
			//If the list doesn't yet have the key, add it
			if (!_lines.ContainsKey (key)) 
			{
				_lines.Add (key, new List<List<Sentence>> ());
			}
			//If the key doesn't yet have the id, add it
			int id = sentence ["id"].AsInt;
			if (_lines [key].Count >= id) 
			{
				_lines [key].Add (new List<Sentence> ());			}
			//Add the line to the id
			_lines [key] [id].Add (new Sentence (sentence ["words"], sentence ["time"].AsFloat));
		}
	}

	void Update()
	{
		//If the presentation isn't active but there's still an audience, tell them to leave
		if ( presState == PresentationState.Idle) 
		{
			if( _audience.Count <= 0 )
				return;
			for (int i = 0; i < _NPCs.Count; i++) {
				_NPCs [i].SendMessage ("MarketCall", -1, SendMessageOptions.DontRequireReceiver);
			}
			return;
		}
		//At any time while the presentation is going, check the score
		else if (_voiceSystem.IsSpeaking ()) 
		{
			//Keep track of the time the player has been talking
			TalkTime += Time.deltaTime;
			_partTalkTime += Time.deltaTime;
			//Ask all NPCs if they want to come watch
			for (int i = 0; i < _NPCs.Count; i++) 
			{
				_NPCs [i].SendMessage ("MarketCall", TalkTime, SendMessageOptions.DontRequireReceiver);
			}
			if (Debug.isDebugBuild) 
			{
				//Display the current talk time in the display for debug
				_scoreDisplay.text = (Mathf.Round (TalkTime * 10) / 10).ToString ();
			}
		}
		if (presState == PresentationState.Attracting) 
		{
			PitchState ();
		}
		else if (presState == PresentationState.Active)
		{
			ActiveState ();
		}
	}

	/// <summary>
	/// Handles the pitch state
	/// </summary>
	void PitchState ()
	{
		if (_timerStart + _partDuration <= Time.time) 
		{
			//End the attraction state
			//Add any points the player was still going to get.
			_voiceSystem.ForceStop ();

			presState = PresentationState.Active;
			//Show sentence
			Sentence s = GetNextSentence ();
			_lineDisplay.text = s.Words;
			_partDuration = s.Time;
			_partTalkTime = 0;
			_timerStart = Time.time;
			_voiceSystem.StartListening (PitchSpoken);
		}
	}

	/// <summary>
	/// Handles the Active presentation state
	/// </summary>
	void ActiveState ()
	{
		//If a sell moment is set and sell moment has been reached
		if(_sellMoment >= 0 && Time.time >= _sellMoment && _audience.Count > 0)
		{
			_sellMoment = -1;
			_audience[Random.Range(0, _audience.Count-1)].Sell();
		}
		//Part finished
		if (_timerStart + _partDuration < Time.time) 
		{
			_voiceSystem.ForceStop ();
			//Show sentence
			Sentence s = GetNextSentence ();
			//If the next sentence is null, the presentation is OVER!!!
			if (s == null) 
			{
				//Unlock the players controls
				var playerObj = GameObject.FindGameObjectWithTag ("Player");
				playerObj.GetComponent<RigidbodyFirstPersonController> ().LockedInput = false;

				var player = playerObj.GetComponent<PlayerController> ();
				player.IgnoreLook = false;
				player.InvLock = false;

//				player.transform.FindChild ("Menu").GetComponent<MenuController> ().UnlockMenu ();

				//Stop the presentation
				presState = PresentationState.Idle;

				//Play the good or bad cutscene
				var cutscenecont = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CutsceneController> ();
				if (cutscenecont.IsPlaying ())
					throw new UnityException ("Cutscene playing at end of presentation.");
				if (player.HasItem ("Goud", 6))
					cutscenecont.PlayCutscene ("PresGood");
				else
					cutscenecont.PlayCutscene ("PresBad");
				return;
			}
			_lineDisplay.text = s.Words;
			_partDuration = s.Time;
			_partTalkTime = 0;

			if(_curKey == "narratio" || _curKey == "argumentatio")
			{
				//Sell a fish at a random moment between the start of the part and the end;
				_sellMoment = Time.time + Random.Range(0, s.Time);
			}
			else if (_curKey == "peroratio")
			{
				//Sell near the end
				_sellMoment = Time.time;
			}
			
			_timerStart = Time.time;
			_voiceSystem.StartListening (PitchSpoken);
		}
	}

	/// <summary>
	/// Get the next sentence in sequence
	/// </summary>
	/// <returns>The next sentence.</returns>
	private Sentence GetNextSentence ()
	{
		//Try to get the next line in the sequence
		if (_curKey != null && _lines [_curKey] [_curId].Count - 1 > _curSequence) 
		{
			_curSequence++;
			return _lines [_curKey] [_curId][_curSequence];
		}
		//If there is no line next in sequence, continue to the next part of the presentation
		switch (_curKey) {
		case null:
			_curKey = "exordium";
			break;
		case "exordium":
			_curKey = "narratio";
			break;
		case "narratio":
			_curKey = "propositio";
			break;
		case "propositio":
			_curKey = "argumentatio";
			break;
		case "argumentatio":
			_curKey = "peroratio";
			break;
		default:
			_curKey = null;
			return null;
		}
		//Get random id from possible id's
		if (_lines.ContainsKey (_curKey))
			_curId = Random.Range (0, _lines [_curKey].Count - 1);
		else 
		{
			throw new UnityException("Lines did not contain part: " + _curKey);
		}
		//Sequence always starts at 0
		_curSequence = 0;
		return _lines [_curKey] [_curId] [_curSequence];
	}

	/// <summary>
	/// Returns a random position in the crowd for an NPC to stand in
	/// </summary>
	/// <returns>The crowd position.</returns>
	public Vector3 GetCrowdPosition()
	{
		//Random distance to stand, preferably far away
		float y = Random.Range (pSettings.CrowdRadius / 10 * 8, pSettings.CrowdRadius);
		//Maximum crowd width at distance
		float x = Mathf.Tan (pSettings.CrowdAngle) * y;
		//Random point in width
		x = Random.Range (-x, x);
		//Translate local point to global
		var point = MarketStand.position + MarketStand.forward * y + MarketStand.right * x;

		//TODO: Push out of other objects

		return point;
	}

	public void PitchSpoken(float duration)
	{
		if(_timerStart + _partDuration > Time.time)
			_voiceSystem.StartListening (PitchSpoken);
	}

	/// <summary>
	/// Change the look score based on the NPCs distraction
	/// </summary>
	/// <param name="distraction">Distraction.</param>
	public void UpdateDistraction(float distraction)
	{
		//Score approaches distraction, based on crowd size
		_lookScore += (distraction - _lookScore) / _audience.Count;
	}
	
	public void JoinAudience(CrowdState NPC)
	{
		if (!_audience.Contains (NPC))
			_audience.Add (NPC);
	}
	
	public void LeaveAudience(CrowdState NPC)
	{
		if (_audience.Contains (NPC))
			_audience.Remove (NPC);
	}
	
	public float GetConvScore()
	{
		float totalTime = Time.time - _presentationStart;
		if (totalTime <= 0)
			return 1;
		float score = 1 / totalTime * TalkTime;
		score *= 1.5f;
		score = Mathf.Clamp01 (score);
		return score;
	}
	
	public float GetPartConvScore()
	{
		float totalTime = Time.time - _presentationStart;
		if (totalTime <= 0)
			return 1;
		float score = 1 / _partDuration * _partTalkTime;
		score *= 2;
		score = Mathf.Clamp01 (score);
		return score;
	}
	
	public float GetLookScore()
	{
		return (100 - _lookScore) / 100;
	}
	
	public float GetTimerFactor()
	{
		return Mathf.Clamp01( 1 / _partDuration * (Time.time - _timerStart) );
	}

	public float GetAvgScore()
	{
		float score1 = 1 - _HRModule.CalculateMeterScale();
		float score2 = GetLookScore();
		float score3 = GetConvScore();
		return (score1 + score2 + score3) / 3;
	}

	/// <summary>
	/// Called when the player is inside of the presentation area.
	/// </summary>
	public void DetectOn()
	{
		//Make sure player has enough fish to sell
		var player = GameObject.FindGameObjectWithTag ("Player");
		if (!player.GetComponent<PlayerController> ().HasItem ("Vis", 3)) {
			return;
		}

		presState = PresentationState.Attracting;
		//Show sentence
		Sentence s = GetNextSentence();
		_lineDisplay.text = s.Words;
		_partDuration = s.Time;
		_partTalkTime = 0;
		TalkTime = 0;

		_timerStart = Time.time;
		_presentationStart = Time.time;

		_voiceSystem.StartListening(PitchSpoken);
		_NPCs = new List<GameObject>( GameObject.FindGameObjectsWithTag("NPC") ); //Get all NPCs

		player.GetComponent<RigidbodyFirstPersonController> ().LockedInput = true;
		player.GetComponent<PlayerController> ().IgnoreLook = true;
		player.transform.FindChild ("Menu").GetComponent<MenuController>().LockMenu(PresMenuObject);

	}

	/// <summary>
	/// Called when player leaves the presentation area
	/// </summary>
	public void DetectOff()
	{
		var player = GameObject.FindGameObjectWithTag ("Player");
		var menu = player.transform.FindChild ("Menu").GetComponent<MenuController>();
		menu.UnlockMenu ();
	}
}
