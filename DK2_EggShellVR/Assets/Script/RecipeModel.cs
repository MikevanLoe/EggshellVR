using System;
using System.Collections.Generic;

[Serializable]
public class RecipeModel
{
	public ItemModel Ingredient1;
	public ItemModel Ingredient2;
	public ItemModel Ingredient3;
	public ItemModel Result;
	
	public RecipeModel (ItemModel result, ItemModel i1, ItemModel i2 = null, ItemModel i3 = null)
	{
		Result = result;
		Ingredient1 = i1;
		Ingredient2 = i2;
		Ingredient3 = i3;
	}

	//Check if three items correspond with the recipe
	public bool IsCorrect(ItemModel i1, ItemModel i2, ItemModel i3)
	{
		//Put all items into a list
		List<ItemModel> items = new List<ItemModel>{i1, i2, i3};
		List<ItemModel> recipeItems = new List<ItemModel>{Ingredient1, Ingredient2, Ingredient3};

		//Check all items to check with items in the recipe
		foreach(var item in items)
		{
			//Check if this item is in the recipe
			bool found = false;
			foreach(var rItem in recipeItems)
			{
				//If both items are null or the same
				if((item == null && rItem == null) || item >= rItem)
				{
					found = true;
					break;
				}
			}
			if(!found)
				return false;
		}
		return true;
	}
}