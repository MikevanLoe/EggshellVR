
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
			return new Tutorial(name, text, time);
		case "QueueScene" :
			return new QueueScene(name);
		case "InvLock" :
			return new InvLock();
		case "InvUnlock" :
			return new InvUnlock();
		case "OpenMenu" :
			return new OpenMenu();
		case "CloseMenu" :
			return new CloseMenu();
		case "AddObjective" :
			return new AddObjective(name);
		case "CompleteObjective" :
			return new CompleteObjective(name);
		case "RollEnding" :
			return new RollEnding();
		case "OpenPresMenu" :
			return new OpenPresMenu();
		case "ClosePresMenu" :
			return new ClosePresMenu();
		case "MoveToCrowd" :
			return new MoveToCrowd();
		case "MovePresDude" :
			return new MovePresDude();
		case "TriggerAnim" :
			return new TriggerAnim(name, text);
		default:
			throw new System.Exception("JSON format error! Script key \"" + key + "\" not found.");
		}
	}
}