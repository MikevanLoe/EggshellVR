using UnityEngine;
using System.Collections.Generic;

public class TravelState : State<NPCController>
{
	public Vector3 Destination;
	public string NextState;
	public float Speed = 10;
	
	public TravelState (NPCController c) : base(c){}
	
	public TravelState (NPCController c, Vector3 dest) : base(c) 
	{
		Destination = dest;
	}
	
	public override bool Handle()
	{
		//Check if arrived at destination yet
		if (Vector3.Distance (_client.transform.position, Destination) > 1) {
			//Move directly at the destination;
			_client.transform.position += (Destination - _client.transform.position).normalized * Speed * Time.deltaTime;
		} else {
			_client.NPCStateMachine.Set(NextState);
		}
		return true;
	}
}