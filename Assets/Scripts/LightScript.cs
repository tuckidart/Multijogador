using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LightScript : NetworkBehaviour {

	[SyncVar]
	public bool isGreen;
	public Light lightColor;
	private Color red = new Color (255, 0, 0);
	private Color yellow = new Color (255, 255, 0);
	private Color green = new Color (0, 255, 0);
	//private float initialY;

	void Awake ()
	{
		isGreen = false;
		lightColor.color = red;
		lightColor.transform.localPosition = new Vector3 (lightColor.transform.localPosition.x, 6.8f, lightColor.transform.localPosition.z);
		//initialY = transform.position.y;
	}
		
	public void ToggleLight ()
	{
		if (isGreen) 
		{
			//transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			lightColor.transform.localPosition = new Vector3 (lightColor.transform.localPosition.x, 6.25f, lightColor.transform.localPosition.z);
			lightColor.color = yellow;

			Invoke ("ToRedLight", 1);
		}
		else 
		{
			//transform.position = new Vector3 (transform.position.x, initialY, transform.position.z);
			Invoke ("ToGreenLight", 1.5f);
		}
	}

	void ToRedLight()
	{
		lightColor.transform.localPosition = new Vector3 (lightColor.transform.localPosition.x, 6.8f, lightColor.transform.localPosition.z);
		lightColor.color = red;
		isGreen = false;
	}
	void ToGreenLight()
	{
		lightColor.transform.localPosition = new Vector3 (lightColor.transform.localPosition.x, 5.75f, lightColor.transform.localPosition.z);
		lightColor.color = green;
		isGreen = true;
	}
}
