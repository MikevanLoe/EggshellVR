using UnityEngine;
using System.Linq;

public class InventoryState : State<MenuController>
{	
	public InventoryState (MenuController c) : base(c)
	{

	}

	public override bool Handle()
	{
		//Write the inventory in the book
//		string InvString = "";
//		if (_client.Player.Inventory.Any ()) {
//			foreach (ItemModel im in _client.Player.Inventory) {
//				InvString += im.Name + " x" + im.Quantity + "\n"; 
//			}
//		} else {
//			InvString = "Your inventory \nis empty";
//		}
//		_client.InventoryTextRight.text = InvString;
		return true;
	}
}