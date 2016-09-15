using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Impact_Effects : NetworkBehaviour {
	
	private GameObject minimap;
	private GameObject cam;

	public Transform smokeTransform;
	public GameObject smokeLowPrefab;
	public GameObject smokeHighPrefab;
	public GameObject firePrefab;

	//variaveis auxiliares de controle de partículas
	private GameObject particle;
	private bool createdSmokeLow = false;
	private bool createdSmokeHigh = false;
	private bool createdFire = false;
	///////////////////////////////////////

	[SyncVar]
	public float carhealth;
	public float maxCarHealth = 100.0f;

	private float damageConstant = 1.0f;

	// Use this for initialization
	void Start () {
		minimap = GameObject.Find ("MiniMap").gameObject;
		cam = GameObject.FindGameObjectWithTag ("MainCamera").gameObject;

		carhealth = maxCarHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision hit)
	{
		if (isLocalPlayer)
		{
			if (hit.relativeVelocity.magnitude > 10.0f && hit.gameObject.name != "Ground")
			{
				//audio de batida
				minimap.GetComponent<bl_MiniMap> ().DoHitEffect ();
				cam.GetComponent<cameraShake> ().Shake ();
				carhealth -= damageConstant * hit.relativeVelocity.magnitude;
			}
		}
			
		CmdCreateParticle ();
	}
		
	void DestroyCar()
	{
		//fazer explosão, etc...
		//gameObject.GetComponent<vehicleController>().alive = false;

		//Destroy (gameObject);
	}

	public void ResetDamage()
	{
		Destroy (particle);
		createdSmokeLow = false;
		createdSmokeHigh = false;
		createdFire = false;
		carhealth = maxCarHealth;
	}

	[Command]
	public void CmdCreateParticle()
	{
		if(carhealth < 70 && !createdSmokeLow)
		{
			particle = Instantiate (smokeLowPrefab, smokeTransform.position, Quaternion.identity) as GameObject;
			particle.transform.parent = transform;
			createdSmokeLow = true;
			NetworkServer.Spawn (particle);
		}
		else if(carhealth < 50 && !createdSmokeHigh)
		{
			Destroy (particle);
			particle = Instantiate (smokeHighPrefab, smokeTransform.position, Quaternion.identity) as GameObject;
			particle.transform.parent = transform;
			createdSmokeHigh = true;
			NetworkServer.Spawn (particle);
		}
		else if(carhealth < 25 && !createdFire)
		{
			Destroy (particle);
			particle = Instantiate (firePrefab, smokeTransform.position, Quaternion.identity) as GameObject;
			particle.transform.parent = transform;
			createdFire = true;
			NetworkServer.Spawn (particle);
		}
		else if(carhealth <= 0)
		{
			carhealth = 0;
			DestroyCar ();
		}
	}
}
