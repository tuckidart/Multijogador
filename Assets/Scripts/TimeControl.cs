using UnityEngine;
using System.Collections;

public class TimeControl : MonoBehaviour {

	public float durationTime;

	private float startTime;


	// Use this for initialization
	void Start () 
	{
		startTime = Time.time;
		//StartCoroutine (StartCountdownToEndGame (durationTime));
	}

	void Update ()
	{
		if (Time.time >= startTime + durationTime) 
		{
			FireEndOfDurationTime ();
		} 
		else 
		{
			//Debug.Log (durationTime - Time.time + startTime);
		}	
	}

	private void FireEndOfDurationTime ()
	{
		//Test for conditions
		//Time.timeScale = 0;

		_GameMaster.GM.myObjectivesController.DoTimerOverActions ();
	}
}
