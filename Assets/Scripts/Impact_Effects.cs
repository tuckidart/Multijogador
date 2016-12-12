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

	public AudioSource crash;
	public AudioClip[] crashType;

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
		}
	}

	void OnTriggerEnter(Collider hit)
	{
		if(hit.gameObject.name == "Water")
		{
			Debug.Log ("cai na agua");
			CmdTakeDamage (carhealth);
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

	[Command]
	void CmdDestroyCar()
	{
		//fazer explosão, etc...
		gameObject.GetComponent<vehicleController>().alive = false;
		carhealth = 0;

		if (gameObject.tag == "Cop")
			_GameMaster.GM.myObjectivesController.CallCarDied (true);
		else if (gameObject.tag == "Suspect")
			_GameMaster.GM.myObjectivesController.CallCarDied (false);

		//Destroy (gameObject);
	}

	[ClientRpc]
	void RpcDestroyCar()
	{
		//fazer explosão, etc...
		gameObject.GetComponent<vehicleController>().alive = false;
		carhealth = 0;

		if (gameObject.tag == "Cop")
			_GameMaster.GM.myObjectivesController.CallCarDied (true);
		else if (gameObject.tag == "Suspect")
			_GameMaster.GM.myObjectivesController.CallCarDied (false);

		//Destroy (gameObject);
	}

	void CheckCarHealth()
	{
		CmdTakeDamage (damageTaken);
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
			smokeTransform.GetComponent<AudioSource> ().Play ();
			break;
		}

		particle.transform.parent = transform;
	}

	[Command]
	public void CmdTakeDamage(float dmg)
	{
		carhealth -= dmg;
		RpcPlayCollisionSound (dmg);

		if (carhealth < 70 && !createdSmokeLow)
		{
			RpcCreateParticle (1);
			createdSmokeLow = true;
		}
		else if (carhealth < 40 && !createdSmokeHigh)
		{
			RpcCreateParticle (2);
			createdSmokeHigh = true;
		}
		else if (carhealth < 20 && !createdFire)
		{
			RpcCreateParticle (3);
			createdFire = true;
		}
		else if (carhealth <= 0)
		{
			RpcDestroyCar ();
		}
	}

	[ClientRpc]
	void RpcPlayCollisionSound(float dmg)
	{
		if(dmg > 10 && dmg < 15)
		{
			crash.clip = crashType[0];
		}
		else if(dmg > 15 && dmg < 25)
		{
			crash.clip = crashType[1];
		}
		else if(dmg > 25)
		{
			crash.clip = crashType[2];
		}
		crash.Play ();
	}

	void UpdateHealth(float newHealth)
	{
		carhealth = newHealth;
	}
}