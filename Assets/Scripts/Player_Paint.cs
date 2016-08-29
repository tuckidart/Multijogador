using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Player_Paint : NetworkBehaviour {

	public GameObject carBody;

	public List<Texture> textureList;
	private Dictionary<int, Texture> textures = new Dictionary<int, Texture> ();

	[SyncVar (hook = "UpdateDisplayedTexture")]
	private int currentTexture;

	private bool canPaint;

	void Start()
	{
		canPaint = true;
		for(int i=0;i<textureList.Count;i++)
		{
			textures.Add (i, textureList [i]);
		}
	}
		
	void OnTriggerEnter(Collider hit)
	{
		if (isLocalPlayer)
		{
			if(hit.gameObject.name == "colliderToChangeTexture" && canPaint)
			{
				//aqui rola um sorteio atraves da lista de textura e escolhe uma aleatoria que não seja a textura atual
				int randTex;
				do
					randTex = Random.Range (0, textureList.Count);
				while
					(randTex == currentTexture);
					
				CmdPaint(randTex);
				canPaint = false;
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
}