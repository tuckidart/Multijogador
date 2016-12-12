using UnityEngine;
using System.Collections;

public class carSound : MonoBehaviour {

	private AudioSource engine;

	// Use this for initialization
	void Start ()
	{
		engine = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		engine.pitch = 0.3f * GetComponent<vehicleController> ().zVel;
		if (engine.pitch < 1)
			engine.pitch = 1;
		else if (engine.pitch > 3)
			engine.pitch = 3;
	}
}
