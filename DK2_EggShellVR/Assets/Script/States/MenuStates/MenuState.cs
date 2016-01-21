using System;
using UnityEngine;

public abstract class MenuState : State<MenuController>
{
	public GameObject Menu;

	public MenuState(MenuController c) : base(c){
	}
	
	public override void Enter()
	{
		Menu.SetActive (true);
	}
	
	public override void Exit()
	{
		Menu.SetActive (false);
	}
}