using UnityEngine;
using System.Collections;

public class FishAI : MonoBehaviour {
	public Mover3D mover;
	public bool isHooked;

	private float delay = 1.5f;
	private float hookedTime;

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!isHooked)
		{
			Vector3 Force;
			Force = mover.WallAvoidance();
			Force += mover.Wander ();
			Force.y = 0;
			//Stop moving when hooked
			mover.Accelerate (Force);
			mover.FaceHeading();
		}
		else
		{
			if (Time.time-hookedTime < delay)
			{
				this.GetComponent<Rigidbody>().velocity = Vector3.zero;
				this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				if (Input.GetButtonDown ("Fire1"))
				{
					GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddItem(new ItemModel("Vis",1, "Dit is een verse haring. Hij is \ngevangen in de Njord zee. Hij is\n 35 cm lang en weegt 1 kilo."));
					Destroy(this.gameObject, 0.7f);
					delay = 100;
				}
			}
			else
			{
				isHooked = !isHooked;
				hookedTime = 0f;
				transform.parent = null;
			}
		}
	}

	public void Hooked()
	{
		hookedTime = Time.time;
		isHooked = true;
		this.GetComponent<Rigidbody>().velocity = Vector3.zero;
		this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
	}
}
