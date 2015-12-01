using UnityEngine;
using System.Collections;

public class FishAI : MonoBehaviour {
	public Mover3D mover;
	public bool isHooked;

	private float delay = 2.5f;
	private float hookedTime;

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
			//this.GetComponent<Rigidbody>().
			if(hookedTime <= 0f)
				hookedTime = Time.time;

			if (hookedTime <= hookedTime+delay) {
				if (Input.GetButtonDown ("Fire1")) {
					GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddItem(new ItemModel("Vis",1));
					Destroy(this.gameObject);
				}
			}
			else
				isHooked = !isHooked;
		}
	}
}
