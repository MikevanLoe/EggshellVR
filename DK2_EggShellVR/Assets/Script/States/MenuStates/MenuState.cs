using System;
using UnityEngine;

public abstract class MenuState : State<MenuController>
{
	public GameObject Menu;

	public MenuState(MenuController c) : base(c){
	}
}