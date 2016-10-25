using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject player;

	public float offsetX;
	public float offsetZ;

	[Range(0f,3f)]
	public float FieldOfViewEffect = 0f;

	public float speedEffect = 0f;

	private Camera myCam;
	private float initFOV;
		
	// Use this for initialization
	void Start () 
	{
		Application.targetFrameRate = 60;
		myCam = GetComponent<Camera>();
		initFOV = myCam.fieldOfView;
	}

	void Update () {
		
		if (player != null)
		{
			//transform.LookAt (player.transform.position);
			transform.position = new Vector3 (player.transform.position.x + offsetX, transform.position.y, player.transform.position.z - offsetZ);
		}

		//pinch camera based on speed of vehicle//
		float fov = initFOV + (player.GetComponent<vehicleController>().zVel*FieldOfViewEffect);
		Mathf.Clamp(fov, initFOV, 169.9f);

		myCam.fieldOfView = fov;
		if(myCam.fieldOfView > 169.9f)
		{
			myCam.fieldOfView = 169.9f;
		}
	}
}