using UnityEngine;
using System.Collections;

public class FishAI : MonoBehaviour {
	public Mover mover;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 Force = mover.Wander ();
		transform.position += new Vector3(Force.x, 0, Force.y);
	}
}
