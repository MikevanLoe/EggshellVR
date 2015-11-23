using System;
using UnityEngine;

public class PlayerLine : Interaction
{
	private Transform _player;
	private TextMesh _hintMesh;
	private string _hintText;
	
	public PlayerLine (string hint, float dur)
	{
		_hintText = hint;
		Duration = dur;

		var p = GameObject.FindGameObjectWithTag ("Player");
		_player = p.transform;
		_hintMesh = p.GetComponent<TextMesh> ();
	}

	public override void Execute(){}
}