using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Paint_Car : NetworkBehaviour {

	private Impact_Effects damageEffects;
	public GameObject carBody;
	private AudioSource autoRepairAudioSource;
	private GameObject doorCol;

	public List<Texture> textureList;
	private Dictionary<int, Texture> textures = new Dictionary<int, Texture> ();

	[SyncVar (hook = "UpdateDisplayedTexture")]
	public int currentTexture; //each car prefab has a different currentTexture int value

	private bool canPaint;

	void Start()
	{
		damageEffects = GetComponent<Impact_Effects> ();
		canPaint = true;
		for (int i = 0; i < textureList.Count; i++) {
			textures.Add (i, textureList [i]);
		}

		if (gameObject.tag == "Suspect" || gameObject.tag == "AI Car")
		{
			if(!isServer)
			{
				int randTex = Random.Range (0, textureList.Count);
				CmdPaint (randTex);
			}
		}
	}

	void OnTriggerEnter(Collider hit)
	{
		if (isLocalPlayer)
		{
			if(hit.gameObject.name == "colliderToOpenDoor")
			{
				autoRepairAudioSource = hit.transform.parent.GetComponent<AudioSource> ();
				doorCol = hit.gameObject;
			}
			else if(hit.gameObject.name == "colliderToChangeTexture" && canPaint)
			{
				//aqui rola um sorteio atraves da lista de textura e escolhe uma aleatoria que não seja a textura atual
				int randTex;
				do
					randTex = Random.Range (0, textureList.Count);
				while (randTex == currentTexture);

				CmdPaint (randTex);
				damageEffects.ResetDamage();
				canPaint = false;
				autoRepairAudioSource.Play();
			}	
		}
	}

	void OnTriggerExit()
	{
		canPaint = true;
	}

	void UpdateDisplayedTexture(int textureIndex)
	{
		carBody.GetComponent<Renderer>().material.mainTexture = textures[textureIndex];
	}

	[Command]
	void CmdPaint(int newTexture)
	{
		currentTexture = newTexture;
	}

	[ClientRpc]
	void RpcPaint(int newTexture)
	{
		currentTexture = newTexture;
	}
}