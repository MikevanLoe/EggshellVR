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
	public GameObject LockedMenuObject;

	private State<MenuController> _preLockState;
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
		
		var inventoryState = new InventoryState (this);
		_stateMachine.Add (inventoryState);
		_stateMachine.Set ("CraftingState");

		//Disable menu after initializing
		gameObject.SetActive (false);
	}
	
	//Refresh the inventory every time the menu is opened
	public void OnEnable()
	{
		//Becuase OnEnable is called before start, we ignore it until start was called
		if (Player == null)
			return;
		var state = _stateMachine.GetCurState ();
		if(state != null)
			state.Enter();
	}

	void Update ()
	{
		if (_stateMachine.GetCurState () == null)
			return;

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

	/// <summary>
	/// Force the menu to only show a certain object and nothing else
	/// </summary>
	/// <param name="menuObject">Menu object.</param>
	public void LockMenu(GameObject menuObject)
	{
		menuObject.SetActive (true);
		LockedMenuObject = menuObject;
		_preLockState = _stateMachine.GetCurState ();
		_stateMachine.Set ("");
		Player.ForceOpenInv (false, true);
	}

	/// <summary>
	/// Unlocks the menu and hides the lock object. Does nothing if the menu wasn't locked
	/// </summary>
	public void UnlockMenu()
	{
		//Check if we are actually locked
		if (LockedMenuObject == null)
			return;

		LockedMenuObject.SetActive (false);
		LockedMenuObject = null;
		_stateMachine.Set (_preLockState);
		Player.ForceCloseInv ();
	}
}