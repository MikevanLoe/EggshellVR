using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ClosePresMenu : SceneEvent
{
	public ClosePresMenu ()
	{
	}
	
	public override void Execute()
	{	
		var player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		PresentationController presCont = GameObject.FindGameObjectWithTag ("PresentationController").GetComponent<PresentationController> ();
		
		player.ForceCloseInv ();
		var menu = player.GetComponentInChildren<MenuController> ();
		menu.UnlockMenu ();
	}
}