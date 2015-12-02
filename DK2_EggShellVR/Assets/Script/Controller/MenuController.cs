using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
	/// <summary>
	/// The crafting menu state has a lot of things we want
	/// to edit in the Unity Editor. Thats why its public.
	/// </summary>
	public CraftingState craftingState;
	private PlayerController _player;

	private StateMachine<MenuController> _stateMachine;

	public PlayerController Player 
	{
		get;
		private set;
	}

	/// <summary>
	/// Initialize the menu
	/// </summary>
	void Start () 
	{
		Player = transform.parent.GetComponent<PlayerController>();
		_stateMachine = new StateMachine<MenuController> ();

		craftingState.Start ();
		var presentationState = new PresentationState (this);

		_stateMachine.Add (craftingState);
		_stateMachine.Add (presentationState);
		_stateMachine.Set ("PresentationState");
	}

	void Update () 
	{
		_stateMachine.Handle ();
	}

	//Refresh the inventory every time the menu is opened
	public void OnEnable()
	{
		//Becuase OnEnable is called before start, we ignore it until start was called
		if (Player == null)
			return;
		_stateMachine.GetCurState().Enter();
	}
}
