using UnityEngine;
using System;
using System.Linq;

public class PresentationState : State<MenuController>
{	
	private PresentationController _presController;
	private HeartrateReader _hrReader;
	private Transform _heartBar;
	private Transform _convoBar;
	private Transform _lookBar;
	private TextMesh _gradeText;

	private float _heartScale;
	private float _heartPos;
	private float _convoScale;
	private float _convoPos;
	private float _lookScale;
	private float _lookPos;

	public PresentationState (MenuController c) : base(c)
	{
		_presController = GameObject.FindGameObjectWithTag ("PresentationController")
			.GetComponent<PresentationController> ();
		var gc = GameObject.FindGameObjectWithTag ("GameController");
		_hrReader = gc.GetComponent<HeartrateReader> ();
		
		_heartBar = _client.transform.FindChild ("PresentationMenu/Left/HeartBar/HeartFill");
		_convoBar = _client.transform.FindChild ("PresentationMenu/Left/ConvoBar/ConvoFill");
		_lookBar = _client.transform.FindChild ("PresentationMenu/Left/LookBar/LookFill");
		var gradeObj = _client.transform.FindChild ("PresentationMenu/Left/Grade");
		if (gradeObj == null)
			throw new UnityException ("Could not find Grade object.");
		_gradeText = gradeObj.GetComponent<TextMesh> ();

		if(_heartBar == null || _convoBar == null || _lookBar == null)
			throw new UnityException("Could not find bars. Make sure bars exist inside of ItemMenu at the correct path.");
		
		_heartScale = _heartBar.localScale.x;
		_heartPos = _heartBar.localPosition.x;
		_convoScale = _convoBar.localScale.x;
		_convoPos = _convoBar.localPosition.x;
		_lookScale = _lookBar.localScale.x;
		_lookPos = _lookBar.localPosition.x;
	}
	
	public override bool Handle()
	{
		float heartScore = _hrReader.CalculateMeterScale ();
		ResizeBar (_heartBar, _heartScale, _heartPos, heartScore);

		float convScore = _presController.GetConvScore ();
		ResizeBar (_convoBar, _convoScale, _convoPos, convScore);

		float lookScore = _presController.GetLookScore ();
		ResizeBar (_lookBar, _lookScale, _lookPos, lookScore);

		float grade = (heartScore + convScore + lookScore) / 3 * 10;

		if (Input.GetKeyDown (KeyCode.E)) {
			grade = Mathf.Max (grade, 1);
			grade = Mathf.Round (grade * 10) / 10; 		//Round to one decimal place

			_gradeText.text = grade.ToString();
		}
		return true;
	}

	private void ResizeBar(Transform bar, float orScale, float orPos, float factor)
	{
		factor = Mathf.Max (factor, 0.05f);
		float newScaleX = orScale * factor;
		Vector3 newScale = bar.localScale;
		newScale.x = newScaleX;
		bar.localScale = newScale;

		float deltaPos = (orScale - newScaleX) / 2;
		float newPosX = orPos - deltaPos;
		Vector3 newPos = bar.localPosition;
		newPos.x = newPosX;
		bar.localPosition = newPos;
	}
}