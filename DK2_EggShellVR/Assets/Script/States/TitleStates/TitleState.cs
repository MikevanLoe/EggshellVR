using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleState : State<TitleController>
{
	private int Selection;
	private Transform SelectionTransform;
	private List<Transform> MenuTransforms;
	private bool _axisUsed;

	public TitleState (TitleController c, Transform s, List<Transform> m) : base(c) 
	{
		SelectionTransform = s;
		MenuTransforms = m;
	}

	//Show the title screen
	public override void Enter()
	{
		SelectionTransform.gameObject.SetActive (true);
		MenuTransforms.ForEach(m => m.gameObject.SetActive(true));
	}
	
	public override bool Handle()
	{	
		//Handle title choices
		if(Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Return))
		{
			switch(Selection)
			{
			case 0:
				_client.GotoTransition();
				break;
			case 1:
				Application.Quit();
				return true;
			}
		}

		//Handle selection switching
		float y = Input.GetAxis ("Vertical");
		if (y == 0)
			_axisUsed = false;
		else 
		{
			//Only use vertical press once until reset
			if(_axisUsed)
				return true;
			_axisUsed = true;
			//Convert input to 1 or -1
			Selection -= (int) Mathf.Sign(y);

			//Wrap selection around the top and bottom
			if(Selection >= MenuTransforms.Count)
				Selection = 0;
			if (Selection < 0)
				Selection = MenuTransforms.Count - 1;
			//Put the selection transform slightly behind the menu choice
			SelectionTransform.position = MenuTransforms[Selection].position;
			SelectionTransform.Translate(0, -0.1f, 0);
		}

		return true;
	}
}