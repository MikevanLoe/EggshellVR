using UnityEngine;
using System.Collections.Generic;

public class TravelState : State<NPCController>
{
	public Vector3 Destination;
	public string NextState;
	public float Speed = 10;

	private Animation NPCAnim;
	
	public TravelState (NPCController c) : base(c)
	{
		NPCAnim = c.GetComponent<Animation> ();
	}
	
	public TravelState (NPCController c, Vector3 dest) : base(c) 
	{
		Destination = dest;
		NPCAnim = c.GetComponent<Animation> ();
	}
	
	public override void Enter()
	{
		NPCAnim.clip = NPCAnim.GetClip ("celebration2");
		_client.transform.LookAt (Destination, Vector3.up);
	}
	
	public override bool Handle()
	{
		//Fake Y
		var pos = _client.transform.position;
		pos.y = Destination.y;

		//Check if arrived at destination yet
		if (Vector3.Distance (pos, Destination) > 1) {
			//Move directly at the destination, ignoring Y
			float y = _client.transform.position.y;
			var newPos = _client.transform.position + (Destination - _client.transform.position).normalized * Speed * Time.deltaTime;
			newPos.y = y;
			_client.transform.position = newPos;
		} else {
			NPCAnim.clip = NPCAnim.GetClip ("idle");
			_client.NPCStateMachine.Set(NextState);
		}
		return true;
	}
}