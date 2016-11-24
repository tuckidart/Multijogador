using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class State : NetworkBehaviour {

	private Transform currentWaypoint;
	private Transform cantGoToThisWaypoint;
	private Transform currentTrafficLights;
	private bool lightAux;

	//=================STATES
	private bool turnedOn;				//Level 1
	//Level 1
	private bool moving;				//Level 2

	//Level 2
	private float acceleration;			// -1 | 1
	private float turning;				// -1 | 1

	private bool parked;				//Used on initialize
	//=================STATES


	//=================SENSORS
	private bool hasObstacle;
	private float obstacleDistance;
	private float curveDistance;

	private Transform[] obstacles;

	private ObstacleType[] currentObstacleType;
	//=================SENSORS

	private int directionMultiplier;
	private Transform curve;

	private vehicleController controller;

//	void Awake () 
//	{
//		obstacles = new Transform[2];
//		currentObstacleType = new ObstacleType[2];
//		controller = GetComponent<vehicleController> ();
//		turnedOn = false;
//		moving = false;
//		directionMultiplier = 1;
//	}

	void Start ()
	{
		obstacles = new Transform[2];
		currentObstacleType = new ObstacleType[2];
		controller = GetComponent<vehicleController> ();
		turnedOn = false;
		moving = false;
		directionMultiplier = 1;

		Invoke("FireRaycast", 0.1f);
	}
	
	void Update ()
	{
		if (!isServer)
			return;
		
		for(int i=0;i<currentObstacleType.Length;i++)
		{
			if (currentObstacleType [i] == ObstacleType.car || (currentObstacleType [i] == ObstacleType.light && !obstacles[i].GetComponent<LightScript>().isGreen))
			{
				if(!lightAux)
				{
					hasObstacle = true;
					break;
				}
				else
				{
					hasObstacle = false;
					break;
				}
			}
			else
				hasObstacle = false;
		}

		if (hasObstacle)
			CalculateObstacleDistance ();
	}

	void FixedUpdate ()
	{
		//Apply values to the car
		ApplyValues ();
	}

	void ApplyValues ()
	{
		if (currentWaypoint == null)
			return;
			
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(currentWaypoint.position.x, transform.position.y, currentWaypoint.position.z);

		//by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
		controller.inputX = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

		controller.steering = ((100 / controller.zVel)*3.3f);
		if (controller.steering > 120)
			controller.steering = 120;

		if (RelativeWaypointPosition.magnitude < controller.zVel)
		{
			currentWaypoint = currentWaypoint.GetComponent<Waypoint>().GetRandomWaypoint(transform, cantGoToThisWaypoint);
		}

		if(!hasObstacle)
		{
			//controller.inputY += 0.01f;
			if (Mathf.Abs (controller.inputX) < 0.5f && Mathf.Abs (controller.inputX) > -0.5f)
				controller.inputY = RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude - Mathf.Abs (controller.inputX);
			else
			{
				controller.inputY -= 0.01f;
				if (controller.inputY <= 0.5f)
					controller.inputY = 0.5f;
			}
		}
		else if (hasObstacle)
		{
			if (controller.zVel > 0)
				controller.inputY -= 1.0f / obstacleDistance;
			else
				controller.inputY = 0.0f;
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

	private void HandleObstacle (Transform newObstacle, ObstacleType newObstacleType)
	{
		if (newObstacleType == ObstacleType.light)
		{
			if (!newObstacle.GetComponent<LightScript>().isGreen)
			{
				if (newObstacle.parent != currentTrafficLights)
				{
					//Fucking stop
					//Debug.Log ("RED Light - Fucking Stop!");
					hasObstacle = true;
					lightAux = false;
					currentTrafficLights = newObstacle.parent;
				}
				else
					lightAux = true;
			}
		}
		else if(newObstacleType == ObstacleType.car)
		{
			//Fucking stop? Maybe a little slower
			//Debug.Log ("Car - Slow Down!");
			hasObstacle = true;
		}
	}

	public void ReceiveObstacle (Transform newObstacle, ObstacleType newObstacleType)
	{
		for(int i=0;i<obstacles.Length;i++)
		{
			if (obstacles [i] == null)
			{
				obstacles[i] = newObstacle;
				currentObstacleType[i] = newObstacleType;
				HandleObstacle (obstacles [i], currentObstacleType[i]);
				return;
			}
		}
	}

	public void RemoveObstacle (Transform obstacleToBeRemoved)
	{
		if(obstacles[0] != null)
		{
			if (obstacles [0].transform.name == obstacleToBeRemoved.name)
			{
				obstacles [0] = null;
				currentObstacleType [0] = ObstacleType.NULL;
			}
		}
		if(obstacles[1] != null)
		{
			if (obstacles [1].transform.name == obstacleToBeRemoved.name)
			{
				obstacles [1] = null;
				currentObstacleType [1] = ObstacleType.NULL;
			}
		}
	}

	public void SetCantGoToThisWaypoint(Transform newC)
	{
		cantGoToThisWaypoint = newC;
	}

	private void FireRaycast ()
	{
		RaycastHit[] hits;
		hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);

		for (int i = 0; i < hits.Length; i++) {

			RaycastHit hit = hits[i];

			if (hits[i].transform.tag == "Curve") 
			{
				currentWaypoint = hits[i].transform;

				if (currentWaypoint.GetComponent<Waypoint>().brother != null)
				{
					cantGoToThisWaypoint = currentWaypoint.GetComponent<Waypoint>().brother;
				}

				return;
			}
		}
	}
}