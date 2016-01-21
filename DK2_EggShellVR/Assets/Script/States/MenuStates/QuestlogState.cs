using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.Collections.Generic;
using System.Linq;

public class QuestlogState : MenuState
{
	private List<string> _objectives;
	private List<string> _completedObjectives;
	private List<GameObject> _spawnedObjects;
	private GameObject _objectiveObject;
	private GameObject _completedObjectiveObject;

	public QuestlogState (MenuController c) : base(c)
	{
		//Find Crafting menu
		Menu = _client.transform.FindChild ("Questlog Menu").gameObject;

		_objectives = new List<string> ();
		_completedObjectives = new List<string> ();

		//Find the default objective object and clean it up
		_objectiveObject = Menu.transform.FindChild ("Left/Objective").gameObject;
		_completedObjectiveObject = Menu.transform.FindChild ("Right/Objective").gameObject;
		_objectiveObject.SetActive (false);
		_completedObjectiveObject.SetActive (false);

		_spawnedObjects = new List<GameObject> ();
	}
	
	public override void Enter()
	{
		base.Enter ();
		Vector3 pos = _objectiveObject.transform.localPosition;
		DrawObjectives (_objectives, pos, _objectiveObject);
		pos.x += 2.17f;
		DrawObjectives (_completedObjectives, pos, _completedObjectiveObject);
	}

	public override void Exit()
	{
		base.Exit ();
		_spawnedObjects.ForEach (o => GameObject.Destroy(o));
		_spawnedObjects.Clear ();
	}

	/// <summary>
	/// Draws the objectives.
	/// </summary>
	/// <param name="list">The objectives to be drawn.</param>
	/// <param name="pos">Position.</param>
	/// <param name="rot">Rot.</param>
	private void DrawObjectives(List<String> list, Vector3 pos, GameObject clone)
	{
		foreach (string objective in list) 
		{
			GameObject obj = GameObject.Instantiate(clone) as GameObject;
			obj.SetActive(true);
			obj.transform.parent = _objectiveObject.transform.parent;
			obj.transform.localPosition = pos;
			obj.transform.localRotation = _objectiveObject.transform.localRotation;
			obj.transform.localScale = _objectiveObject.transform.localScale;

			var textTrans = obj.transform.GetChild(0);
			TextMesh text = textTrans.GetComponent<TextMesh>();
			text.text = objective;
			pos.z -= 0.3f;

			_spawnedObjects.Add (obj);
		}
	}

	public void AddObjective(string objective)
	{
		_objectives.Add (objective);
	}

	public bool CompleteObjective(string objective)
	{
		if (!_objectives.Contains (objective))
			return false;
		_objectives.Remove (objective);
		_completedObjectives.Add (objective);
		return true;
	}
}