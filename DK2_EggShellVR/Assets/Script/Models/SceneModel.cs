using System.Collections.Generic;

public class SceneModel
{
	public float Range;
	public List<Interaction> Interactions;

	public SceneModel(float range, List<Interaction> inter)
	{
		Range = range;
		Interactions = inter;
	}
}