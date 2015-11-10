using System;
using System.Collections.Generic;
using UnityEngine;
public class StateMachine<T>
{
	private List<State<T>> _states;
	private State<T> _curState;

	public StateMachine ()
	{
		_states = new List<State<T>> ();
	}

	public StateMachine(List<State<T>> s)
	{
		_states = s;
	}

	public void Add(State<T> s)
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
				_curState.Enter();
				return true;
			}
		}
		Debug.Log ("State named \"" + name + "\" not found.");
		return false;
	}

	public State<T> GetCurState()
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