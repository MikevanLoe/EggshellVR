using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour {
	public string PersonalityName;
	public Vector3 RotOffset;
	public float MaxRot = 2f;

	private Transform _center;
	private Transform _neck;
	private StateMachine<NPCController> _stateMachine;
	private float _curAngle;
	private Dictionary<string, bool> Switches;
	private Dictionary<string, float> Variables;
	private Personality _personality;
	private Vector3 _originalPosition;

	public StateMachine<NPCController> NPCStateMachine {
		get { return _stateMachine; }
	}

	void Start () 
	{
		//Find child object with the name "Neck"
		_neck = FindTransform (transform, "Neck");
		if (_neck == null)
			throw new UnityException ("NPC: " + name + " has no neck attached");
		//Find child object with name "Center"
		_center = FindTransform (transform, "Center");
		if (_center == null)
			throw new UnityException ("NPC: " + name + " has no center attached");

		_stateMachine = new StateMachine<NPCController> ();

		_stateMachine.Add (new TownState (this, 8));
		_stateMachine.Add (new CrowdState (this));
		_stateMachine.Add (new TravelState (this));
		if (!_stateMachine.Set ("TownState")) 
		{
			Debug.Log ("Failed to set state.");
			throw new UnityException();
		}

		Switches = new Dictionary<string, bool> ();
		Variables = new Dictionary<string, float> ();

		_originalPosition = transform.position;

		switch (PersonalityName) 
		{
		case "Bland":
			_personality = new Bland(this);
			break;
		case "Islander":
			_personality = new Islander(this);
			break;
		case "ShopOwner":
			_personality = new ShopOwner(this);
			break;
		case "Couple":
			_personality = new Couple(this);
			break;
		case "Rude":
			_personality = new Rude(this);
			break;

		default:
			throw new UnityException("Chosen personality for NPC: \"" + name + "\" not found");
		}
	}

	void LateUpdate () 
	{
		_stateMachine.Handle ();
		_personality.Update ();
	}

	public void MarketCall(float performance)
	{
		var stateName = _stateMachine.GetCurState ().Name;
		bool interested = _personality.MarketCall (performance);
		if (stateName != "CrowdState" && stateName != "TravelState") 
		{
			if (interested) 
			{
				_stateMachine.Set ("TravelState");
				var travelState = (TravelState)_stateMachine.GetCurState ();
				var obj = GameObject.FindGameObjectWithTag("PresentationController");
				PresentationController presCont = obj.GetComponent<PresentationController>();
				travelState.Destination = presCont.GetCrowdPosition();
				travelState.NextState = "CrowdState";
			}
		} 
		else 
		{
			if(!interested)
			{
				TravelState travelState;
				if (stateName != "TravelState")
					_stateMachine.Set ("TravelState");
				travelState = (TravelState)_stateMachine.GetCurState ();
				travelState.Destination = _originalPosition;
				travelState.NextState = "TownState";
			}
		}
	}

	public void LookAt(Transform obj)
	{
		//Create a dummy that looks directly at the target
		Transform target = new GameObject().transform;
		target.position = _neck.position;
		target.rotation = _neck.rotation;
		target.LookAt(obj.position);

		float a = Mathf.Abs(target.rotation.eulerAngles.y - _center.rotation.eulerAngles.y);
		if (a >= 70)
		{
			target.position = _center.position;
			target.rotation = _center.rotation;
		}
		target.Rotate (RotOffset);
		_neck.rotation = Quaternion.RotateTowards(_neck.rotation, target.rotation, MaxRot);
		return;
	}
	
	public Transform GetNeckTransform()
	{
		return _neck;
	}
	
	public Transform GetCenterTransform()
	{
		return _center;
	}

	public void LookedAt()
	{
		_personality.LookedAt ();
	}
	
	public bool GetSwitch(string key)
	{
		bool retSwitch;
		if (Switches.TryGetValue (key, out retSwitch))
			return retSwitch;
		return false;
	}
	
	public float GetVariable(string key)
	{
		float retVar;
		if (Variables.TryGetValue (key, out retVar))
			return retVar;
		return 0;
	}
	
	public void SetSwitch(string key, bool value)
	{
		Switches [key] = value;
	}
	
	public void SetVariable(string key, float value)
	{
		Variables [key] = value;
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
