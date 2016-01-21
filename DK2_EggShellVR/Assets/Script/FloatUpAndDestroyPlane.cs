using UnityEngine;
using System.Collections;

public class FloatUpAndDestroyPlane : MonoBehaviour {
	public Material FadeMaterial;
	public float Lifetime;
	public float Floatspeed;
	
	private float Deathtime;

	void OnEnable()
	{
		//Destroy self after a set amount of time
		Deathtime = Time.time + Lifetime;
		Destroy (gameObject, Lifetime);

		
		Color col = FadeMaterial.color;
		col.a = 1;
		FadeMaterial.color = col;
	}
	
	void Update () {
		//Move the object up by a bit every update
		Vector3 newPos = transform.position;
		newPos.y += Floatspeed * Time.deltaTime;
		transform.position = newPos;
		if (FadeMaterial != null) 
		{
			//Make the text slowly fade out
			Color col = FadeMaterial.color;
			float prog = 1 + 1 / Lifetime * (Time.time - Deathtime);
			col.a = Mathf.Lerp(1, 0, prog);
			FadeMaterial.color = col;
		}
	}
}
