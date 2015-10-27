using System;
public abstract class State
{
	public String Name;

	protected NPCController _client;

	public State(NPCController c)
	{
		_client = c;
		Name = this.GetType().ToString();
	}

	public abstract bool Handle();
}