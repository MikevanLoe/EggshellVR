using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	public List<AudioClip> audioClips;

	private Dictionary<string, AudioClip> VoiceClips;
	private Dictionary<string, bool> Switches;
	private Dictionary<string, float> Variables;

	//Awake is called before start
	void Awake () {
		Cursor.visible = false;

		//Convert voice clips to a managable format
		VoiceClips = new Dictionary<string, AudioClip> ();
		foreach (AudioClip a in audioClips) 
		{
			VoiceClips.Add(a.name, a);
		}

		Switches = new Dictionary<string, bool> ();
		Variables = new Dictionary<string, float> ();
	}

	void Update() {
		//Refresh lock state when refresh focus
		if (Input.GetMouseButtonDown(0)	) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	//Get clip from dictionary by name
	public AudioClip GetClip(string name)
	{
		AudioClip retClip;
		if (VoiceClips.TryGetValue (name, out retClip))
			return retClip;
		throw new UnityException ("Clip searched not found");
	}

	//Get global switch
	public bool GetSwitch(string key)
	{
		bool retSwitch;
		if (Switches.TryGetValue (key, out retSwitch))
			return retSwitch;
		return false;
	}

	//Get global variable
	public float GetVariable(string key)
	{
		float retVar;
		if (Variables.TryGetValue (key, out retVar))
			return retVar;
		return 0;
	}

	//Set global switch
	public void SetSwitch(string key, bool value)
	{
		Switches [key] = value;
	}

	//Set global variable
	public void SetVariable(string key, float value)
	{
		Variables [key] = value;
	}
}
