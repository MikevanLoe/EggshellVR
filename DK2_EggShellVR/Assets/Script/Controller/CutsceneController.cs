using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJSON;

public class CutsceneController : MonoBehaviour
{
	private Dictionary<string, SceneModel> _cutscenes;
	private List<Interaction> _curScene;
	private float _interactionDelay;
	private int _curInteraction;
	private Transform _player;
	private Vector3 _sceneStartPos;
	private float _sceneRange;

	void Start()
	{
		SerializeJson ();
		_player = GameObject.FindGameObjectWithTag ("Player").transform;
	}

	void Update()
	{
		if (_curScene == null)
			return;

		//Cancel scene when out of range;
		if(Vector3.SqrMagnitude(_sceneStartPos - _player.position) > _sceneRange)
		{
			_curScene [_curInteraction].Cancel();
			//By setting curscene to null next update no scene will be played
			_curScene = null;
			_curInteraction = 0;
			return;
		}

		if (_interactionDelay > Time.time)
			return;

		//Go to next interaction
		_curInteraction++;
		_curScene [_curInteraction].Execute ();
		if (_curScene [_curInteraction] is PlayerLine)
			_interactionDelay = Mathf.Infinity;			//Player lines don't end until they're spoken
		else
			_interactionDelay = Time.time + _curScene [_curInteraction].Duration;
		if (_curScene.Count <= _curInteraction) 
		{	
			_curScene = null;
			_curInteraction = 0;
		}
	}

	void SerializeJson()
	{
		//Get all the presentation lines fitting with the current scene
		string scene = Application.loadedLevelName;
		string json = DataReader.GetAllText ("Assets/"+ scene +"cs.json");
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
					part = Event.GetScript(interaction["ScriptKey"], interaction["Duration"].AsFloat);
					break;
				case "NPCLine":
					part = new NPCLine(interaction["NpcName"], interaction["VoiceKey"], interaction["Duration"].AsFloat);
					break;
				case "PlayerLine":
					part = new PlayerLine(interaction["Hint"], interaction["Duration"].AsFloat, GoToNextPart);
					break;
				}
				if(part == null)
					throw new Exception("JSON format exception in cutscene controller. Unknown type used. Input: " + interaction["Type"]);
				if(part != null)
					cutscene.Add(part);
			}
			_cutscenes.Add(key, new SceneModel(cutscenejs["Range"].AsFloat ,cutscene));
		}
	}

	public void PlayCutscene(string key)
	{
		if (_curScene != null || !_cutscenes.ContainsKey(key))
			return;
		_sceneStartPos = _player.position;
		_curScene = _cutscenes [key].Interactions;
		_sceneRange = _cutscenes [key].Range;
		_interactionDelay = Time.time + _curScene [_curInteraction].Duration;
		_curScene [_curInteraction].Execute ();
	}

	public void GoToNextPart()
	{
		_interactionDelay = 0;
	}
}