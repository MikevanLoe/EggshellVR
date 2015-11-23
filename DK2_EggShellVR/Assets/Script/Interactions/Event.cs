
public abstract class Event : Interaction
{
	public static Event GetScript(string key, float time)
	{
		switch (key) {
		case "LockPlayer" :
			return new LockPlayer(time);
		case "UnlockPlayer" :
			return new UnlockPlayer(time);
		default:
			return null;
		}
	}
}