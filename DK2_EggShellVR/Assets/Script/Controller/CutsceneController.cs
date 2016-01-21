using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class CutsceneController : MonoBehaviour
{
	private Dictionary<string, SceneModel> _cutscenes;
	private List<Interaction> _curScene;
	private Queue<string> _sceneQueue;
	private GameObject _buttonIcon;
	private MusicController _musicCont;
	private float _interactionDelay;
	private int _curInteraction = -1;
	private Transform _player;
	private Vector3 _sceneStartPos;
	private float _sceneRange;
	private string _waitButton;
	private bool _waitInput;

	void Start()
	{
		SerializeJson ();
		_sceneQueue = new Queue<string> ();
		_player = GameObject.FindGameObjectWithTag ("Player").transform;
		_buttonIcon = GameObject.Find ("ButtonIcon");
		_buttonIcon.SetActive(false);
		_musicCont = GameObject.Find ("Music").GetComponent<MusicController>();
	}

	void Update()
	{
		if (_curScene == null) {
			if(_sceneQueue.Count > 0)
			{
				PlayCutscene(_sceneQueue.Dequeue());
				return;
			}
			else
				return;
		}

		//Cancel scene when out of range;
		if(Vector3.SqrMagnitude(_sceneStartPos - _player.position) > _sceneRange)
		{
			if(_curInteraction > 0)
				_curScene [_curInteraction-1].Finish ();
			if(_curInteraction >= 0)
				_curScene [_curInteraction].Cancel();
			//By setting curscene to null next update no scene will be played
			_curScene = null;
			_curInteraction = -1;
			_musicCont.SetVolume (-1);
			return;
		}

		//If delay time isn't over yet
		if (Time.time < _interactionDelay)
	    {
			//Check if we are waiting for input
			if (_waitInput) {
				//If we are, check if we are waiting for a specific input
				if (_waitButton != null) {
					//If that specific input is pressed, set delay to zero so the next scene plays on the next update (THIS IS IMPORTANT)
					if (Input.GetButtonDown (_waitButton))
						_interactionDelay = 0;
					else
						return;
				}
				//If no specific input. Listen for any input
				else if (Input.anyKeyDown)
					_interactionDelay = 0;
			}
			return;
		}

		if(_curInteraction >= 0)
			_curScene [_curInteraction].Finish ();

		//Go to next interaction
		_curInteraction++;

		//If there are no interactions left, end the scene
		if (_curScene.Count <= _curInteraction) 
		{
			_curScene = null;
			_curInteraction = -1;
			_musicCont.SetVolume (-1);
			return;
		}
		
		_curScene [_curInteraction].Execute ();

		_waitInput = false;
		_buttonIcon.SetActive(false);

		if (_curScene [_curInteraction] is PlayerLine)
			_interactionDelay = Mathf.Infinity;			//Player lines don't end until they're spoken
		else {
			if (_curScene [_curInteraction].Duration >= 0)
				_interactionDelay = Time.time + _curScene [_curInteraction].Duration;
			//If negative delay set, wait until input
			else
			{
				//Set delay to infinity
				_interactionDelay = Mathf.Infinity;
				_waitInput = true;
				_waitButton = _curScene[_curInteraction].WaitButton;
				if(_waitButton == null)
					_buttonIcon.SetActive(true);
			}
		}
	}

	void SerializeJson()
	{
		//Get all the presentation lines fitting with the current scene
		string scene = Application.loadedLevelName;
		string json = DataReader.GetAllText ("Merchanted_Data/"+ scene +"cs.json");
		//Convert the lines data to a managable format
		JSONNode data = JSON.Parse (json);

		var cutsjs = data ["scenes"];
		_cutscenes = new Dictionary<string, SceneModel> ();
		//Loop trough all sentences and place them in the _lines member
		for (int i = 0; i < cutsjs.Count; i++) 
		{
			var cutscenejs = cutsjs [i];

			string key = cutscenejs["SceneName"];
			var cutscene = new List<Interaction>();
			var interactions = cutscenejs["Interaction"];
			
			//Loop through interactions
			for(int j = 0; j < interactions.Count; j++)
			{
				var interaction = interactions[j];
				//Check what kind of interaction it is
				Interaction part = null;
				switch (interaction["Type"])
				{
				case "Event":
					part = SceneEvent.GetScript(interaction["ScriptKey"], interaction["Name"], interaction["Duration"].AsFloat, interaction["Text"]);
					break;
				case "NPCLine":
					part = new NPCLine(interaction["NpcName"], interaction["VoiceKey"], interaction["Subtitles"], interaction["Duration"].AsFloat);
					break;
				case "PlayerLine":
					part = new PlayerLine(interaction["Hint"], interaction["Duration"].AsFloat, GoToNextPart);
					break;
				case "Monologue":
					part = new Monologue(interaction["VoiceKey"], interaction["Subtitles"], interaction["Duration"].AsFloat);
					break;
				}
				if(part == null)
					throw new Exception("JSON format exception in cutscene controller. Unknown type used. Input: " + interaction["Type"]);
				if(part != null)
				{
					if(interaction["WaitButton"] != null)
						part.WaitButton = interaction["WaitButton"];
					cutscene.Add(part);
				}
			}
			_cutscenes.Add(key, new SceneModel(cutscenejs["Range"].AsFloat ,cutscene));
		}
	}

	public bool PlayCutscene(string key)
	{
		if (_curScene != null || !_cutscenes.ContainsKey(key))
			return false;
		_sceneStartPos = _player.position;
		_curScene = _cutscenes [key].Interactions;
		_sceneRange = _cutscenes [key].Range;
		_interactionDelay = 0;
		_musicCont.SetVolume (0.2f);
		return true;
	}

	public bool QueueScene(string key)
	{
		if (!_cutscenes.ContainsKey (key))
			return false;
		_sceneQueue.Enqueue (key);
		return true;
	}

	public void GoToNextPart()
	{
		_interactionDelay = 0;
	}

	public bool IsPlaying()
	{
		//If the scene is NOT null, scene IS playing
		return _curScene != null;
	}
}