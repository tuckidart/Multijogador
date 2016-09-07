﻿using UnityEngine;
using System.Collections;

public class Radar : MonoBehaviour {

	public float radius;
	private GameObject objFound;
	private GameObject highlightedObj;
	public Shader highlighted;
	public Shader notHighlighted;
	public float distanceToLoseSuspect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetButtonDown ("Jump"))
			RadarScan ();

		if(objFound != null)
			if(Vector3.Distance(transform.position, objFound.transform.position) > distanceToLoseSuspect)
			{
				highlightedObj.GetComponent<Renderer>().material.shader = notHighlighted;
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

					Debug.Log ("VAGABUNDO ENCONTRADO!");
				}
			}
		}
	}
}