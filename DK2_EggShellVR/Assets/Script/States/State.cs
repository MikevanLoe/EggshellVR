using System;
public abstract class State<T>
{
	public String Name;

	public T _client;

	public State(T c)
	{
		_client = c;
		Name = this.GetType().ToString();
	}

	public virtual void Enter(){}

	public abstract bool Handle();
}