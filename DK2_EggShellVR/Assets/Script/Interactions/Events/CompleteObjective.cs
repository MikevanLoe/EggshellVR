using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CompleteObjective : SceneEvent
{
	private string _objective;

	public CompleteObjective (string name)
	{
		_objective = name;
	}

	public override void Execute()
	{
		var menu = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().InventoryObject.GetComponent<MenuController>();
		menu.Questlog.CompleteObjective (_objective);
	}
}