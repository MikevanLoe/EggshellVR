using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnZone :MonoBehaviour {

	public List<Transform> fishes = new List<Transform> ();
	public Transform area;
	public float xMin;
	public float xMax;
	public float zMin;
	public float zMax;

	void OnEnable ()
	{
		xMin = area.position.x - (area.localScale.x / 2);
		xMax = area.position.x + (area.localScale.x / 2);
		zMin = area.position.z - (area.localScale.z / 2);
		zMax = area.position.z + (area.localScale.z / 2);
	}
}
