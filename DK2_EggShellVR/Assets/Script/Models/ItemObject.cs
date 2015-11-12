using UnityEngine;
using System.Collections;

public class ItemObject : MonoBehaviour {
	public string ItemName;
	public float Quantity;
	public GameObject PickupText;
	public GameObject ItemMesh;
	public GameObject NameText;

	void Start()
	{
		NameText.GetComponent<TextMesh> ().text = ItemName;
		PickupText.GetComponent<TextMesh> ().text = "+" + Quantity + " " + ItemName;
	}

	//Called when the object is clicked on
	public void PickUp()
	{
		//The name, collider and mesh should immediately disappear
		Destroy (ItemMesh);
		Destroy (NameText);
		var col = GetComponent<CapsuleCollider> ();
		if(col != null)
			col.enabled = false;

		PickupText.SetActive (true);

		//This object should be destroyed when the Pickup text is destroyed too.
		float deathTime = PickupText.GetComponent<FloatUpAndDestroy> ().Lifetime;
		Destroy (gameObject, deathTime);
	}

	//Called when the object is looked at
	public void LookedAt()
	{
		//If the name text is already destroyed, don't do anything
		if (NameText == null)
			return;
		//Activate the name text so it can be seen
		NameText.SetActive (true);
	}

	public void LookedAway()
	{
		//If the name text is already destroyed, don't do anything
		if (NameText == null)
			return;
		//Activate the name text so it can be seen
		NameText.SetActive (false);
	}
}
