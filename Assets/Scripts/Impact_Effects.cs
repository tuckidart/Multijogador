using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Impact_Effects : NetworkBehaviour {

	private GameObject minimap;
	private GameObject cam;

	[SyncVar]
	public float carhealth = 100.0f;

	private float damageConstant = 1.0f;

	// Use this for initialization
	void Start () {
		minimap = GameObject.Find ("MiniMap").gameObject;
		cam = GameObject.FindGameObjectWithTag ("MainCamera").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision hit)
	{
		if (isLocalPlayer)
		{
			if (hit.relativeVelocity.magnitude > 10.0f) {
				minimap.GetComponent<bl_MiniMap> ().DoHitEffect ();
				carhealth -= damageConstant * hit.relativeVelocity.magnitude;
			}
		}

		/*if(carhealth < 0)
		{
			destroycar ();
		}*/
	}
}
