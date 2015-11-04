using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {
	public bool RotatesOnX;

	private Transform _center;
	private Transform _neck;
	private StateMachine _stateMachine;
	private float _curX;
	private Vector3 _forward;

	void Start () 
	{
		//Find child object with the name "neck"
		_neck = FindTransform (transform, "Neck");
		_center = FindTransform (transform, "Center");
		_forward = _neck.forward;
		_stateMachine = new StateMachine ();

		_stateMachine.Add (new TownState (this, 8));
		_stateMachine.Add (new CrowdState (this));
		if (!_stateMachine.Set ("CrowdState")) 
		{
			Debug.Log ("Failed to set state.");
			throw new UnityException();
		}
	}

	void LateUpdate () 
	{
		_stateMachine.Handle ();
	}

	public void LookAt(Transform obj)
	{
		//Find the direction of the object relative to NPC
		Vector3 dir = _neck.position - obj.position;
		
		//Find the angle between the forward line and the direction to the player
		float b = Vector2.Angle(new Vector2(dir.x, dir.z), new Vector2(_forward.x, _forward.z));
		
		//Turn it by 90 because 0 degrees isn't forward. If our 3D modellers make this too I will shank them.
		if (RotatesOnX)
			b -= 90;
		else 
		{
			//If the head turns over 90 degrees, start turning in the other direction
			if(b >= 90)
				b = 90 - (b - 90);
			if(dir.x < 0)
				b *= -1;
		}
		
		//Turn the neck by the difference with current angle
		if (Mathf.Round (b) != Mathf.Round (_curX)) 
		{
			if(RotatesOnX)
				_neck.Rotate (new Vector3 (b - _curX, 0, 0));
			else
				_neck.Rotate (new Vector3 (0, b - _curX, 0));
			//Remember current angle
			_curX = b;
		}
	}
	
	public Transform GetNeckTransform()
	{
		return _neck;
	}
	
	public Transform GetCenterTransform()
	{
		return _center;
	}

	public static Transform FindTransform(Transform parent, string name)
	{
		if (parent.name.Equals(name)) return parent;
		foreach (Transform child in parent)
		{
			Transform result = FindTransform(child, name);
			if (result != null) return result;
		}
		return null;
	}
}
