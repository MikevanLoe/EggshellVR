using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {
	
	public enum Axis {up, down, left, right, forward, back};

	public Axis axis = Axis.up;

	private Camera Cam;

	// return a direction based upon chosen axis
	public Vector3 GetAxis (Axis refAxis)
	{
		switch (refAxis)
		{
		case Axis.down:
			return Vector3.down; 
		case Axis.forward:
			return Vector3.forward; 
		case Axis.back:
			return Vector3.back; 
		case Axis.left:
			return Vector3.left; 
		case Axis.right:
			return Vector3.right; 
		}
		
		// default is Vector3.up
		return Vector3.up;
	}

	void Update () {
		if (Cam == null)
			Cam = Camera.main;
		else {
			Vector3 targetPos = transform.position + Cam.transform.rotation * Vector3.back;
			Vector3 targetOrientation = Cam.transform.rotation * GetAxis(axis);
			transform.LookAt (targetPos, targetOrientation);
		}
	}
}
