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

	public virtual void Exit(){}

	public abstract bool Handle();
}