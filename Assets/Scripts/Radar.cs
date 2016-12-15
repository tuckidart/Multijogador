using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Radar : NetworkBehaviour {

	public float useCost;
	public float radius;
	private GameObject objFound;
	private GameObject highlightedObj;
	public Shader highlighted;
	public Shader notHighlighted;
	public float distanceToLoseSuspect;

	private SirenEffect sirenEffectScript;

	private float cooldownValue;
	private BarScript barUI;

	private bool isGreen;

	// Use this for initialization
	void Start ()
	{
		sirenEffectScript = GetComponent<SirenEffect> ();

		cooldownValue = 0;

		if (isLocalPlayer && gameObject.tag == "Cop")
		{
			barUI = GameObject.FindGameObjectWithTag ("RadarSlider").GetComponent<BarScript> ();
			barUI.maxValue = 100;
			barUI.updateAutomatically = false;
			barUI.TurnChildrenOnOff (true);
			barUI.SetColor (new Color (0.8f, 0f, 0f, 0.8f));
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(isLocalPlayer)
			if (Input.GetButtonDown ("Jump"))
			{
				if(objFound == null)
					RadarScan ();
				else
					sirenEffectScript.CmdToggleSiren(true);
			}

		if(objFound != null)
			if(Vector3.Distance(transform.position, objFound.transform.position) > distanceToLoseSuspect)
			{
				highlightedObj.GetComponent<Renderer>().material.shader = notHighlighted;
				objFound = null;
				sirenEffectScript.CmdToggleSiren(false);
			}
			
		IncreaseCooldown ();
	}

	void RadarScan()
	{
		if (cooldownValue > useCost)
		{
			Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, radius);

			for(int i=0;i<hitColliders.Length;i++)
			{
				if (hitColliders [i].transform.name == "colbody1")
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

			cooldownValue -= useCost;
		}
	}

	private void IncreaseCooldown ()
	{
		if (cooldownValue <= 100f)
			cooldownValue += (0.9f * Time.deltaTime) * 3f;

		if (barUI)
			barUI.SetCurrentValue(cooldownValue);

		if (cooldownValue > useCost && isGreen == false) 
		{
			barUI.SetColor (new Color (0f, 0.8f, 0f, 0.8f));
			isGreen = true;
		}
		else if (cooldownValue < useCost && isGreen == true) 
		{
			barUI.SetColor (new Color (0.8f, 0f, 0f, 0.8f));
			isGreen = false;
		}
	}
}