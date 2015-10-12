using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	private Transform View;

	void Start () 
	{
		View = FindTransform (transform, "CenterEyeAnchor");
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
			if (result != null) return result;
		}
		return null;
	}
}
