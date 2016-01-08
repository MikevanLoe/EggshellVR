using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class FishingMinigame : MonoBehaviour
{
	public float RodOffset;

	public bool isRodActive;
	public bool isHookOut;
	public Transform rod;
	public Transform hook;
	public SpawnZone spawnZone;
	public List<Transform> fishList;
	public Animator anim;

	public void Start ()
	{
		fishList = spawnZone.GetComponent<SpawnZone> ().fishes;

		for (int i = 0; i < fishList.Count; i++)
			SpawnFish (fishList[i]);
	}

	public void Update ()
	{
		// Use the rod when the left mouse button is pressed and the rod is active	
		if(Input.GetButtonDown("Fire1") && isRodActive)
			UseRod ();
	}

	public void SpawnFish (Transform spawningFish)
	{
		float xMin = spawnZone.xMin;
		float xMax = spawnZone.xMax;
		float zMin = spawnZone.zMin;
		float zMax = spawnZone.zMax;

		Vector3 randPos = new Vector3(UnityEngine.Random.Range (xMin, xMax), 6.5f,
		                              UnityEngine.Random.Range (zMin, zMax));

		spawningFish.position = randPos;
	}

	public void ActivateRod ()
	{
		if (isHookOut)
			return;
		isRodActive = !isRodActive;
		rod.gameObject.SetActive (isRodActive);
		
		var player = GameObject.FindGameObjectWithTag ("Player");
		rod.position = player.transform.position + transform.forward * RodOffset + transform.right * 0.1f;
		player.GetComponent<RigidbodyFirstPersonController>().LockedInput = isRodActive;

		if (!isRodActive)
		{
			isHookOut = true; 
			UseRod ();
		}
	}

	public void UseRod ()
	{
		isHookOut = !isHookOut;
		anim.SetBool("isHookOut", isHookOut);
	}
}