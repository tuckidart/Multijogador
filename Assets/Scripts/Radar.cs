﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Radar : NetworkBehaviour {

	public float radius;
	private GameObject objFound;
	private GameObject highlightedObj;
	public Shader highlighted;
	public Shader notHighlighted;
	public float distanceToLoseSuspect;

	private SirenEffect sirenEffectScript;

	// Use this for initialization
	void Start ()
	{
		sirenEffectScript = GetComponent<SirenEffect> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(isLocalPlayer)
			if (Input.GetButtonDown ("Jump"))
				RadarScan ();

		if(objFound != null)
			if(Vector3.Distance(transform.position, objFound.transform.position) > distanceToLoseSuspect)
			{
				highlightedObj.GetComponent<Renderer>().material.shader = notHighlighted;
				sirenEffectScript.CmdToggleSiren(false);
			}
	}

	void RadarScan()
	{
		Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, radius);

		for(int i=0;i<hitColliders.Length;i++)
		{
			if (hitColliders [i].transform.parent.name == "bodyRef")
			{
				if(hitColliders [i].transform.parent.parent.gameObject.tag == "Suspect")
				{
					objFound = hitColliders [i].transform.parent.gameObject;
					highlightedObj = objFound.transform.Find("body").gameObject;
					highlightedObj.GetComponent<Renderer>().material.shader = highlighted;

					sirenEffectScript.CmdToggleSiren(true);
					Debug.Log ("VAGABUNDO ENCONTRADO!");
				}
			}
		}
	}
}