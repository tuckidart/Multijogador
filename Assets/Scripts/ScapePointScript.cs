using UnityEngine;
using System.Collections;

public class ScapePointScript : MonoBehaviour {

	void OnTriggerEnter (Collider other)
	{
		if (other.transform.parent.parent.gameObject.tag == "Suspect")
			_GameMaster.GM.myObjectivesController.CallSupectScaped ();
	}
}
