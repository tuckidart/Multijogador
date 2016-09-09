using UnityEngine;
using System.Collections;

public class TimeControl : MonoBehaviour {

	public float durationTime;

	private float startTime;

	// Use this for initialization
	void Start () 
	{
		startTime = Time.time;
		StartCoroutine (StartCountdownToEndGame (durationTime));
	}

	private IEnumerator StartCountdownToEndGame (float seconds) 
	{
		yield return new WaitForSeconds (seconds);

		FireEndOfDurationTime ();
	}

	private void FireEndOfDurationTime ()
	{
		//Test for conditions
		//Time.timeScale = 0;

		_GameMaster.GM.myObjectivesController.CheckObjetiveVariables ();
	}
}
