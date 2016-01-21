using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class OpenPresMenu : SceneEvent
{
	public OpenPresMenu ()
	{
	}
	
	public override void Execute()
	{	
		var player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		PresentationController presCont = GameObject.FindGameObjectWithTag ("PresentationController").GetComponent<PresentationController> ();
		
		player.ForceOpenInv (true, true);
		var menu = player.GetComponentInChildren<MenuController> ();
		menu.LockMenu (presCont.PresMenuObject);
		presCont.PresMenuObject.SetActive (true);
	}
}