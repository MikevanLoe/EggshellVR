using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour {
	public string PersonalityName;
	public Vector3 RotOffset;
	public float MaxRot = 2f;
	public Personality Personality;
	[HideInInspector]
	public Animator MyAnimator;

	private Transform _center;
	private Transform _neck;
	private Transform _target;
	private StateMachine<NPCController> _stateMachine;
	private float _curAngle;
	private Dictionary<string, bool> Switches;
	private Dictionary<string, float> Variables;
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

		MyAnimator = GetComponent<Animator> ();
		if (MyAnimator == null)
			throw new UnityException ("NPC: " + name + " has no Animator attached");

		_target = new GameObject ("LookTarget").transform;
		_target.parent = transform;
		_target.position = _center.position;
		_target.rotation = _center.rotation;

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
			Personality = new Bland(this);
			break;
		case "Islander":
			Personality = new Islander(this);
			break;
		case "ShopOwner":
			Personality = new ShopOwner(this);
			break;
		case "Couple":
			Personality = new Couple(this);
			break;
		case "Rude":
			Personality = new Rude(this);
			break;
		case "PresTutorial":
			Personality = new PresTutorial(this);
			break;

		default:
			throw new UnityException("Chosen personality for NPC: \"" + name + "\" not found");
		}
	}

	void LateUpdate () 
	{
		_stateMachine.Handle ();
		Personality.Update ();
	}

	public void MarketCall(float performance)
	{
		var stateName = _stateMachine.GetCurState ().Name;
		bool interested = Personality.MarketCall (performance);
		if (stateName != "CrowdState" && stateName != "TravelState") 
		{
			if (interested) 
			{
				_originalPosition = transform.position;
				_stateMachine.Set ("TravelState");
				var travelState = (TravelState)_stateMachine.GetCurState ();

				//Find Presentation Controller
				var presContObj = GameObject.FindGameObjectWithTag("PresentationController");
				PresentationController presCont = presContObj.GetComponent<PresentationController>();

				travelState.Destination = presCont.GetCrowdPosition();
				travelState.NextState = "CrowdState";
			}
		} 
		else 
		{
			if(!interested)
			{
				var crowdState = (CrowdState) _stateMachine.Get ("CrowdState");
				if(!crowdState.IsReadyToLeave())
					return;
				if (stateName != "TravelState")
					_stateMachine.Set ("TravelState");
				TravelState travelState = (TravelState)_stateMachine.GetCurState ();
				travelState.Destination = _originalPosition;
				travelState.NextState = "TownState";
			}
		}
	}

	/// <summary>
	/// Travel to the specified Destination and change to set state afterwards.
	/// </summary>
	/// <param name="Destination">Destination.</param>
	/// <param name="NextState">Next state.</param>
	public void Travel(Vector3 Destination, string NextState)
	{
		string stateName = _stateMachine.GetCurState ().Name;
		if (stateName != "TravelState")
			_stateMachine.Set ("TravelState");
		TravelState travelState = (TravelState)_stateMachine.GetCurState ();
		travelState.Destination = Destination;
		travelState.NextState = NextState;
	}

	public void LookAt(Transform obj)
	{
		//Create a dummy that looks directly at the target
		_target.position = _neck.position;
		_target.rotation = _neck.rotation;
		_target.LookAt(obj.position);

		//Difference between target angle and neutral look angle
		float a = Mathf.Abs(_target.rotation.eulerAngles.y - _center.rotation.eulerAngles.y);

		//If the target angle is more than 70 degrees off from neutral angle, look at neutral angle
		if (a >= 70)
		{
			_target.position = _center.position;
			_target.rotation = _center.rotation;
		}
		//Offset rotation if necessary
		_target.Rotate (RotOffset);
		//Rotate towards destination
		_neck.rotation = Quaternion.RotateTowards(_neck.rotation, _target.rotation, MaxRot);
		return;
	}

	/// <summary>
	/// Turns NPC to target
	/// </summary>
	/// <returns>The amount by which the NPC has turned</returns>
	/// <param name="obj">Target.</param>
	/// <param name="maxRot">Max rotation.</param>
	public float TurnTo(Vector3 target)
	{
		//Create a dummy that looks directly at the target
		_target.position = transform.position;
		_target.rotation = transform.rotation;
		_target.LookAt(target);

		//Ignore any rotation on all axes but Y
		var newRot = _target.rotation.eulerAngles;
		newRot.x = 0;
		newRot.z = 0;
		_target.rotation = Quaternion.Euler (newRot);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, _target.rotation, MaxRot * 3);
		return newRot.y - transform.rotation.eulerAngles.y;
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
		Personality.LookedAt ();
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

	public State<NPCController> GetState(string state)
	{
		return _stateMachine.Get (state);
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
