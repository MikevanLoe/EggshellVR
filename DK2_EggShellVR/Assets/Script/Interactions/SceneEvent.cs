
public abstract class SceneEvent : Interaction
{
	public static SceneEvent GetScript(string key, string name, float time)
	{
		switch (key) {
		case "LockPlayer" :
			return new LockPlayer();
		case "UnlockPlayer" :
			return new UnlockPlayer();
		case "Finished" :
			return new Finished(name);
		default:
			throw new System.Exception("JSON format error! Script key not found.");
		}
	}
}