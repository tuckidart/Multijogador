using UnityEngine;
using System.Collections;

public class ScapePointScript : MonoBehaviour {

	public float secondsToWait;
	public BarScript barUI;

	private bool hasEntered;

	private float EnteredScapeTime;

	void Start ()
	{
		barUI = GameObject.FindGameObjectWithTag ("ScapeSlider").GetComponent<BarScript> ();
		barUI.maxValue = secondsToWait;
		barUI.updateAutomatically = false;
	}

	void Update ()
	{
		if(hasEntered)
			barUI.SetCurrentValue(Time.time - EnteredScapeTime + 0.25f);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.name == "colbody1")
			if (other.transform.parent.parent.gameObject.tag == "Suspect") 
			{
				EnteredScapeTime = Time.time;
				barUI.TurnChildrenOnOff (true);
				Invoke ("CallSuspectEscaped", secondsToWait);
				hasEntered = true;
			}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.name == "colbody1")
			if (other.transform.parent.parent.gameObject.tag == "Suspect") 
			{
				barUI.TurnChildrenOnOff (false);
				CancelInvoke ();
				hasEntered = false;
			}
	}
		
	private void CallSuspectEscaped ()
	{
		if (hasEntered == true)
			_GameMaster.GM.myObjectivesController.CallSupectScaped ();
	}
}