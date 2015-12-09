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
				_curState.Exit();
				_curState = s;
				_curState.Enter();
				return true;
			}
		}
		Debug.Log ("State named \"" + name + "\" not found.");
		return false;
	}

	public int TryNext()
	{
		int s = _states.FindIndex (st => st == _curState);
		s++;
		if (s < _states.Count) {
			return s;
		}
		return -1;
	}

	public bool Next()
	{
		int s = TryNext ();
		if(s >= 0){
			Set (_states [s].Name);
			return true;
		}
		return false;
	}

	public int TryPrevious()
	{
		int s = _states.FindIndex (st => st == _curState);
		s--;
		if (s < _states.Count && s >= 0) {
			return s;
		}
		return -1;
	}

	public bool Previous()
	{
		int s = TryPrevious ();
		if(s >= 0){
			Set (_states [s].Name);
			return true;
		}
		return true;
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