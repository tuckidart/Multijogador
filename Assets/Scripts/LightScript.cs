using UnityEngine;
using System.Collections;

public class LightScript : MonoBehaviour {

	public bool isGreen;

	public void ToggleLight ()
	{
		if (isGreen)
			isGreen = false;
		else
			isGreen = true;
	}
}
