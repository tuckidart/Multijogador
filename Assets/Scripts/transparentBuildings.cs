using UnityEngine;
using System.Collections;

public class transparentBuildings : MonoBehaviour {

	public GameObject player;

	private RaycastHit hit;
	public Material transparentMaterial;
	private Material originalMaterial;
	private GameObject lastHit;

	private Renderer rend;
	private bool transparent;
	// Update is called once per frame
	void Update () {
		
		Ray ray = new Ray (Camera.main.transform.position, player.transform.position - Camera.main.transform.position);
		Debug.DrawRay(Camera.main.transform.position, player.transform.position - Camera.main.transform.position);
		if(Physics.Raycast(ray, out hit))
		{
			if (hit.transform.tag == "Building")
			{
				if (!transparent)
				{
					lastHit = hit.transform.gameObject;
					rend = hit.transform.gameObject.GetComponent<Renderer> ();
					originalMaterial = rend.sharedMaterial;
					rend.sharedMaterial = transparentMaterial;
					transparent = true;
				}
				//Debug.Log ("DETECTEI OBJETO! " + hit.transform.gameObject.name);
			}
			if(lastHit != null)
			{
				if(hit.transform.gameObject.name != lastHit.gameObject.name)
				{
					rend = lastHit.gameObject.GetComponent<Renderer> ();
					rend.sharedMaterial = originalMaterial;
					transparent = false;
				}
			}
		}
	}
}
