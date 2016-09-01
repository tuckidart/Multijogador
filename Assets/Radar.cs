using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Radar : NetworkBehaviour {

	public float radius;
	public GameObject objFound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetButtonDown ("Jump"))
			RadarScan ();
	}

	void RadarScan()
	{
		Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, radius);

		for(int i=0;i<hitColliders.Length;i++)
		{
			if (hitColliders [i].transform.parent.name == "Suspension")
			{
				if(hitColliders [i].transform.parent.parent.gameObject.tag == "Suspect")
				{
					objFound = hitColliders [i].transform.parent.parent.gameObject;
					Debug.Log ("VAGABUNDO ENCONTRADO!");
				}
			}
		}
	}
}