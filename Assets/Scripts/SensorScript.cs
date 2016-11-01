using UnityEngine;
using System.Collections;

public enum ObstacleType 
{
	car, light, curve
};
	
public class SensorScript : MonoBehaviour {

	private State myCarState;
	private BoxCollider sensorSize;

	void Start () 
	{
		sensorSize = GetComponent<BoxCollider> ();
		myCarState = transform.parent.GetComponent<State>();
	}

	void Update()
	{
		
//		sensorSize.size += new Vector3(0.0f, 0.0f, 0.1f);
//		sensorSize.center += new Vector3(0.0f, 0.0f, 0.1f);
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
			myCarState.RemoveObstacle ();
		}
	}
}
