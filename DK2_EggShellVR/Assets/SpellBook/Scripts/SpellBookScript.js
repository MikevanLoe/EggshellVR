#pragma strict
var timer : float;
var interval : float;
var curAnimation : String;

function Start () {
	GetComponent.<Animation>().Play("Closed");
	GetComponent.<Animation>()["Closed"].speed = 1;
	GetComponent.<Animation>()["Closed"].wrapMode = WrapMode.Loop;
	interval = 2;
	curAnimation = "Closed";
}
function FixedUpdate () {
	timer += 1 * Time.deltaTime;
	if(timer >= interval) {
		if(curAnimation == "Closed") {
			GetComponent.<Animation>().Play("Open_Flat");
			GetComponent.<Animation>()["Open_Flat"].speed = 0.5;
			GetComponent.<Animation>()["Open_Flat"].wrapMode = WrapMode.Once;
			curAnimation = "Open_Flat";
			timer = 0;
			interval = 2;
		}
		else {
			if(curAnimation == "Open_Flat") {
				GetComponent.<Animation>().Play("Open_Flat");
				GetComponent.<Animation>()["Open_Flat"].time = GetComponent.<Animation>()["Open_Flat"].length;
				GetComponent.<Animation>()["Open_Flat"].speed = -0.5;
				GetComponent.<Animation>()["Open_Flat"].wrapMode = WrapMode.Once;
				curAnimation = "Closed_Temp";
				timer = 0;
				interval = 2;
			}
			else {
				if(curAnimation == "Closed_Temp") {
					GetComponent.<Animation>().Play("Open");
					GetComponent.<Animation>()["Open"].speed = 0.5;
					GetComponent.<Animation>()["Open"].wrapMode = WrapMode.Once;
					curAnimation = "Open";
					timer = 0;
					interval = 2;
				}
				else {
					if(curAnimation == "Open") {
						GetComponent.<Animation>().Play("Open");
						GetComponent.<Animation>()["Open"].time = GetComponent.<Animation>()["Open"].length;
						GetComponent.<Animation>()["Open"].speed = -0.5;
						GetComponent.<Animation>()["Open"].wrapMode = WrapMode.Once;
						curAnimation = "Closed";
						timer = 0;
						interval = 2;
					}
				}
			}
		}
	}
}