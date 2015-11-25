using UnityEngine;
using System.Collections;

public class FishAI : MonoBehaviour {
	public Mover3D mover;
	
	// Update is called once per frame
	void Update () {
		Vector3 Force = mover.Wander ();
		Force.y = 0;
		mover.Accelerate (Force);
	}
}
