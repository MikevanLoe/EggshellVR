using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AddObjective : SceneEvent
{
	private string _objective;

	public AddObjective (string name)
	{
		_objective = name;
	}

	public override void Execute()
	{
		var menu = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().InventoryObject.GetComponent<MenuController>();
		menu.Questlog.AddObjective (_objective);
	}
}