using UnityEngine;
using System.Collections;

public class FishAI : MonoBehaviour {
	public Mover3D mover;
	public bool isHooked;

	private float delay = 1.5f;
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
			if (Time.time-hookedTime < delay)
			{
				this.GetComponent<Rigidbody>().velocity = Vector3.zero;
				this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				if (Input.GetButtonDown ("Fire1"))
				{
					GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddItem(new ItemModel("Vis",1));
					Destroy(this.gameObject);
					hookedTime = 0f;
				}
			}
			else
			{
				isHooked = !isHooked;
				hookedTime = 0f;
			}
		}
	}

	public void Hooked()
	{
		hookedTime = Time.time;
		isHooked = true;
	}
}
