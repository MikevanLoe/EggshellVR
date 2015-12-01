
public abstract class SceneEvent : Interaction
{
	public static SceneEvent GetScript(string key, float time)
	{
		switch (key) {
		case "LockPlayer" :
			return new LockPlayer(time);
			break;
		case "UnlockPlayer" :
			return new UnlockPlayer(time);
			break;
		default:
			throw new System.Exception("JSON format error! Script key not found.");
		}
	}
}