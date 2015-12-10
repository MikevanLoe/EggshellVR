using UnityEngine;
using System;
using System.Collections;

public class MenuController : MonoBehaviour {
	/// <summary>
	/// The crafting menu state has a lot of things we want
	/// to edit in the Unity Editor. Thats why its public.
	/// </summary>
	public CraftingState craftingState;
	public Animator Animator;
	private PlayerController _player;

	private StateMachine<MenuController> _stateMachine;
	private bool running;

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
		Animator = GetComponent<Animator> ();
		_stateMachine.Add (craftingState);

		craftingState.Start ();
		try{
			var presentationState = new PresentationState (this);
			_stateMachine.Add (presentationState);
		}
		catch(NullReferenceException){}
		
		var inventoryState = new InventoryState (this);
		_stateMachine.Add (inventoryState);
		_stateMachine.Set ("CraftingState");
	}

	void Update () 
	{
		_stateMachine.Handle ();

		if (Input.GetButtonDown ("MenuLeft")) 
		{
			StartCoroutine ("Flip", false);
		}
		else if(Input.GetButtonDown("MenuRight"))
		{
			if (_stateMachine.TryNext () >= 0 && !running)
				StartCoroutine ("Flip", true);
		}
	}

	IEnumerator Flip(bool next)
	{
		//Check if we can flip right now
		if (running)
			yield break;
		if (next) 
		{
			if (_stateMachine.TryNext () < 0)
				yield break;
		}
		else
			if (_stateMachine.TryPrevious () < 0)
				yield break;

		running = true;
		if(next)
			Animator.SetTrigger("flip2");
		else
			Animator.SetTrigger("flip");

		yield return new WaitForSeconds(0.3f);
		if(next)
			_stateMachine.Next();
		else
			_stateMachine.Previous();
		yield return new WaitForSeconds(0.3f);
		running = false;
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
