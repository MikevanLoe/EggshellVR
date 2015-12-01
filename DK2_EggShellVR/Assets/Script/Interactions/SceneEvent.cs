
public abstract class SceneEvent : Interaction
{
	public static SceneEvent GetScript(string key, float time)
	{
		switch (key) {
		case "LockPlayer" :
			return new LockPlayer(time);
		case "UnlockPlayer" :
			return new UnlockPlayer(time);
		default:
			throw new System.Exception("JSON format error! Script key not found.");
		}
	}
}