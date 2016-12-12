using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

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

	[SyncVar (hook = "UpdateHealth")]
	public float carhealth;
	private float damageTaken;

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
		if (hit.relativeVelocity.magnitude > 10.0f && hit.gameObject.name != "Ground" && hit.gameObject.name != "Curb" )
		{
			damageTaken = damageConstant * hit.relativeVelocity.magnitude;

			CheckCarHealth ();

			if (isLocalPlayer)
			{
				minimap.GetComponent<bl_MiniMap> ().DoHitEffect ();
				cam.GetComponent<cameraShake> ().Shake ();
			}
			//GetComponent<AudioSource> ().Play();
		}
	}

	public void ResetDamage()
	{
		Destroy (particle);
		createdSmokeLow = false;
		createdSmokeHigh = false;
		createdFire = false;
		carhealth = maxCarHealth;
	}

	void DestroyCar(GameObject car)
	{
		//fazer explosão, etc...
		car.GetComponent<vehicleController>().alive = false;
		carhealth = 0;

		if (car.tag == "Cop")
			_GameMaster.GM.myObjectivesController.CallCarDied (true);
		else if (car.tag == "Suspect")
			_GameMaster.GM.myObjectivesController.CallCarDied (false);

		//Destroy (gameObject);
	}

	void CheckCarHealth()
	{
		CmdTakeDamage (damageTaken);

//		if (carhealth < 70 && !createdSmokeLow)
//		{
//			if (isLocalPlayer)
//				CmdCreateParticle (1);
//			else if(isServer)
//				RpcCreateParticle (1);
//			createdSmokeLow = true;
//		}
//		else if (carhealth < 40 && !createdSmokeHigh)
//		{
//			if (isLocalPlayer)
//				CmdCreateParticle (2);
//			else if(isServer)
//				RpcCreateParticle (2);
//			createdSmokeHigh = true;
//		}
//		else if (carhealth < 20 && !createdFire)
//		{
//			if (isLocalPlayer)
//				CmdCreateParticle (3);
//			else if(isServer)
//				RpcCreateParticle (3);
//			createdFire = true;
//		}
//		else if (carhealth <= 0)
//		{
//			DestroyCar ();
//		}
	}

	[Command]
	public void CmdCreateParticle(int valor)
	{		
		if (particle != null)
			Destroy (particle);
		
		switch(valor)
		{
		case 1:
			particle = Instantiate (smokeLowPrefab, smokeTransform.position, Quaternion.identity) as GameObject;
			break;
		case 2:
			particle = Instantiate (smokeHighPrefab, smokeTransform.position, Quaternion.identity) as GameObject;
			break;
		case 3:
			particle = Instantiate (firePrefab, smokeTransform.position, Quaternion.identity) as GameObject;
			break;
		}

		particle.transform.parent = transform;
		NetworkServer.Spawn (particle);
	}

	[ClientRpc]
	public void RpcCreateParticle(int valor)
	{
		if (particle != null)
			Destroy (particle);

		switch(valor)
		{
		case 1:
			particle = Instantiate (smokeLowPrefab, smokeTransform.position, Quaternion.identity) as GameObject;
			break;
		case 2:
			particle = Instantiate (smokeHighPrefab, smokeTransform.position, Quaternion.identity) as GameObject;
			break;
		case 3:
			particle = Instantiate (firePrefab, smokeTransform.position, Quaternion.identity) as GameObject;
			break;
		}

		particle.transform.parent = transform;
	}

	[Command]
	public void CmdTakeDamage(float dmg)
	{
		carhealth -= dmg;
		if (carhealth <= 0)
		{
			DestroyCar (transform.gameObject);
		}
	}

	[ClientRpc]
	void RpcTakeDamage()
	{

	}

	void UpdateHealth(float newHealth)
	{
		carhealth = newHealth;
	}
}