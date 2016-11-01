using UnityEngine;
using System.Collections;

public class State : MonoBehaviour {

	//=================STATES
	private bool turnedOn;				//Level 1
	//Level 1
	private bool moving;				//Level 2

	//Level 2
	private float acceleration;			// 1 | -1
	private float turning;				// 1 | -1

	private bool parked;				//Used on initialize
	//=================STATES


	//=================SENSORS
	private bool hasObstacle;
	private float obstacleDistance;

	private Transform obstacle;

	private ObstacleType currentObstacleType;
	//=================SENSORS

	private vehicleController controller;

	void Awake () 
	{
		controller = GetComponent<vehicleController> ();
		turnedOn = false;
		moving = false;
	}
	
	void Update () 
	{
		if (hasObstacle)
			CalculateObstacleDistance ();

		//Apply values to the car
		ApplyValues ();
	}

	//public void ReceiveCar ()


	public void ReceiveObstacle (Transform newObstacle, ObstacleType newObstacleType)
	{
		obstacle = newObstacle;
		currentObstacleType = newObstacleType;
		hasObstacle = true;
		HandleObstacle ();
	}

	public void RemoveObstacle ()
	{
		hasObstacle = false;
		obstacle = null;
	}
		
	void ApplyValues ()
	{
		//teste do zerinhoooo!!!
//		controller.inputY += 0.01f;
//		if(controller.inputY > 1.0f)
//			controller.inputY = 1.0f;
//		controller.inputX += 0.01f;
//		if(controller.inputX > 1.0f)
//			controller.inputX = 1.0f;
		//////////////////////////////

		if (obstacle == null)
		{
			controller.inputY += 0.01f;
		}
		else
		{
			if (controller.zVel > 0)
				controller.inputY -= 1.0f / obstacleDistance;
			else
				controller.inputY = 0.0f;
		}
	}

	private void CalculateObstacleDistance ()
	{
		obstacleDistance = Vector3.Distance (transform.position, obstacle.position);
	}

	private void HandleObstacle ()
	{
		if (currentObstacleType == ObstacleType.curve) 
		{
			if (Random.Range(0, 2) == 1)
			{
				if (obstacle.GetComponent<CurveScript>().isRight) 
				{
					//Set turning to +1
					//Compute obstacle multiplier
					Debug.Log ("Curve - Turn right");
				}
				else 
				{
					//Set turning to -1
					//Compute obstacle multiplier
					Debug.Log ("Curve - Turn left");
				}

				obstacle.GetComponent<CurveScript> ().IncreaseNumberOfCarsThatTurned ();
			}
		} 
		else if (currentObstacleType == ObstacleType.light && obstacle.GetComponent<LightScript>().isGreen == false) 
		{
			//Fucking stop
			Debug.Log ("Light - Fucking stop");
		}
		else 
		{
			//Fucking stop? Maybe a little slower
			Debug.Log ("Car - Fucking stop?");
		}
	}
}
