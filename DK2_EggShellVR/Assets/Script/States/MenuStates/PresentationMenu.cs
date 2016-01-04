using UnityEngine;
using System;
using System.Linq;

public class PresentationMenu : MonoBehaviour
{	
	private PresentationController _presController;
	private HeartrateReader _hrReader;
	private Transform _heartBar;
	private Transform _convoBar;
	private Transform _lookBar;
	private Transform _timerBar;
	private TextMesh _gradeText;

	private bool _started = false;
	private float _heartScale;
	private float _heartPos;
	private float _convoScale;
	private float _convoPos;
	private float _lookScale;
	private float _lookPos;
	private float _timerScale;
	private float _timerPos;

	void Start()
	{
		//Obtain all external objects
		var pres = GameObject.FindGameObjectWithTag ("PresentationController");
		if(pres == null)
		{
			Debug.Log("No object with PresentationController tag in scene. Just saying, in case you meant for there to be one. Because, uhh. Well, there's not.");
			return;
		}
		_presController = pres.GetComponent<PresentationController> ();

		var gc = GameObject.FindGameObjectWithTag ("GameController");
		_hrReader = gc.GetComponent<HeartrateReader> ();
		
		_heartBar = transform.FindChild ("Left/HeartBar/HeartFill");
		_convoBar = transform.FindChild ("Left/ConvoBar/ConvoFill");
		_lookBar = transform.FindChild ("Left/LookBar/LookFill");
		_timerBar = transform.FindChild ("TimerBar/TimerFill");
		if(_heartBar == null || _convoBar == null || _lookBar == null)
			throw new UnityException("Could not find bars. Make sure bars exist inside of ItemMenu at the correct path.");

		var gradeObj = transform.FindChild ("Left/Grade");
		if (gradeObj == null)
			throw new UnityException ("Could not find Grade object.");
		_gradeText = gradeObj.GetComponent<TextMesh> ();

		//Set all original scales and positions
		_heartScale = _heartBar.localScale.x;
		_heartPos = _heartBar.localPosition.x;
		_convoScale = _convoBar.localScale.x;
		_convoPos = _convoBar.localPosition.x;
		_lookScale = _lookBar.localScale.x;
		_lookPos = _lookBar.localPosition.x;
		_timerScale = _timerBar.localScale.x;
		_timerPos = _timerBar.localPosition.x;
	}
	
	void Update()
	{
		//If presentation is currently being given
		if (_presController.presState != PresentationController.PresentationState.Idle) {
			if(!_started)
				_started = true;
			//Get and set the score
			float heartScore = 1 - _hrReader.CalculateMeterScale ();
			ResizeBar (_heartBar, _heartScale, _heartPos, heartScore);
			
			float convScore = _presController.GetConvScore ();
			ResizeBar (_convoBar, _convoScale, _convoPos, convScore);
			
			float lookScore = _presController.GetLookScore ();
			ResizeBar (_lookBar, _lookScale, _lookPos, lookScore);
			
			//float timerFactor = _presController.GetTimerFactor ();
			//ResizeBar (_timerBar, _timerScale, _timerPos, timerFactor);
		} else {
			if(!_started)
				return;
			_started = false;
			//Get and set the score
			float heartScore = 1 - _hrReader.CalculateMeterScale ();
			ResizeBar (_heartBar, _heartScale, _heartPos, heartScore);
			
			float convScore = _presController.GetConvScore ();
			ResizeBar (_convoBar, _convoScale, _convoPos, convScore);
			
			float lookScore = _presController.GetLookScore ();
			ResizeBar (_lookBar, _lookScale, _lookPos, lookScore);
			
			float timerFactor = 0;
			ResizeBar (_timerBar, _timerScale, _timerPos, timerFactor);

			//The grade is the avarage of all
			float grade = (heartScore + convScore + lookScore) / 3 * 10;
			grade = Mathf.Max (grade, 1);
			grade = Mathf.Round (grade * 10) / 10; 		//Round to one decimal place
			
			_gradeText.text = grade.ToString();
		}
	}

	/// <summary>
	/// Resizes the different bars.
	/// </summary>
	/// <param name="bar">The bar transform.</param>
	/// <param name="orScale">Original scale.</param>
	/// <param name="orPos">Original position.</param>
	/// <param name="factor">Factor of the new size.</param>
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