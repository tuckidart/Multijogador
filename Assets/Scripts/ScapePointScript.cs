using UnityEngine;
using System.Collections;

public class ScapePointScript : MonoBehaviour {

	public float secondsToWait;
	public BarScript barUI;

	private bool hasExited;

	private float EnteredScapeTime;

	void Start ()
	{
		barUI = GameObject.FindGameObjectWithTag ("ScapeSlider").GetComponent<BarScript> ();
		barUI.maxValue = secondsToWait;
		barUI.updateAutomatically = false;
	}

	void Update ()
	{
		barUI.SetCurrentValue(Time.time - EnteredScapeTime + 0.25f); 
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "colbody1")
			if (other.transform.parent.parent.gameObject.tag == "Suspect") 
			{
				EnteredScapeTime = Time.time;
				barUI.TurnChildrenOnOff (true);
				Invoke ("CallSuspectEscaped", secondsToWait);
				hasExited = false;
			}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == "colbody1")
			if (other.transform.parent.parent.gameObject.tag == "Suspect") 
			{
				barUI.TurnChildrenOnOff (false);
				CancelInvoke ();
				hasExited = true;
			}
	}
		
	private void CallSuspectEscaped ()
	{
		if (hasExited == false) 
			_GameMaster.GM.myObjectivesController.CallSupectScaped ();
	}
}
