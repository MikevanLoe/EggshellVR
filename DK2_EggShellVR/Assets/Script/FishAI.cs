using UnityEngine;
using System.Collections;

public class FishAI : MonoBehaviour {
	public Mover3D mover;
	
	private const float delay = 1.5f;
	private const float reelDelay = 0.7f;
	private float hookedTime;
	private float reelTime;
	private bool reel;

	// Update is called once per frame
	void FixedUpdate ()
	{
		//While reeled, await death
		if (reel) {
			if (Time.time > reelTime)
			{
				//Respawn fish
				var fishGame = GameObject.Find("fishingGame").GetComponent<FishingMinigame>();
				transform.parent = null;
				fishGame.SpawnFish(transform);

				//Reset
				reel = false;
			}
			return;
		}
		if (Time.time > hookedTime)
		{
			//When not hooked, make sure fish isn't parented to hook
			transform.parent = null;

			//Calculate force
			Vector3 Force;
			Force = mover.WallAvoidance();
			Force += mover.Wander ();
			//Make sure to never move on Y axis, even when pushed
			Force.y = 0;
			//Execute force and face towards heading
			mover.Accelerate (Force);
			mover.FaceHeading();
		}
		else
		{
			//Do not move when hooked
			this.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

			//If the reel button is pressed while hooked
			if (Input.GetButtonDown ("Fire1") && !reel)
			{
				//Fish can only be reeled once
				reel = true;
				//Make fish respawn after reeling in
				reelTime = Time.time + reelDelay;
				//Reset hooked time
				hookedTime = 0;

				//Add item to player inventory
				var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
				player.AddItem(new ItemModel("Vis",1, "Dit is een verse haring. Hij is \ngevangen in de Njord zee. Hij is\n 35 cm lang en weegt 1 kilo."));

				Debug.Log("FishAdded " + player.FindItem("Vis").Quantity + " " + name);


				//Get the GameController here because it's needed in both if-cases
				var gameContObj = GameObject.FindGameObjectWithTag ("GameController");
				var gameCont = gameContObj.GetComponent<GameController>();
				if(player.HasItem("Vis", 3))
				{
					player.Inventory.Find(i => i.Name == "Vis").Quantity = 3;
					var cutscenecont = gameContObj.GetComponent<CutsceneController> ();
					if(cutscenecont.IsPlaying())
						return;
					//Check if the cutscene already played before
					if(gameCont.GetSwitch("EnoughFish"))
					{
						cutscenecont.PlayCutscene ("TooManyFish");
						player.Inventory.Find(i => i.Name == "Vis").Quantity = 3;
					}
					else
						cutscenecont.PlayCutscene ("EnoughFish");
					gameCont.SetSwitch("EnoughFish", true);
				}
				else
				{
					gameCont.SetSwitch("EnoughFish", false);
				}
			}
		}
	}

	public void Hooked()
	{
		hookedTime = Time.time + delay;
		this.GetComponent<Rigidbody>().velocity = Vector3.zero;
		this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
	}
}
