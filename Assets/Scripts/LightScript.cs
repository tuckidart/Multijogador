using UnityEngine;
using System.Collections;

public class LightScript : MonoBehaviour {

	public bool isGreen;

	void Awake ()
	{
		isGreen = false;
	}

	public void ToggleLight ()
	{
		if (isGreen)
			isGreen = false;
		else
			isGreen = true;
	}
}
