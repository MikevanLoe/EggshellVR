using UnityEngine;
using System.Collections;

public class RecenterState : State<TitleController>
{
	//Object that tells player to recenter
	private GameObject RecenterText;

	public RecenterState (TitleController c, GameObject r) : base(c) 
	{
		RecenterText = r;
	}
	
	public override bool Handle()
	{
		//Wait for player to press key and start recenter process
		if (Input.anyKeyDown && RecenterText.activeSelf) 
		{
			_client.StartCoroutine("Recenter");
			//Hide recenter text
			RecenterText.SetActive(false);
		}
		return true;
	}
}