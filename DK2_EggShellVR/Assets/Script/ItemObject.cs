using UnityEngine;
using System.Collections;

public class ItemObject : MonoBehaviour {
	public string ItemName;
	public float Quantity;
	public GameObject PickupText;
	public GameObject ItemMesh;
	public GameObject NameText;
	public float NameDisappearTime;

	void Start()
	{
		NameText.GetComponent<TextMesh> ().text = ItemName;
		PickupText.GetComponent<TextMesh> ().text = "+" + Quantity + " " + ItemName;
	}

	void Update()
	{
		//Deactivate the name text after not being looked at for a moment
		if (Time.time > NameDisappearTime) 
		{
			NameText.SetActive(false);
		}
	}

	//Called when the object is clicked on
	public void PickUp()
	{
		//The name and mesh should immediately disappear
		Destroy (ItemMesh);
		Destroy (NameText);

		PickupText.SetActive (true);

		//This object should be destroyed when the Pickup text is destroyed too. Ideally not before.
		float deathTime = PickupText.GetComponent<FloatUpAndDestroy> ().Lifetime;
		NameDisappearTime = Time.time + deathTime + 1;
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
		//Set a timer to make it disappear after not being looked at for a moment.
		NameDisappearTime = Time.time + 0.1f;
	}
}
