using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
	public CraftingState craftingState;
	private PlayerController _player;

	private StateMachine<MenuController> _stateMachine;

	public PlayerController Player {
		get{ return _player; }
		private set{ _player = value; }
	}

	/// <summary>
	/// Initialize the menu
	/// </summary>
	void Init () {
		Player = transform.parent.GetComponent<PlayerController>();
		_stateMachine = new StateMachine<MenuController> ();

		craftingState.Start ();

		_stateMachine.Add (craftingState);
		_stateMachine.Set ("CraftingState");
	}

	void Update () {
		_stateMachine.Handle ();
	}

	//Refresh the inventory every time the menu is opened
	public void OnEnable()
	{
		//Becuase OnEnable is called before start, we have to initialize on the first
		//time OnEnable is called instead.
		if (Player == null)
			Init ();
		_stateMachine.GetCurState().Enter();
	}
}
