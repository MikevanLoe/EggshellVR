using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MoveToCrowd : SceneEvent
{
	public MoveToCrowd ()
	{
	}
	
	public override void Execute()
	{
		var ShopOwner = GameObject.Find ("ShopOwner");
		var CoupleF = GameObject.Find ("CoupleF");
		var CoupleM = GameObject.Find ("CoupleM");
		var Rude = GameObject.Find ("Rude");
		var PresDude = GameObject.Find ("PresDude");
		
		ShopOwner.transform.localPosition = new Vector3 (-4.53f, -2.550703f, 2.12f);
		CoupleF.transform.localPosition = new Vector3 (-4.05f, -2.550703f, -0.85f);
		CoupleM.transform.localPosition = new Vector3 (-4.1f, -2.453703f, -1.8f);
		Rude.transform.localPosition = new Vector3 (-2.6f, -1.403f, -6.2f);
		if (!PresDude.GetComponent<NPCController> ().GetSwitch ("Finished2")) 
		{
			PresDude.transform.localPosition = new Vector3 (2.440002f, -2.453703f, 1.409973f);
			PresDude.transform.rotation = Quaternion.Euler (0, 231.2452f, 0);
		}
	}
}