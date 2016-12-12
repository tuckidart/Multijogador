using UnityEngine;
using System.Collections;

public class ScapePointScript : MonoBehaviour {

	public float secondsToWait;

	private bool hasExited;

	void OnTriggerEnter (Collider other)
	{
		if (other.transform.parent.parent.gameObject.tag == "Suspect") 
		{
			//_GameMaster.GM.myObjectivesController.CallSupectScaped ();
			Invoke ("CallSuspectEscaped", secondsToWait);
			hasExited = false;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.transform.parent.parent.gameObject.tag == "Suspect") 
		{
			hasExited = true;
		}
	}

	private void CallSuspectEscaped ()
	{
		if (hasExited == false) 
			_GameMaster.GM.myObjectivesController.CallSupectScaped ();
	}
}
