using System;

[Serializable]
public class ItemModel
{
	public string Name;
	public float Quantity;
	public string Description;
	
	public ItemModel (string n, float q)
	{
		Name = n;
		Quantity = q;
	}
	
	public ItemModel (ItemModel copy)
	{
		Name = copy.Name;
		Quantity = copy.Quantity;
	}
	
	public static bool operator >=(ItemModel i1, ItemModel i2)
	{
		if (i1 == null || i2 == null)
			return false;
		if(i1.Name == i2.Name && i1.Quantity >= i2.Quantity)
			return true;
		return false;
	}
	
	public static bool operator <=(ItemModel i1, ItemModel i2)
	{
		if (i1 == null || i2 == null)
			return false;
		if(i1.Name == i2.Name && i1.Quantity <= i2.Quantity)
			return true;
		return false;
	}
}