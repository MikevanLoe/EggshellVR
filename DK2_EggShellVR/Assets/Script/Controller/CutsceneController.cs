using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJSON;

public class CutsceneController : MonoBehaviour
{
	private Dictionary<string, List<Interaction>> cutscenes;
	private List<Interaction> curScene;
	private float interactionDelay;
	private int curInteraction;

	void Start()
	{
		SerializeJson ();
	}

	void Update()
	{
		if (curScene == null)
			return;
		if (interactionDelay > Time.time)
			return;
		curScene [curInteraction].Execute ();
		interactionDelay = Time.time + curScene [curInteraction].Duration;
		curInteraction++;
		if (curScene.Count <= curInteraction) 
		{	
			//By setting curscene to null next update no scene will be played
			curScene = null;
			curInteraction = 0;
		}
	}

	void SerializeJson()
	{
		//Get all the presentation lines fitting with the current scene
		string scene = Application.loadedLevelName;
		string json = DataReader.GetAllText ("Assets/"+ scene +"cs.json");
		//Convert the lines data to a managable format
		JSONNode data = JSON.Parse (json);
		var cuts = data ["scenes"];
		cutscenes = new Dictionary<string, List<Interaction>> ();
		//Loop trough all sentences and place them in the _lines member
		for (int i = 0; i < cuts.Count; i++) 
		{
			var cutscenejs = cuts [i];

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
					break;
				}
//				if(part == null)
//					throw new Exception("JSON format exception in cutscene controller. Unknown type used.");
				if(part != null)
					cutscene.Add(part);
			}
			cutscenes.Add(key, cutscene);
		}
	}

	public void PlayCutscene(string key)
	{
		if(!cutscenes.ContainsKey(key))
			return;

		curScene = cutscenes [key];
	}
}