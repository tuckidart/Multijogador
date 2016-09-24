using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScapeController : MonoBehaviour {

	public int timeToScape;
	public List<GameObject> scapeObjects;

	private int durationTime;
	private float startTime;
	private bool scapeIsOpen;
	private int currentScapeObjectIndex;

	private bool lateStartCalled;

	void Awake ()
	{
		startTime = Time.time;
	}

	// Use this for initialization
	void Start () 
	{
		ChooseCurrentScape ();
		StartCoroutine (LateStart ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!scapeIsOpen && lateStartCalled)
		{
			if ((Time.time - startTime) >=  durationTime -  timeToScape)
			{
				SetScapeToOpen ();
			}
		}
	}

	void ChooseCurrentScape ()
	{
		currentScapeObjectIndex = Random.Range (0, scapeObjects.Count);
	}

	void SetScapeToOpen ()
	{
		scapeIsOpen = true;
		scapeObjects [currentScapeObjectIndex].SetActive (true);

		Debug.Log ("Opened the Scape");
	}

	private IEnumerator LateStart ()
	{
		yield return new WaitForSeconds (1f);
	
		durationTime = (int)_GameMaster.GM.myTimeControl.durationTime;

		lateStartCalled = true;
	}
}
