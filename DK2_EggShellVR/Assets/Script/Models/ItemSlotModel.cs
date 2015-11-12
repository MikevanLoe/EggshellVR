using System;
using UnityEngine;

[Serializable]
public class ItemSlotModel
{
	public GameObject Object;
	public ItemModel Item;

	private Material _material;

	public ItemSlotModel ()
	{
	}

	public void ResetMaterials()
	{
		//Write the inventory in the book
		var renderer = Object.GetComponent<MeshRenderer>();
		
		_material = new Material(Shader.Find("Transparent/Diffuse"));
		_material.mainTexture = Resources.Load ("border") as Texture;
		renderer.material = _material;
	}

	public void SetTexture(Texture tex = null)
	{
		if (tex == null) 
		{
			if(Item == null)
				tex = Resources.Load ("border") as Texture;
			else
				tex = Resources.Load (Item.Name) as Texture;
		}
		_material.mainTexture = tex;
	}
}