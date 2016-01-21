using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class ResetScript : MonoBehaviour {
	public Transform ResetPos;
	public UnityEngine.UI.Image Image;
	public bool running;
	public bool cancel;

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;
		if (running)
			cancel = true;
		StartCoroutine ("Fade");
	}

	IEnumerator Fade()
	{
		running = true;
		while (Image.color.a < 1) {
			if(cancel)
			{
				cancel = false;
				return true;
			}
			Color c = Image.color;
			c.a += Time.deltaTime;
			Image.color = c;
			yield return new WaitForEndOfFrame();
		}
		var player = GameObject.FindGameObjectWithTag ("Player");
		player.transform.position = ResetPos.position;
		player.GetComponent<RigidbodyFirstPersonController> ().LockedInput = true;
		while (Image.color.a  > 0) {
			if(cancel)
			{
				cancel = false;
				return true;
			}
			Color c = Image.color;
			c.a -= Time.deltaTime;
			Image.color = c;
			yield return new WaitForEndOfFrame();
		}
		player.GetComponent<RigidbodyFirstPersonController> ().LockedInput = false;
		if(cancel)
		{
			cancel = false;
			return true;
		}
		running = false;
	}
}
