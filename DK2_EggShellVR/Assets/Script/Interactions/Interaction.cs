public abstract class Interaction
{
	public float Duration;
	
	public abstract void Execute();
	public virtual void Finish(){}
	public virtual void Cancel(){}
}