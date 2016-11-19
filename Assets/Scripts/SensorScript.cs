using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public enum ObstacleType 
{
	NULL, car, light, curve
};
	
public class SensorScript : NetworkBehaviour {

	private State myCarState;
	public Transform min;
	public Transform max;

	private vehicleController controller;

	public float carVel;
	private float maxVel;

	private float scaleAux;
	private float positionAux;
	private float initialZScale;
	private float initialZPosition;

	Vector3 newScale;
	Vector3 newPosition;

	void Start () 
	{
		controller = transform.parent.GetComponent<vehicleController>();
		myCarState = transform.parent.GetComponent<State>();

		maxVel = 20.0f;
		scaleAux = max.localScale.z - min.localScale.z;
		positionAux = max.position.z - min.position.z;
		initialZScale = transform.localScale.z;
		initialZPosition = transform.localPosition.z;
	}

	void Update()
	{
		if (!isServer)
			enabled = false;
		//adjust sensorSize and position accordingly to car's velocity
		carVel = controller.zVel;
		InterpolateTransform ();
	}

	void InterpolateTransform()
	{
		float x = carVel / maxVel;

		float newZScale = initialZScale + (scaleAux*x);
		newScale = new Vector3 (transform.localScale.x, transform.localScale.y, newZScale);
		transform.localScale = newScale;

		float newZPos = initialZPosition + (positionAux*x);
		newPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, newZPos);
		transform.localPosition = newPosition;
	}

	void OnTriggerEnter (Collider other)
	{
		//Put any other needed tags here
		if (other.tag == "Car")
		{
			myCarState.ReceiveObstacle (other.transform, ObstacleType.car);
		}
		else if (other.tag == "Curve")
		{
			myCarState.ReceiveObstacle (other.transform, ObstacleType.curve);

		}
		else if (other.tag == "Light")
		{
			myCarState.ReceiveObstacle (other.transform, ObstacleType.light);
		}
	}

	void OnTriggerExit (Collider other)
	{
		//Put any other needed tags here
		if (other.tag == "Car" || other.tag == "Curve" || other.tag == "Light")
		{
			myCarState.RemoveObstacle (other.transform);
		}
	}
}