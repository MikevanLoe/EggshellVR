using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CraftingState : State<MenuController>{	
	public List<RecipeModel> Recipes;
	public ItemSlotModel[] Slots;
	public ItemSlotModel[] InventorySlots;
	public int InventoryRows = 2;

	private int selected = 0;
	private bool heldX, heldY;

	public CraftingState (MenuController c) : base(c)
	{
	}

	public void Start()
	{
		foreach (var slot in Slots) 
		{
			slot.ResetMaterials();
		}
		foreach (var slot in InventorySlots) 
		{
			slot.ResetMaterials();
		}
	}
	
	public override bool Handle()
	{
		//TODO: Massive refactor PLEASE

		//Refresh player inventory
		List<ItemModel> inv = _client.Player.Inventory;
		int i = 0;
		for (; i < inv.Count() && i < InventorySlots.Count(); i++) {
			InventorySlots [i].Item = inv [i];
			InventorySlots [i].SetTexture ();
		}
		while(i < InventorySlots.Count())
		{
			InventorySlots [i].Item = null;
			InventorySlots [i].SetTexture ();
			i++;
		}

		//Reset every slot every update.
		//Like, what? Are you kidding?
		//I'm surprised the game can just deal with this lol
		foreach (var slot in Slots) 
		{
			slot.SetTexture();
		}

		SwitchMenu ();

		//Action button handling
		if (Input.GetButtonDown ("Submit")) {
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

	void HandleAction ()
	{
		if (selected >= 0) {
			//If the current slot is empty, move on
			if (InventorySlots [selected].Item == null)
				return;
			//Move selected item to craft list if there is space
			foreach (ItemSlotModel slot in Slots) {
				if (slot.Item != null)
					continue;
				//Copy item over
				slot.Item = new ItemModel(InventorySlots [selected].Item);
				//Remove original
				_client.Player.RemoveItem(InventorySlots [selected].Item);
				break;
			}
		}
		else {
			if(selected == -4)
			{
				Craft ();
				return;
			}
			int ci = -1 - selected;
			if (Slots [ci].Item == null)
				return;
			foreach (ItemSlotModel slot in InventorySlots) {
				if (slot.Item != null)
					continue;
				//Copy item over
				_client.Player.AddItem(Slots [ci].Item);
				//Remove original
				Slots [ci].Item = null;
				break;
			}
		}
	}

	void SwitchMenu ()
	{
		//Moving the selection
		Vector2 input = new Vector2
		{
			x = CrossPlatformInputManager.GetAxis("Horizontal"),
			y = CrossPlatformInputManager.GetAxis("Vertical")
		};

		//Convert float to -1 or 1
		if (input.x != 0)
			input.x = input.x / Mathf.Abs (input.x);
		if (input.y != 0)
			input.y = input.y / Mathf.Abs (input.y);

		//Reset hold
		if (heldX && input.x == 0)
			heldX = false;
		if (heldY && input.y == 0)
			heldY = false;

		if (!heldX && input.x != 0) {
			heldX = true;
			if (selected >= 0) {
				//Inventory page x
				int hold = selected;
				selected += (int)input.x * InventorySlots.Count () / 2;
				if (selected >= InventorySlots.Count ())
					selected = hold;
				if (selected >= -3)
					selected = (int)Mathf.Clamp (selected, -2, InventorySlots.Count () - 1);
			}
			else {
				//Craft page x
				if (selected > -4) {
					selected += (int)input.x * 2;
					selected = (int)Mathf.Clamp (selected, -3, 1);
					if (selected >= 0) {
						if (selected == 0)
							selected = 2;
						else
							selected = 3;
					}
				}
				else {
					selected += (int)input.x * 4;
					selected = (int)Mathf.Clamp (selected, -4, 0);
				}
			}
		}
		if (!heldY && input.y != 0) {
			heldY = true;
			if (selected >= 0) {
				//Inventory page y
				if (!(selected == 3 && input.y > 0) && !(selected == 4 && input.y < 0))
					selected += (int)input.y;
				selected = (int)Mathf.Clamp (selected, 0, InventorySlots.Count () - 1);
			}
			else {
				//Crafting page y
				if (selected == -2) {
					if (input.y > 0)
						selected = -1;
					if (input.y < 0)
						selected = -4;
				}
				else
					if (selected == -4 && input.y > 0)
						selected = -2;
					else
						if (selected != -4 && input.y < 0)
							selected = -2;
			}
		}
		if (selected >= 0)
			_client.SetSelectionPosition (InventorySlots [selected].Object.transform);
		else
			_client.SetSelectionPosition (Slots [-1 - selected].Object.transform);
	}
	
	RecipeModel CheckRecipe()
	{
		foreach(var recipe in Recipes)
		{
			if(recipe.IsCorrect(Slots[0].Item, Slots[1].Item, Slots[2].Item))
				return recipe;
		}
		return null;
	}

	bool TryCheckRecipe(out RecipeModel recipe)
	{
		recipe = CheckRecipe ();
		if (recipe == null)
			return false;
		return true;
	}

	public void Craft()
	{
		RecipeModel curRecipe = CheckRecipe ();
		if (curRecipe == null)
			return;
		//Remove the used items from the inventory
		_client.Player.RemoveItem (curRecipe.Ingredient1);
		_client.Player.RemoveItem (curRecipe.Ingredient2);
		_client.Player.RemoveItem (curRecipe.Ingredient3);

		Slots [0].Item = null;
		Slots [1].Item = null;
		Slots [2].Item = null;
		Slots [3].Item = null;

		//Add the crafted item to the inventory
		_client.Player.AddItem (curRecipe.Result);
	}
}