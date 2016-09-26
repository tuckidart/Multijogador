using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeControl : MonoBehaviour {

	public float durationTime;

	[SerializeField]
	private Text timeText;
	private float startTime;

	private float totalSeconds;
	private int minutes;
	private int seconds;

	private bool timeOver;

	// Use this for initialization
	void Awake () 
	{
		timeOver = false;
		startTime = Time.time;
		//Debug.Log (durationTime);
	}

	void Update ()
	{
		if(!timeOver)
		{
			if (Time.time >= startTime + durationTime) 
			{
				FireEndOfDurationTime ();
			} 
			else
			{
				UpdateTimeUI ();
			}
		}
	}

	private void FireEndOfDurationTime ()
	{
		//Test for conditions
		//Time.timeScale = 0;

		_GameMaster.GM.myObjectivesController.DoTimerOverActions ();
		timeOver = true;
	}

	private void UpdateTimeUI ()
	{
		totalSeconds = (durationTime + startTime - Time.time);
		minutes = (int)(totalSeconds / 60f);
		seconds = (int)totalSeconds - (minutes * 60);

		if (seconds >= 10) 
		{
			timeText.text = minutes.ToString () + ":" + seconds.ToString ();
		} 
		else 
		{
			timeText.text = minutes.ToString () + ":" + "0" + seconds.ToString ();
		}
	}
}
