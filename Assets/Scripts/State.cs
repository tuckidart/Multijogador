﻿using UnityEngine;
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
	private Transform nullTransform;	//Used to set transforms back to null

	private ObstacleType currentObstacleType;
	//=================SENSORS

	void Awake () 
	{
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
		hasObstacle = true;
		obstacle = newObstacle;
		currentObstacleType = newObstacleType;
		HandleObstacle ();
	}

	public void RemoveObstacle ()
	{
		hasObstacle = false;
		obstacle = nullTransform;
	}
		
	void ApplyValues ()
	{

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
