using System;
using System.Collections.Generic;
using UnityEngine;
public class StateMachine
{
	private List<State> _states;
	private State _curState;

	public StateMachine ()
	{
		_states = new List<State> ();
	}

	public StateMachine(List<State> s)
	{
		_states = s;
	}

	public void Add(State s)
	{
		_states.Add (s);
		if (_curState == null)
			_curState = s;
	}

	public bool Set(string name)
	{
		foreach (var s in _states) 
		{
			if(s.Name == name)
			{
				_curState = s;
				return true;
			}
		}
		Debug.Log ("State named \"" + name + "\" not found.");
		return false;
	}

	public State GetCurState()
	{
		return _curState;
	}

	public bool Handle(){
		if (_curState == null) 
		{
			Debug.Log("Handle called without state set");
			return false;
		}
		return _curState.Handle ();
	}
}