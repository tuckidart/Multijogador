using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject player;

	public float offsetX = 15;
	public float offsetZ = 15;
	
	// Update is called once per frame
	void Update () {
		
		if(player != null)
			transform.position = new Vector3 (player.transform.position.x + offsetX, transform.position.y, player.transform.position.z - offsetZ);
	}
}