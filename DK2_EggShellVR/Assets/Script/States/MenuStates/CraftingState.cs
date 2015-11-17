using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CraftingState : State<MenuController>
{
	const int CraftingSlots = 3;
	const int CraftResultSlot = -4;

	public List<RecipeModel> Recipes;
	public ItemSlotModel[] Slots;
	public ItemSlotModel[] InventorySlots;
	public int InventoryRows = 2;
	public Transform SelectionHighlight;

	private int selected = 0;
	private bool heldX, heldY;

	public CraftingState (MenuController c) : base(c)
	{
	}

	/// <summary>
	/// Called during the start of menu
	/// </summary>
	public void Start()
	{
		//Intiailize the crafting menu and inventory screen
		foreach (var slot in Slots) 
		{
			slot.ResetMaterials();
			slot.Item = null;
		}
		foreach (var slot in InventorySlots) 
		{
			slot.ResetMaterials();
			slot.Item = null;
		}
	}

	/// <summary>
	/// Called when crafting state is entered
	/// </summary>
	public override void Enter()
	{
		RefreshInventory ();
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
		for (; i < inv.Count() && i < InventorySlots.Count(); i++) 
		{
			InventorySlots [i].Item = inv [i];
			InventorySlots [i].SetTexture ();
		}
		//For every inventory slot that's left, empty it and refresh
		for (; i < InventorySlots.Count(); i++)
		{
			InventorySlots [i].Item = null;
			InventorySlots [i].SetTexture ();
		}
		
		//Refresh the crafting slots too
		foreach (var slot in Slots) 
		{
			if(slot.Item == null || slot.Item.Quantity == 0)
				slot.Item = null;
			slot.SetTexture();
		}
	}

	/// <summary>
	/// Handle this updates in crafting menu.
	/// </summary>
	public override bool Handle()
	{
		HandleNavigation ();

		//Action button handling
		if (Input.GetButtonDown ("Submit")) 
		{
			HandleAction ();
			//Update crafting menu
			RecipeModel recipe;
			
			if (TryCheckRecipe (out recipe)) 
				Slots[3].Item = recipe.Result;
			else
				Slots[3].Item = null;
			Slots[3].SetTexture();
		}

		return true;
	}

	/// <summary>
	/// Handles the submit input depending on current selection
	/// </summary>
	void HandleAction ()
	{
		//If selection is in item menu
		if (selected >= 0) 
		{
			//If the selected slot is empty 
			if (InventorySlots [selected].Item == null)
				return;
			//Move selected item to craft list if there is space
			for (int i = 0; i < CraftingSlots; i++) 
			{
				if (Slots[i].Item != null && Slots[i].Item.Quantity > 0)
					continue;
				Slots[i].Item = new ItemModel(InventorySlots [selected].Item); 	//Copy item over
				_client.Player.RemoveItem(InventorySlots [selected].Item); 		//Remove original
				break;
			}
		}
		//If selection is on the crafting menu
		else 
		{
			if(selected == CraftResultSlot)
			{
				Craft ();
			}
			else
			{
				int ci = -1 - selected;		//Convert selection to craft slot index
				//If the craft slot is empty, do nothing
				if (Slots [ci].Item == null)
					return;
				//Else, find space in the inventory if there is any
				foreach (ItemSlotModel slot in InventorySlots) 
				{
					if (slot.Item != null && slot.Item.Quantity > 0)
						continue;
					_client.Player.AddItem(Slots [ci].Item);	//Copy item over
					Slots [ci].Item = null;						//Remove original
					break;
				}
			}
		}
		RefreshInventory ();
	}

	/// <summary>
	/// Handles the navigation through the crafting menu.
	/// </summary>
	void HandleNavigation ()
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

		//Convert float to -1 or 1
		if (input.x != 0)
			input.x = input.x / Mathf.Abs (input.x);
		if (input.y != 0)
			input.y = input.y / Mathf.Abs (input.y);

		//Inventory page
		if (selected >= 0) 
		{
			if (!heldX && input.x != 0) 
			{
				//Inventory page x
				int hold = selected;
				selected += (int)input.x * InventorySlots.Count () / 2;
				if (selected >= InventorySlots.Count ())
					selected = hold;
				if (selected >= -3)
					selected = (int)Mathf.Clamp (selected, -2, InventorySlots.Count () - 1);
			}
			if (!heldY && input.y != 0) 
			{
				//Inventory page y
				if (!(selected == 3 && input.y > 0) && !(selected == 4 && input.y < 0))
					selected += (int)input.y;
				selected = (int)Mathf.Clamp (selected, 0, InventorySlots.Count () - 1);
			}
		}
		//Craft page
		else if (selected > -4) 
		{
			//craft page x
			if (!heldX && input.x != 0) 
			{
				selected += (int)input.x * 2;
				selected = (int)Mathf.Clamp (selected, -3, 1);
				if (selected >= 0) 
				{
					if (selected == 0)
						selected = 2;
					else
						selected = 3;
				}
			}
			if (!heldY && input.y != 0) 
			{
				//Crafting page y
				if (selected == -2) 
				{
					if (input.y > 0)
						selected = -1;
					if (input.y < 0)
						selected = -4;
				}
				else
					if (input.y < 0)
						selected = -2;
			}
		}
		//Result slot
		else
		{
			if (!heldX && input.x > 0) 
				selected = 0;
			if (!heldY && input.y > 0)
				selected = -2;
		}
		
		if (!heldX && input.x != 0) 
		{
			heldX = true;	
		}
		if (!heldY && input.y != 0) 
		{
			heldY = true;
		}

		if (selected >= 0)
			SetSelectionPosition (InventorySlots [selected].Object.transform);
		else
			SetSelectionPosition (Slots [-1 - selected].Object.transform);
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

	/// <summary>
	/// Checks if the current craft item result in a recipe
	/// </summary>
	/// <returns>The possible recipe</returns>
	RecipeModel CheckRecipe()
	{
		foreach(var recipe in Recipes)
		{
			if(recipe.IsCorrect(Slots[0].Item, Slots[1].Item, Slots[2].Item))
				return recipe;
		}
		return null;
	}

	/// <summary>
	/// Checks if the current craft items result in a recipe
	/// </summary>
	/// <returns><c>true</c>, if check recipe was possible, <c>false</c> otherwise.</returns>
	/// <param name="recipe">The found recipe.</param>
	bool TryCheckRecipe(out RecipeModel recipe)
	{
		recipe = CheckRecipe ();
		if (recipe == null)
			return false;
		return true;
	}

	/// <summary>
	/// Checks if craft items are a recipe, consumes them and makes a new item.
	/// </summary>
	public void Craft()
	{
		RecipeModel curRecipe;
		if (!TryCheckRecipe(out curRecipe))
			return;

		//Remove the used items from the inventory
		_client.Player.RemoveItem (curRecipe.Ingredient1);
		_client.Player.RemoveItem (curRecipe.Ingredient2);
		_client.Player.RemoveItem (curRecipe.Ingredient3);

		//Clear the crafting screen
		Slots [0].Item = null;
		Slots [1].Item = null;
		Slots [2].Item = null;
		Slots [3].Item = null;

		//Add the crafted item to the inventory
		_client.Player.AddItem (curRecipe.Result);
	}
}