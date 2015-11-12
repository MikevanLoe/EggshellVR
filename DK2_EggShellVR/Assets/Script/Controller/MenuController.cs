using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
	public CraftingState craftingState;

	[HideInInspector]
	public PlayerController Player;

	private StateMachine<MenuController> _stateMachine;
	[SerializeField]
	private Transform SelectionHighlight;

	// Use this for initialization
	void Start () {
		Player = transform.parent.GetComponent<PlayerController>();
		_stateMachine = new StateMachine<MenuController> ();

		craftingState.Start ();

		_stateMachine.Add (craftingState);

		_stateMachine.Set ("CraftingState");
	}
	
	// Update is called once per frame
	void Update () {
		_stateMachine.Handle ();
	}
	
	public void SetSelectionPosition(Transform t)
	{
		SelectionHighlight.position = t.position;
		SelectionHighlight.rotation = t.rotation;
	}
	
	public void SetSelectionPosition(Vector3 pos)
	{
		SelectionHighlight.position = pos;
	}
	
	public void SetSelectionPosition(Vector3 pos, Quaternion rot)
	{
		SelectionHighlight.position = pos;
		SelectionHighlight.rotation = rot;
	}
}
