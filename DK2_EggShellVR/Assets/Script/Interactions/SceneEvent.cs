
public abstract class SceneEvent : Interaction
{
	public static SceneEvent GetScript(string key, string name, float time, string text)
	{
		switch (key) {
		case "LockPlayer" :
			return new LockPlayer();
		case "UnlockPlayer" :
			return new UnlockPlayer();
		case "Finished" :
			return new Finished(name);
		case "Tutorial" :
			return new Tutorial(name, text);
		case "QueueScene" :
			return new QueueScene(name);
		case "InvLock" :
			return new InvLock();
		case "InvUnlock" :
			return new InvUnlock();
		default:
			throw new System.Exception("JSON format error! Script key not found.");
		}
	}
}