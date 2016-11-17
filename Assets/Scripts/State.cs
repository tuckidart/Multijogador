using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class State : MonoBehaviour {

	private bool isRight;

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
	public bool hasObstacle;
	public float obstacleDistance;
	public float curveDistance;

	public Transform[] obstacles;

	public ObstacleType[] currentObstacleType;
	//=================SENSORS

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
		currentWaypoint = way1;
	}
	
	void Update () 
	{
		for(int i=0;i<currentObstacleType.Length;i++)
		{
			if (currentObstacleType [i] == ObstacleType.car || (currentObstacleType [i] == ObstacleType.light && !obstacles[i].GetComponent<LightScript>().isGreen))
			{
				hasObstacle = true;
				break;
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
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(currentWaypoint.position.x, transform.position.y, currentWaypoint.position.z);

		//by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
		controller.inputX = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

		controller.steering = (100 / controller.zVel) + 40;
		if (controller.steering > 120)
			controller.steering = 120;

		if (RelativeWaypointPosition.magnitude < controller.zVel)
		{
			currentWaypoint = currentWaypoint.GetComponent<Waypoint>().GetRandomWaypoint();
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
			
//			for (int i = 0; i < currentObstacleType.Length; i++)
//			{
//				if (currentObstacleType [i] == ObstacleType.car || (currentObstacleType [i] == ObstacleType.light && !obstacles[i].GetComponent<LightScript>().isGreen))
//				{
//					if (controller.zVel > 0)
//						controller.inputY -= 1.0f / obstacleDistance;
//					else
//						controller.inputY = 0.0f;
//				}
//				if (currentObstacleType [i] == ObstacleType.curve)
//				{
//					if (controller.zVel > 0)
//						controller.inputY -= 1.0f / curveDistance;
//					if (controller.inputY <= 0.5f)
//						controller.inputY = 0.5f;
//				}
//			}
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
//		if (newObstacleType == ObstacleType.curve)
//		{
//			if (newObstacle.GetComponent<Waypoint> ().isRight)
//			{
//				//Set turning to +1
//				//Compute obstacle multiplier
//				isRight = true;
//				directionMultiplier = 1;
//				Debug.Log ("Curve - Turn right");
//			} 
//			else
//			{
//				//Set turning to -1
//				//Compute obstacle multiplier
//				isRight = false;
//				directionMultiplier = -1;
//				Debug.Log ("Curve - Turn left");
//			}
//
//				//obstacles[i].GetComponent<CurveScript> ().IncreaseNumberOfCarsThatTurned ();
//		}
		if (newObstacleType == ObstacleType.light && !newObstacle.GetComponent<LightScript> ().isGreen)
		{
			//Fucking stop
			Debug.Log ("RED Light - Fucking Stop!");
			hasObstacle = true;
		}
		else if(newObstacleType == ObstacleType.car)
		{
			//Fucking stop? Maybe a little slower
			Debug.Log ("Car - Slow Down!");
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
//		for(int i=0;i<obstacles.Length;i++)
//		{
//			if (obstacles [i].transform.name == obstacleToBeRemoved.name)
//			{
//				obstacles [i] = null;
//				currentObstacleType [i] = ObstacleType.NULL;
//				if (obstacles [0] == null && obstacles [1] == null)
//					hasObstacle = false;
//				break;
//			}
//		}

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
}