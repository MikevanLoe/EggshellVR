using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PresentationController : MonoBehaviour {

	public bool PlayerIsClose;
	public bool PresentationStarted;
	
	[System.Serializable]
	public class PresentationSettings
	{
		public float CrowdRadius;
		public float CrowdAngle;
		public float Accuracy;
		public float ScorePerLine;
		public float MaxScore = 100;
	}

	public PresentationSettings pSettings = new PresentationSettings();
	public float Performance;
	public Transform MarketStand;
	
	[HideInInspector]
	public List<GameObject> Audience;

	private float _oldPerformance = -1;
	private TextMesh _lineDisplay;
	private VoiceResponse _voiceSystem;
	private Dictionary<string, List<List<Sentence>>> _lines;
	private string _curKey;
	private int _curId;
	private int _curSequence;
	private float _filler;

	// Use this for initialization
	void Start () 
	{
		//The line display is the text mesh that shows the text the player has to say
		_lineDisplay = GetComponentInChildren<TextMesh> ();
		//The voice response object is used to determine when the player is talking
		_voiceSystem = GetComponent<VoiceResponse> ();
		if (_lineDisplay == null)
			throw new UnityException ("Presentation Controller found no TextMesh in children!");
		if (_voiceSystem == null)
			throw new UnityException ("Presentation Controller has no VoiceResponse script attachd to it!");

		//This factor is used to round the number later in code between 0 and -1
		_filler = 1 / pSettings.Accuracy - 1;

		GetLinesFromJSON ();
	}

	//Gets all the presentation lines from the JSON file
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

	public void SentenceSpoken (float deviation)
	{
		//Gets a number between 1 and -1 representing the distance from
		//the accuracy to perfection and the maximum deviation.
		float factor = 1 - 1 / pSettings.Accuracy * deviation;
		//If the factor is negative, divide it by a factor to round it to -1 at max
		if (factor < 0)
			factor /= _filler;
		float score = pSettings.ScorePerLine * factor;
		Performance = Mathf.Clamp(Performance + score, 0, pSettings.MaxScore);
		
		//Get the next sentence and send it to the voice controller
		Sentence s = GetNextSentence ();

		//TODO: End the presentation when there is no next line
		if (s == null) 
		{
			_lineDisplay.text = "";
			Performance = 0;
		}
		else
		{
			_voiceSystem.SetSentence (s);
			_lineDisplay.text = s.Words; //Display the text on the display area
		}
		
		//Bring in audience
		if (_oldPerformance != Performance) //If the performance score changed
		{
			var npcs = GameObject.FindGameObjectsWithTag("NPC"); //Get all NPCs
			for(int i = 0; i < npcs.Length; i++)
			{
				if(npcs[i].GetComponent<NPCController>() != null)
					npcs[i].SendMessage ("MarketCall", Performance); //Ask NPC if they want to/can watch
			}
		}
		_oldPerformance = Performance; //Store the performance for comparison later
	}
	
	//Get the next sentence in sequence
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
			_curKey = "start";
			break;
		case "start":
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
			_curKey = "peratio";
			break;
		default:
			return null;
		}
		//Get random id from possible id's
		_curId = Random.Range (0, _lines [_curKey].Count - 1);
		//Sequence always starts at 0
		_curSequence = 0;
		return _lines [_curKey] [_curId] [_curSequence];
	}

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

	public void DetectOn()
	{
		PlayerIsClose = true;
		
		//Show sentence
		Sentence s = GetNextSentence();
		_lineDisplay.text = s.Words;
		_voiceSystem.SetSentence (s);
		
		//Chain player to position
	}
	
	public void DetectOff()
	{
		PlayerIsClose = false;
	}
}
