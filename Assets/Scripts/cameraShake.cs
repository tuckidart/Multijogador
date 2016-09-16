using UnityEngine;
using System.Collections;

public class cameraShake : MonoBehaviour {

	private Vector3 originalPosition;
	private Quaternion originalRotation;

	private float shakeDecay;
	private float shakeIntensity;

	/*void OnGUI()
	{
		if(GUI.Button (new Rect (20,40,80,20), "Shake"))
		{
			Shake ();
		}
	}*/
	
	// Update is called once per frame
	void Update () {
		if(shakeIntensity > 0)
		{
			transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
			transform.rotation = new Quaternion (
				originalRotation.x + Random.Range (-shakeIntensity, shakeIntensity) * 0.2f,
				originalRotation.y + Random.Range (-shakeIntensity, shakeIntensity) * 0.2f,
				originalRotation.z + Random.Range (-shakeIntensity, shakeIntensity) * 0.2f,
				originalRotation.w + Random.Range (-shakeIntensity, shakeIntensity) * 0.2f);
			shakeIntensity -= shakeDecay;
		}
	}

	public void Shake()
	{
		originalPosition = transform.position;
		originalRotation = transform.rotation;
		shakeIntensity = 0.02f;
		shakeDecay = 0.002f;
	}
}