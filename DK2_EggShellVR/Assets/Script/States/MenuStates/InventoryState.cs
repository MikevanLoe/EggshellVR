using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityStandardAssets.CrossPlatformInput;

public class InventoryState : MenuState
{	
	const int CraftingSlots = 3;
	const int CraftResultSlot = -4;
	
	public List<RecipeModel> Recipes;
	public int InventoryColumns = 2;
	public Transform SelectionHighlight;
	
	private List<ItemSlotModel> SlotsLeft;
	private List<ItemSlotModel> SlotsRight;
	private TextMesh Description;
	private int selected = 0;
	private bool heldX, heldY;

	public InventoryState (MenuController c) : base(c)
	{
		//Find Crafting menu
		Menu = _client.transform.FindChild ("Inventory Menu").gameObject;
		SelectionHighlight = Menu.transform.FindChild ("Highlight");
		int i = 1;
		SlotsLeft = new List<ItemSlotModel> ();
		while (true) {
			Transform slotT = Menu.transform.FindChild ("Left/Slot " + i);
			if(slotT == null)
				break;
			SlotsLeft.Add(new ItemSlotModel(slotT.gameObject));
			i++;
		}
		
		i = 1;
		SlotsRight = new List<ItemSlotModel> ();
		while (true) {
			Transform slotT = Menu.transform.FindChild ("Right/Slot " + i);
			if(slotT == null)
				break;
			SlotsRight.Add(new ItemSlotModel(slotT.gameObject));
			i++;
		}

		Description = Menu.transform.FindChild ("Left/ItemDesc").GetComponent<TextMesh> ();

		//Intiailize the crafting menu and inventory screen
		foreach (var slot in SlotsLeft) 
		{
			slot.ResetMaterials();
			slot.Item = null;
		}
		foreach (var slot in SlotsRight) 
		{
			slot.ResetMaterials();
			slot.Item = null;
		}
	}
	
	public override void Enter()
	{
		RefreshInventory ();
		Menu.SetActive (true);
	}
	
	public override void Exit()
	{
		Menu.SetActive (false);
	}
	
	/// <summary>
	/// Refreshs the inventory.
	/// </summary>
	public void RefreshInventory()
	{
		//Put inventory on the screen
		List<ItemModel> inv = _client.Player.Inventory;
		int i = 0;
		//Loop through every item in the player inventory
		for (; i < inv.Count && i < SlotsLeft.Count() + SlotsRight.Count; i++) {
			UpdateSlot (i, inv[i]);
		}
		//For every inventory slot that's left, empty it and refresh
		for (; i < SlotsLeft.Count + SlotsRight.Count; i++) {
			UpdateSlot (i, null);
		}
	}

	void UpdateSlot (int i, ItemModel im)
	{
		if (i >= SlotsLeft.Count) {
			int j = i - SlotsLeft.Count;
			SlotsRight [j].Item = im;
			SlotsRight [j].SetTexture ();
		}
		else {
			int j = i;
			if(i < SlotsLeft.Count)
				j = SlotsLeft.Count - 1 - i;
			SlotsLeft [j].Item = im;
			SlotsLeft [j].SetTexture ();
		}
	}

	public override bool Handle()
	{
		//Moving the selection
		Vector2 input = new Vector2
		{
			x = CrossPlatformInputManager.GetAxis("Horizontal"),
			y = CrossPlatformInputManager.GetAxis("Vertical")
		};
		
		//Reset hold
		if (heldX && input.x == 0)
			heldX = false;
		if (heldY && input.y == 0)
			heldY = false;
		
		//If there is no input
		if (input.x == 0 && input.y == 0)
			return true;
		
		//Convert float to -1 or 1
		if(input.x != 0)
			input.x = Mathf.Sign (input.x);
		if(input.y != 0)
			input.y = Mathf.Sign (input.y);

		//Don't wrap plz
		if (input.y > 0 && !heldY) {
			heldY = true;
			if (selected != -1 && selected != -3 && selected != 2 && selected < SlotsRight.Count -1)
				selected += (int)input.y;
		} else if (input.y < 0 && !heldY) {
			heldY = true;
			if (selected != -2 && selected != 0 && selected != 3 && selected > -SlotsLeft.Count)
				selected += (int)input.y;
		}

		if((input.x > 0 || input.x < 0) && !heldX)
		{
			heldX = true;
			//Right page
			if (selected >= 0) {
				int hold = selected;
				selected += (int)input.x * SlotsRight.Count / 2;
				if (selected >= SlotsRight.Count ())
					selected = hold;
				if(selected < 0)
				{
					if(hold >= 1)
						selected = -1;
					else
						selected = -2;
				}
			}
			else
			{
				int hold = selected;
				selected += (int)input.x * SlotsLeft.Count / 2;
				if (-1 - selected >= SlotsLeft.Count ())
					selected = hold;
			}
		}

		if (selected >= 0)
		{
			SetSelectionPosition (SlotsRight [selected].Object.transform);
			var item = SlotsRight [selected].Item;
			if(item != null)
				Description.text = item.Description;
		}
		else
		{
			SetSelectionPosition (SlotsLeft [-1 - selected].Object.transform);
			var item = SlotsLeft [-1 - selected].Item;
			if(item != null){
				Description.text = item.Description;
			}
			else
				Description.text = "";
		}

		return true;
	}
	
	/// <summary>
	/// Sets the position of the selection highlight.
	/// </summary>
	/// <param name="transform">Destination for the selection highlight</param>
	public void SetSelectionPosition(Transform t)
	{
		SelectionHighlight.position = t.position;
		SelectionHighlight.rotation = t.rotation;
	}
}