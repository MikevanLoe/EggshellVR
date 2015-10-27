using UnityEngine;
using System.Collections;

public class FloatUpAndDestroy : MonoBehaviour {
	public TextMesh PickupText;
	public float Lifetime;
	public float Deathtime;
	public float Floatspeed;

	void OnEnable()
	{
		//Destroy self after a set amount of time
		Deathtime = Time.time + Lifetime;
		Destroy (gameObject, Lifetime);
	}

	void Update () {
		//Move the object up by a bit every update
		Vector3 newPos = transform.position;
		newPos.y += Floatspeed * Time.deltaTime;
		transform.position = newPos;
		if (PickupText != null) 
		{
			//Make the text slowly fade out
			Color col = PickupText.color;
			float prog = 1 + 1 / Lifetime * (Time.time - Deathtime);
			col.a = Mathf.Lerp(1, 0, prog);
			PickupText.color = col;
		}
	}
}
