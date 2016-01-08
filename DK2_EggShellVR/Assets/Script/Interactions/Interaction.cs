using UnityEngine;

public abstract class Interaction
{
	public float Duration;
	public string WaitButton;
	
	public abstract void Execute();
	public virtual void Finish(){}
	public virtual void Cancel(){}
}