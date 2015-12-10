using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PresentationController : MonoBehaviour {
	public enum PresentationState { Idle, Attracting, Active };

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
	public float Performance;
	public Transform MarketStand;
	public PresentationState presState;
	public TextMesh LineDisplay;
	public TextMesh ScoreDisplay;
	
	private List<GameObject> Audience;
	private VoiceResponse _voiceSystem;
	private Dictionary<string, List<List<Sentence>>> _lines;
	private string _curKey;
	private int _curId;
	private int _curSequence;
	private bool _presentationStarted;
	private float _timerStart;
	private float _presentationStart;
	private float _talkTime;
	private float _lookScore;
	private float _stressScore;
	private List<GameObject> _NPCs;
	private float _partDuration;

	void Start () 
	{
		//The voice response object is used to determine when the player is talking
		_voiceSystem = GameObject.FindGameObjectWithTag("GameController").GetComponent<VoiceResponse> ();
		if (_voiceSystem == null)
			throw new UnityException ("Game Controller has no VoiceResponse script attached to it!");

		GetLinesFromJSON ();
		_NPCs = new List<GameObject> ();
		
		Audience = new List<GameObject> ();
	}
	
	/// <summary>
	///Gets all the presentation lines from the JSON file
	/// </summary>
	void GetLinesFromJSON ()
	{
		//Get all the presentation lines
		string json = DataReader.GetAllText ("Assets/lines.json");
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
				_lines [key].Add (new List<Sentence> ());
			}
			//Add the line to the id
			_lines [key] [id].Add (new Sentence (sentence ["words"], sentence ["time"].AsFloat));
		}
	}

	void Update()
	{
		//Display the current talk time in the display for debug
		ScoreDisplay.text = (Mathf.Round(_talkTime * 10) / 10).ToString ();
		if (_voiceSystem.IsSpeaking ()) 
		{
			//Score is the amount of time the player was talking during the pitch phase
			_talkTime += Time.deltaTime;
			//Ask all NPCs if they want to come watch
			for (int i = 0; i < _NPCs.Count; i++) 
			{
				_NPCs [i].SendMessage ("MarketCall", _talkTime, 
				                       SendMessageOptions.DontRequireReceiver);
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
			LineDisplay.text = s.Words;
			_partDuration = s.Time;
			_timerStart = Time.time;
			_voiceSystem.StartListening (PitchSpoken);
		}
	}

	/// <summary>
	/// Handles the Active presentation state
	/// </summary>
	void ActiveState ()
	{
		//Part finished
		if (_timerStart + _partDuration < Time.time) {
			_voiceSystem.ForceStop ();
			//Show sentence
			Sentence s = GetNextSentence ();
			//TODO: End the presentation when there is no next line
			if (s != null) 
			{
				LineDisplay.text = s.Words;
				_partDuration = s.Time;
			}
			else 
			{
				GameObject.FindGameObjectWithTag ("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController> ().LockedInput = false;
				//TODO: Display end score and whatever
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
		//Random distance to stand
		float y = Random.Range (pSettings.CrowdRadius / 10, pSettings.CrowdRadius);
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
		_lookScore += (distraction - _lookScore) / Audience.Count;
	}

	public void JoinAudience(GameObject NPC)
	{
		if (!Audience.Contains (NPC))
			Audience.Add (NPC);
	}

	public float GetConvScore()
	{
		float totalTime = Time.time - _presentationStart;
		if (totalTime <= 0)
			return 1;
		float score = 1 / totalTime * _talkTime;
		score *= 2;
		score = Mathf.Clamp01 (score);
		return score;
	}

	public float GetLookScore()
	{
		return (100 - _lookScore) / 100;
	}

	/// <summary>
	/// Called when the player is inside of the presentation area.
	/// </summary>
	public void DetectOn()
	{
		LineDisplay.text = "Detect ON!";

		presState = PresentationState.Attracting;
		//Show sentence
		Sentence s = GetNextSentence();
		LineDisplay.text = s.Words;
		_partDuration = s.Time;

		_timerStart = Time.time;
		_presentationStart = Time.time;

		_voiceSystem.StartListening(PitchSpoken);
		_NPCs = new List<GameObject>( GameObject.FindGameObjectsWithTag("NPC") ); //Get all NPCs
		
		GameObject.FindGameObjectWithTag ("Player")
			.GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController> ()
				.LockedInput = true;

	}

	/// <summary>
	/// Called when player leaves the presentation area
	/// </summary>
	public void DetectOff()
	{
		LineDisplay.text = "Detect OFF!";
		//Tell all NPCs the show is over
		for(int i = 0; i < _NPCs.Count; i++)
		{
			if(_NPCs[i].GetComponent<NPCController>() != null)
				_NPCs[i].SendMessage ("MarketCall", 0f, SendMessageOptions.DontRequireReceiver); //Send 0 as performance so everyone loses interest instantly
		}
	}
}
