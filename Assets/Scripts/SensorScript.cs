using UnityEngine;
using System.Collections;

public enum ObstacleType 
{
	car, light, curve
};
	
public class SensorScript : MonoBehaviour {

	private State myCarState;

	void Start () 
	{
		myCarState = transform.parent.GetComponent<State> ();
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
