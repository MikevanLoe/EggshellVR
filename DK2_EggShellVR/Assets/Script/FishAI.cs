using UnityEngine;
using System.Collections;

public class FishAI : MonoBehaviour {
	public Mover3D mover;
	public bool isHooked;
	
	// Update is called once per frame
	void Update ()
	{
		if (!isHooked)
		{
			Vector3 Force = mover.Wander ();
			Force.y = 0;
			mover.Accelerate (Force);
		}
		else
		{
			// Haak beweegt op en neer 5x
			
			if (Input.GetButtonDown ("Fire1")) {
				GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddItem(new ItemModel("Vis",1));
				Destroy(this.gameObject);
			}
			// Na 5x Vis zwemt weg
		}
	}
}
