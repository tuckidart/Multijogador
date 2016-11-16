using UnityEngine;
using System.Collections;

public class LightScript : MonoBehaviour {

	public bool isGreen;

	private float initialY;

	void Awake ()
	{
		isGreen = false;
		initialY = transform.position.y;
	}

	public void ToggleLight ()
	{
		if (isGreen) 
		{
			transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			isGreen = false;
		}
		else 
		{
			transform.position = new Vector3 (transform.position.x, initialY, transform.position.z);
			isGreen = true;
		}
	}
}
