using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJSON;

public class CutsceneController : MonoBehaviour
{
	private Dictionary<string, List<IInteraction>> Cutscenes;

	void Start()
	{
		SerializeJson ();
	}

	void SerializeJson()
	{
		//Get all the presentation lines fitting with the current scene
		string scene = Application.loadedLevelName;
		string json = DataReader.GetAllText ("Assets/"+ scene +"cs.json");
		//Convert the lines data to a managable format
		JSONNode data = JSON.Parse (json);
		var cuts = data ["scenes"];
		Cutscenes = new Dictionary<string, List<IInteraction>> ();
		//Loop trough all sentences and place them in the _lines member
		for (int i = 0; i < cuts.Count; i++) 
		{
			var cutscenejs = cuts [i];

			string key = cutscenejs["SceneName"];
			var cutscene = new List<IInteraction>();
			
			//Loop through interactions
			for(int j = 0; j < cutscenejs.Count; j++)
			{
				var interaction = cutscenejs[j];
				string test = "";
				//Check what kind of interaction it is
				if(interaction["ScriptKey"] != null) //Event
				{
					test = interaction["ScriptKey"];
				}
				else if(interaction["NpcName"] != null) //NPCLine
				{
					test = interaction["NpcName"];
				}
				else if(interaction["Hint"] != null) //PlayerLine
				{
					test = interaction["Hint"];
				}
				var x = test;
			}

			Cutscenes.Add(key, cutscene);
		}
	}
}