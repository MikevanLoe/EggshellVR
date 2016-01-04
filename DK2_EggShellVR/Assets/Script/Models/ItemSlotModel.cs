using System;
using UnityEngine;

[Serializable]
public class ItemSlotModel
{
	public GameObject Object;
	public ItemModel Item;

	private Material _material;

	//ItemSlotModels are filled through the unity editor so don't need a constructor
	public ItemSlotModel (GameObject o, ItemModel i = null)
	{
		this.Object = o;
		Item = i;
	}

	//Reset the material so it can be edited in code
	public void ResetMaterials()
	{
		//Write the inventory in the book
		var renderer = Object.GetComponent<Decal>();

		//Make a new texture
		_material = new Material(Shader.Find("Decal/DecalShader"));
		_material.mainTexture = Resources.Load ("item slot") as Texture;
		renderer.m_Material = _material;
	}

	public void SetTexture(Texture tex = null)
	{
		if (tex == null) 
		{
			if(Item == null || Item.Name == "")
				tex = Resources.Load ("item slot") as Texture;
			else
				tex = Resources.Load (Item.Name) as Texture;
		}
		_material.mainTexture = tex;
	}
}