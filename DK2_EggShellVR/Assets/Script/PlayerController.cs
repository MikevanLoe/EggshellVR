﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour {
	private Transform View;
	public List<ItemModel> Inventory;
	public float InteractRange;
	public Transform AimObject;

	public Transform InventoryObject;
	public Transform InventoryClosed;
	public Transform InventoryOpen;
	public TextMesh InventoryText;
	public float TweenTime;

	private float _inventoryProgress;
	private bool _invOpen = false;
	private bool _invTweening = false;
	private GameObject prevLookedAt;

	void Start () 
	{
		View = FindTransform (transform, "CenterEyeAnchor");
		if (View == null)
			View = transform;
		Inventory = new List<ItemModel> ();
	}

	void Update ()
	{
		//See what player is looking at
		CheckLookAt ();

		//Open and close Inventory
		if (Input.GetKeyDown (KeyCode.E))
		{
			if(!_invTweening)
			{
				if(!_invOpen)
				{
					InventoryObject.gameObject.SetActive(true);
					StartCoroutine(OpenAndCloseInventory(InventoryOpen));
				}
				else
				{
					StartCoroutine(OpenAndCloseInventory(InventoryClosed));
				}
			}
		}
		string InvString = "";
		if (Inventory.Any ()) {
			foreach (ItemModel im in Inventory) {
				InvString += im.Name + " x" + im.Quantity + "\n"; 
			}
		} else {
			InvString = "Your inventory \nis empty";
		}
		InventoryText.text = InvString;
	}

	//Raycasts forward to find what the player is looking at
	void CheckLookAt ()
	{
		RaycastHit hitData;
		if (Physics.Raycast (AimObject.position, AimObject.forward, out hitData, InteractRange)) 
		{
			GameObject obj = hitData.transform.gameObject;
			if (obj != prevLookedAt && prevLookedAt != null)
			{
				prevLookedAt.SendMessageUpwards ("LookedAway", SendMessageOptions.DontRequireReceiver);
				obj.SendMessageUpwards ("LookedAt", SendMessageOptions.DontRequireReceiver);
			}
			//If it's an item and the player just clicked, pick it up
			if (obj.tag == "Item") 	
			{
				ItemObject item = obj.GetComponentInParent<ItemObject> ();
				//If the player wants to pick it up, do that
				if (Input.GetMouseButtonDown (0)) 
				{
					AddItem (new ItemModel (item.ItemName, item.Quantity));
					obj.SendMessageUpwards ("PickUp");
				}
			}
			prevLookedAt = obj;
		} else if (prevLookedAt != null) {
			prevLookedAt.SendMessageUpwards ("LookedAway", SendMessageOptions.DontRequireReceiver);
			prevLookedAt = null;
		}
	}

	//Coroutine for a tweening animation of inventory opening and closing
	IEnumerator OpenAndCloseInventory(Transform destination)
	{
		_invTweening = true;
		//Calculate progress to move per frame
		float progPerFrame = 1 * Time.deltaTime / TweenTime;
		Vector3 startPos = InventoryObject.localPosition;
		Quaternion startRot = InventoryObject.localRotation;
		for (float prog = 0; prog <= 1f; prog += progPerFrame)
		{
			//Tween local transform
			InventoryObject.localPosition = Vector3.Lerp(startPos, destination.localPosition, prog);
			InventoryObject.localRotation = Quaternion.Slerp(startRot, destination.localRotation, prog);
			yield return new WaitForSeconds(Time.deltaTime);
		}
		_invOpen = !_invOpen;
		if(!_invOpen)
		{
			InventoryObject.gameObject.SetActive(false);
		}
		_invTweening = false;
	}

	//Gets the angle at which the player is looking at a position
	public float GetAngle(Vector3 pos)
	{
		Vector3 dir = pos - transform.position;
		float angle = Vector3.Angle (View.forward, dir);
		return angle;
	}

	//Find a child object by name
	public static Transform FindTransform(Transform parent, string name)
	{
		if (parent.name.Equals(name))
			return parent;
		foreach (Transform child in parent)
		{
			Transform result = FindTransform(child, name);
			if (result != null) 
				return result;
		}
		return null;
	}

	//Add the item to the player inventory
	public void AddItem(ItemModel item)
	{
		//Find the item in the inventory
		int index = Inventory.FindIndex (i => i.Name == item.Name);
		//If the inventory doesn't yet contain this item
		if (Inventory.FindIndex (i => i.Name == item.Name) == -1) 
		{
			Inventory.Add (item);
		}
		else 
		{
			Inventory[index].Quantity += item.Quantity;
		}
	}
	
	//Remove an item from player inventory
	public void RemoveItem(string iName)
	{
		//Find the item in the inventory
		int index = Inventory.FindIndex (i => i.Name == iName);
		//If the inventory doesn't contain this item
		if (Inventory.FindIndex (i => i.Name == iName) == -1) 
		{
			return;
		}
		else 
		{
			Inventory.RemoveAt(index);
		}
	}

	//Check if the player inventory contains a given item
	public bool HasItem(string iName, float quanitity = 1)
	{
		return Inventory.Any (i => i.Name == iName && i.Quantity >= quanitity);
	}
}
