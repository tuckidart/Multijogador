using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

	public float secondsToDestroy;

	// Use this for initialization
	void Start () {
		Invoke ("SelfDestroy", secondsToDestroy);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SelfDestroy()
	{
		Destroy (gameObject);
	}
}
