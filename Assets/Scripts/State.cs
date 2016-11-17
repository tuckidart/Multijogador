﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class State : MonoBehaviour {

	public bool button;
	private bool isRight;
	public float y;

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
	public float obstacleDistance;
	public float curveDistance;

	public Transform[] obstacles;

	public ObstacleType[] currentObstacleType;
	//=================SENSORS

	private bool isTurning;
	private float initialYrot;
	private float targetRotation;
	private bool rotationIsOver;
	private int rightTurns;
	private bool completedTurning;
	private float startTurnTime;
	private int directionMultiplier;
	private Transform curve;

	public Transform way1;
	private Transform currentWaypoint;

	private vehicleController controller;

	void Awake () 
	{
		obstacles = new Transform[2];
		currentObstacleType = new ObstacleType[2];
		controller = GetComponent<vehicleController> ();
		turnedOn = false;
		moving = false;
		directionMultiplier = 1;
	}

	void Start ()
	{
		//transform.localRotation = Quaternion.Euler (transform.localRotation.eulerAngles.x, 0.00001f, transform.localRotation.eulerAngles.z);
		currentWaypoint = way1;
	}
	
	void Update () 
	{
		if (hasObstacle)
			CalculateObstacleDistance ();

		//Apply values to the car
		ApplyValues ();
	}

	void FixedUpdate ()
	{
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint( 
			currentWaypoint.position.x, 
			transform.position.y, 
			currentWaypoint.position.z );

		//Debug.Log (RelativeWaypointPosition.x);


		// by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
		float inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

		controller.inputX = inputSteer;

		if ( RelativeWaypointPosition.magnitude < 10 ) {
			currentWaypoint = currentWaypoint.GetComponent<Waypoint>().GetRandomWaypoint();

			//if ( currentWaypoint >= waypoints.length ) {
				//currentWaypoint = 0;
			//}
		}

		Debug.Log (inputSteer);
	}

	//public void ReceiveCar ()


	public void ReceiveObstacle (Transform newObstacle, ObstacleType newObstacleType)
	{
		for(int i=0;i<obstacles.Length;i++)
		{
			if (obstacles [i] == null)
			{
				obstacles[i] = newObstacle;
				currentObstacleType[i] = newObstacleType;
				hasObstacle = true;
				HandleObstacle ();
				return;
			}
		}
	}

	public void RemoveObstacle (Transform obstacleToBeRemoved)
	{
		for(int i=0;i<obstacles.Length;i++)
		{
			if (obstacles [i].transform.name == obstacleToBeRemoved.name)
			{
				obstacles [i] = null;
				currentObstacleType [i] = ObstacleType.NULL;
				if (obstacles [0] == null && obstacles [1] == null)
					hasObstacle = false;
				return;
			}
		}
	}

	void ApplyValues ()
	{
		if (obstacles[0] == null && obstacles[1] == null)
		{
			controller.inputY += 0.01f;
		}
		else
		{
			for (int i = 0; i < currentObstacleType.Length; i++)
			{
				if (currentObstacleType[i] == ObstacleType.car || currentObstacleType[i] == ObstacleType.light)
				{
					if (controller.zVel > 0)
						controller.inputY -= 1.0f / obstacleDistance;
					else
						controller.inputY = 0.0f;
				}
				if (currentObstacleType[i] == ObstacleType.curve && button)
				{
					if (controller.zVel > 0)
						controller.inputY -= 1.0f / curveDistance;
					if (controller.inputY <= 0.5f)
						controller.inputY = 0.5f;
				}
			}
		}

		if (button && curveDistance < 5.0f)
		{
			button = false;
		}
	}
		
	private void CalculateObstacleDistance ()
	{
		for (int i = 0; i < currentObstacleType.Length; i++)
		{
			if (currentObstacleType [i] != ObstacleType.curve && currentObstacleType [i] != ObstacleType.NULL)
			{
				obstacleDistance = Vector3.Distance (transform.position, obstacles [i].position);
			}
			else if (currentObstacleType [i] == ObstacleType.curve)
			{
				curveDistance = Vector3.Distance (transform.position, obstacles [i].position);
			}
		}
	}

	private void HandleObstacle ()
	{
		for (int i = 0; i < currentObstacleType.Length; i++)
		{
			if (currentObstacleType[i] == ObstacleType.curve)
			{
				//if (Random.Range(0, 2) == 1)
				//{
				if (obstacles[i].GetComponent<CurveScript> ().isRight)
				{
					//Set turning to +1
					//Compute obstacle multiplier
					button = true;
					isRight = true;
					directionMultiplier = 1;
					Debug.Log ("Curve - Turn right");
				} 
				else
				{
					//Set turning to -1
					//Compute obstacle multiplier
					button = true;
					isRight = false;
					directionMultiplier = -1;
					Debug.Log ("Curve - Turn left");
				}

				obstacles[i].GetComponent<CurveScript> ().IncreaseNumberOfCarsThatTurned ();
				//}
			}
			else if (currentObstacleType[i] == ObstacleType.light && obstacles[i].GetComponent<LightScript> ().isGreen == false)
			{
				//Fucking stop
				Debug.Log ("RED Light - Fucking stop");
			}
			else
			{
				//Fucking stop? Maybe a little slower
				Debug.Log ("Car - Fucking stop?");
			}
		}
	}
}